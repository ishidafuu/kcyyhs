using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniJSONForTimeFlowShiki;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	[ExecuteInEditMode]
	public class ARIMotionMainWindow : EditorWindow
	{
		[SerializeField]
		public List<ScoreComponent> scores_ = new List<ScoreComponent>();
		//public JMCharManager charManager_;
		public Sprite sprite;
		public const int FPS = 60;
		public ARIMotionScoreWindow scoreWindow_;
		public ARIMotionSubWindow subWindow_;
		public static List<string> dirList_ = new List<string>();
		public static List<string> fileList_ = new List<string>();
		public static MotionCommandManager tackCmd_ = new MotionCommandManager(true);
		public static MotionCommandManager scoreCmd_ = new MotionCommandManager(false);

		Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
		//List<string> spriteLoadPath = new List<string>();
		Vector2 leftScrollPos_ = Vector2.zero;
		Vector2 rightScrollPos_ = Vector2.zero;
		const float DOT_PER_UNIT = 16f;
		string selectedDir_ = "";
		string textField_ = "";
		int activeScoreIndex_ = 0;
		bool isNeedSave_ = false;

		[UnityEditor.MenuItem("Window/ARIMotionWindow")]
		static void ShowMainWindow()
		{
			GetWindow(typeof(ARIMotionMainWindow));
		}

		public static Rect GetSpriteNormalRect(Sprite sp)
		{
			// spriteの親テクスチャー上のRect座標を取得.
			Rect rectPosition = sp.textureRect;

			// 親テクスチャーの大きさを取得.
			float parentWith = sp.texture.width;
			float parentHeight = sp.texture.height;
			// spriteの座標を親テクスチャーに合わせて正規化.
			Rect NormalRect = new Rect(
				rectPosition.x / parentWith,
				rectPosition.y / parentHeight,
				rectPosition.width / parentWith,
				rectPosition.height / parentHeight
			);

			return NormalRect;
		}

		//リロードセーブデータ
		public void ReloadSavedData()
		{
			this.scores_.Clear();

			foreach (var item in fileList_)
			{
				var dataPath = GetJsonPath() + "/" + item + ".json";
				var deserialized = new Dictionary<string, object>();

				if (File.Exists(dataPath))
				{
					deserialized = LoadData(dataPath);
				}

				if (deserialized.Any())
				{
					//Debug.Log("LoadScore");
					ScoreComponent loadScore = LoadScore(deserialized);
					if (loadScore.id_ != item)loadScore.id_ = item; //IDとファイル名合わせる
					this.scores_.Add(loadScore);

				}
			}

			// load demo data then save it.
			if (!this.scores_.Any())
			{
				foreach (enMotionType item in Enum.GetValues(typeof(enMotionType)))
				{
					var firstAuto = GenerateFirstScore(item.ToString());
					this.scores_.Add(firstAuto);
				}

				//生成されたヤツ全保存
				SaveDataAll();
				Debug.Log("SaveDataAll");
			}

			SetActiveScore(activeScoreIndex_);
		}

		//アクティブスコアセット
		public void SetActiveScore(int index)
		{
			if (index >= scores_.Count)index = 0;

			for (var i = 0; i < scores_.Count; i++)
			{
				if (i == index)
				{
					scores_[i].SetActive();
					activeScoreIndex_ = i;
				}
				else
				{
					scores_[i].SetDeactive();
				}
			}
		}

		//アクティブスコア取得
		public ScoreComponent GetActiveScore()
		{
			var res = scores_.Where(s => s.IsActive()).FirstOrDefault();
			return res;
		}

		//セーブが必要
		public void NeedSave()
		{
			isNeedSave_ = true;
		}

		public void SaveData2(bool isForce)
		{
			if (isForce || isNeedSave_)
			{
				//スコア全部ではなく、アクティブなスコアだけ保存する
				var score = GetActiveScore();
				GenSaveData(score);
			}
			isNeedSave_ = false;
		}

		public void SaveDataAll()
		{
			foreach (var score in scores_)
			{
				GenSaveData(score);
			}
		}

		//新規スコア作成
		public void AddNewScore()
		{
			if (textField_ == "")
			{
				EditorUtility.DisplayDialog("AddNewScore", "NewScoreNameを設定してください", "ok");
				return;
			}

			bool isSameId = scores_.Where(s => s.id_ == textField_).Any();

			if (isSameId)
			{
				EditorUtility.DisplayDialog("AddNewScore", textField_ + "は既に存在します", "ok");
				return;
			}

			if (!Directory.Exists(GetJsonPath()))
			{
				Directory.CreateDirectory(GetJsonPath());
			}

			//スコア全部ではなく、アクティブなスコアだけ保存する
			var score = GenerateFirstScore(WindowSettings.DEFAULT_SCORE_ID);
			score.id_ = textField_;

			var timelineList = new List<object>();
			foreach (var timeline in score.timelineTracks_)
			{
				//削除済み
				if (!timeline.IsExistTimeline_)continue;

				var tackList = new List<object>();
				//tack
				foreach (var tack in timeline.tackPoints_)
				{
					//削除済み
					if (!tack.IsExistTack_)continue;

					var tackDict = tack.OutputDict();
					tackList.Add(tackDict);
				}
				//timeline
				var timelineDict = timeline.OutputDict(tackList);

				timelineList.Add(timelineDict);
			}

			//score
			var scoreObject = score.OutputDict(timelineList);

			var dataStr = Json.Serialize(scoreObject);

			//var targetFilePath = Path.Combine(Application.dataPath, TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_FILEPATH);
			var targetFilePath = GetJsonPath() + "/" + score.id_ + ".json"; // "timeflowshiki.json";// + selectedFile;

			using(var sw = new StreamWriter(targetFilePath))
			{
				sw.Write(dataStr);
			}

			//更新
			RefreshDirectory();
			RefreshScoreFile();
			ReloadSavedData();
			RepaintAllWindow();
		}

		public void Emit(OnTrackEvent onTrackEvent)
		{
			var type = onTrackEvent.eventType;

			switch (type)
			{
				case OnTrackEvent.EventType.EVENT_MAIN_ADDSCORE:
					{
						return;
					}

				default:
					{
						Debug.LogError("no match type:" + type);
						break;
					}
			}
		}

		public void RepaintAllWindow()
		{
			Repaint();
			if (scoreWindow_ != null)scoreWindow_.Repaint();
			if (subWindow_ != null)subWindow_.Repaint();
		}

		void OnEnable()
		{
			// handler for Undo/Redo
			Undo.undoRedoPerformed += () =>
			{
				NeedSave();
				RepaintAllWindow();
			};

			RefreshDirectory();
			ReloadSavedData();
			LoadSprite();
		}

		void OnGUI()
		{
			try
			{
				EditorGUILayout.LabelField("JMMotionGUIWindow");
				InputLayout();
				//新規マップ
				DrawButtonOpenEditorWindow();
				//アイコン＋名前
				DrawImageParts();
				//マウス入力イベント設定
				HandlingMouseEvent();
			}
			catch (System.Exception exeption)
			{
				if (exeption is ExitGUIException)
				{
					throw exeption;
				}
				else
				{
					Debug.LogError(exeption.ToString());
				}
			}
		}

		//入力フォームの描画
		void InputLayout()
		{
			//入力フォーム
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal();
			////GUILayout.FlexibleSpace();
			//var charManager = EditorGUILayout.ObjectField("charManager", this.charManager_, typeof(JMCharManager), true);
			////var holdCharManager = EditorGUILayout.ObjectField("holdCharManager", this.holdCharManager_, typeof(JMCharManager), true);

			GUILayout.EndHorizontal();
		}

		// 画像一覧をボタン選択出来る形にして出力
		void DrawImageParts()
		{

			EditorGUILayout.BeginHorizontal(GUI.skin.box);

			EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(100));
			EditorGUILayout.LabelField("技セットフォルダ");
			if (GUILayout.Button("Refresh", GUILayout.Height(16), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
			{
				RefreshButton();
			}

			if (GUILayout.Button("Save", GUILayout.Height(16), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
			{
				SaveDataAll();
			}

			// 左側のスクロールビュー(横幅300px)
			leftScrollPos_ = EditorGUILayout.BeginScrollView(leftScrollPos_, GUI.skin.box);
			{
				float y = 00.0f;

				EditorGUILayout.BeginVertical();
				int index = 0;

				foreach (var item in dirList_)
				{
					GUIContent contents = new GUIContent();

					contents.text = Path.GetFileName(item);

					int TIPSIZE = 32;
					if (selectedDir_ == item)
					{
						GUI.color = new Color(1f, 0.5f, 1f, 1f);
					}
					else
					{
						GUI.color = new Color(1f, 1f, 1f, 1f);
					}

					//フォルダ変更
					if (GUILayout.Button(contents, GUILayout.Height(TIPSIZE), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
					{
						ChangeDirButton(item);
					}
					if (selectedDir_ == item)GUI.color = new Color(1f, 1f, 1f, 1f);

					Rect lastRect = GUILayoutUtility.GetLastRect();

					lastRect.width = TIPSIZE;
					lastRect.height = TIPSIZE;
					y += TIPSIZE;
					index++;
				}

				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndScrollView();

			if (GUILayout.Button("SaveAll"))SaveDataAll();
			//if (GUILayout.Button("CreateScriptableObject")) CreateObject(false);
			//if (GUILayout.Button("CreateScriptableObjectAll")) CreateObjectAll();
			if (GUILayout.Button("CreateAniScriptSheet"))CreateAniScriptSheet();
			if (GUILayout.Button("CreateAniBaseScript"))CreateAniBaseScript();
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();
			rightScrollPos_ = EditorGUILayout.BeginScrollView(rightScrollPos_, GUI.skin.box);
			EditorGUILayout.LabelField("技リスト");
			//if(imgDirectory != null)
			{
				float y = 00.0f;

				int index = 0;

				foreach (var item in this.scores_)
				{
					GUIContent contents = new GUIContent();
					contents.text = item.id_.ToString(); // + " : " + item.title_;

					int TIPSIZE = 48;
					if (item.IsActive())
					{
						GUI.color = new Color(1f, 0.5f, 1f, 1f);
					}
					else
					{
						GUI.color = new Color(1f, 1f, 1f, 1f);
					}

					if (GUILayout.Button(contents, GUILayout.Height(TIPSIZE), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
					{
						ChangeScoreButton(index);
					}
					if (item.IsActive())GUI.color = new Color(1f, 1f, 1f, 1f);

					Rect lastRect = GUILayoutUtility.GetLastRect();

					lastRect.width = TIPSIZE;
					lastRect.height = TIPSIZE;
					y += TIPSIZE;
					index++;
				}
			}

			EditorGUILayout.EndScrollView();
			GUILayout.Label("NewScoreName");
			textField_ = GUILayout.TextField(textField_);
			if (GUILayout.Button("AddNewScore"))AddNewScore();

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
		}

		//リフレッシュボタン
		void RefreshButton()
		{
			RefreshDirectory();
			if (dirList_.Any())selectedDir_ = dirList_[0];
			RefreshScoreFile();
			ReloadSavedData();
			RepaintAllWindow();
		}

		//ディレクトリ変更ボタン
		void ChangeDirButton(string item)
		{
			if (selectedDir_ != item)
			{
				SaveDataAll();
				selectedDir_ = item;
				RefreshScoreFile();
				ReloadSavedData();
				RepaintAllWindow();
			}
		}

		//スコア変更ボタン
		void ChangeScoreButton(int index)
		{
			//Undoクリア
			tackCmd_.Clear();
			scoreCmd_.Clear();
			SetActiveScore(index);
			scoreWindow_.SetZeroFrame();
			OpenEditorWindow();
			OpenSubWindow();
			RepaintAllWindow();
		}

		//マウス入力イベント設定
		void HandlingMouseEvent()
		{
			//var repaint = false;
			var useEvent = false;
			switch (Event.current.type)
			{
				case EventType.ContextClick:
					{
						ShowContextMenu();
						useEvent = true;
						break;
					}
				case EventType.MouseUp:
					{

						// right click.
						if (Event.current.button == 1)
						{
							ShowContextMenu();
						}
						useEvent = true;

						break;
					}
			}

			if (useEvent)Event.current.Use();
		}

		//右クリックメニュー
		void ShowContextMenu()
		{
			var nearestTimelineIndex = 0; // fixed. should change by mouse position.
			var menu = new GenericMenu();
			var menuItems = new Dictionary<string, OnTrackEvent.EventType>
				{ { "Add New Score \\", OnTrackEvent.EventType.EVENT_MAIN_ADDSCORE },
				};

			foreach (var key in menuItems.Keys)
			{
				var eventType = menuItems[key];
				var enable = true; // IsEnableEvent(eventType);
				if (enable)
				{
					menu.AddItem(
						new GUIContent(key),
						false,
						() =>
						{
							Emit(new OnTrackEvent(eventType, null, nearestTimelineIndex));
						}
					);
				}
				else
				{
					menu.AddDisabledItem(new GUIContent(key));
				}
			}

			menu.ShowAsContext();
		}

		// エディタウィンドウを開くボタンを生成
		void DrawButtonOpenEditorWindow()
		{
			EditorGUILayout.BeginVertical();
			//GUILayout.FlexibleSpace();
			if (GUILayout.Button("OpenEditorWindow"))
			{
				OpenEditorWindow();
			}
			if (GUILayout.Button("OpenSubWindow"))
			{
				OpenSubWindow();
			}
			EditorGUILayout.EndVertical();
		}

		//リフレッシュディレクトリ
		void RefreshDirectory()
		{
			dirList_.Clear();
			var dataPath = Path.Combine(Application.dataPath, TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_PATH);

			string[] fs = System.IO.Directory.GetDirectories(dataPath, "*");
			foreach (var item in fs)
			{
				dirList_.Add(Path.GetFileName(item));
			}

			if ((selectedDir_ == "") && (dirList_.Any()))selectedDir_ = dirList_[0];

			RefreshScoreFile();
		}

		//リフレッシュスコアファイル
		void RefreshScoreFile()
		{
			fileList_.Clear();
			string[] fs = System.IO.Directory.GetFiles(GetJsonPath(), "*.json");
			foreach (var item in fs)
			{
				//Debug.Log(item);
				fileList_.Add(Path.GetFileNameWithoutExtension(item)); //拡張子抜き
			}
		}

		//エディタウインドウ開く
		void OpenEditorWindow()
		{
			if (scoreWindow_ == null)
			{
				scoreWindow_ = ARIMotionScoreWindow.ShowEditor(this);
				ARIMotionScoreWindow.ParentEmit = Emit;
			}
			else
			{
				scoreWindow_.Focus();
			}
		}

		//サブウインドウ開く
		void OpenSubWindow()
		{
			if (subWindow_ == null)
			{
				subWindow_ = ARIMotionSubWindow.ShowEditor(this);
				ARIMotionSubWindow.ParentEmit = Emit;
			}
			else
			{
				subWindow_.Focus();
			}
		}

		//フォーカス
		void FocusEditorWindow()
		{
			if (scoreWindow_ != null)
			{
				scoreWindow_.Focus();
			}
		}

		//ロードデータ
		Dictionary<string, object> LoadData(string dataPath)
		{
			var dataStr = string.Empty;

			using(var sr = new StreamReader(dataPath))
			{
				dataStr = sr.ReadToEnd();
			}
			return Json.Deserialize(dataStr)as Dictionary<string, object>;
		}

		//スコアファイルを単体JSONに分ける
		ScoreComponent LoadScore(Dictionary<string, object> deserialized)
		{
			var scoreTimelines = deserialized[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_SCORE_TIMELINES] as List<object>;
			var currentTimelines = new List<TimelineTrack>();
			foreach (var scoreTimeline in scoreTimelines)
			{
				var scoreTimelineDict = scoreTimeline as Dictionary<string, object>;

				var timelineTacks = scoreTimelineDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TIMELINE_TACKS] as List<object>;
				var currentTacks = new List<TackPoint>();
				foreach (var timelineTack in timelineTacks)
				{
					var timelineTacksDict = timelineTack as Dictionary<string, object>;
					var newTack = new TackPoint(currentTacks.Count, timelineTacksDict);
					currentTacks.Add(newTack);
				}
				var newTimeline = new TimelineTrack(currentTimelines.Count, scoreTimelineDict, currentTacks);
				currentTimelines.Add(newTimeline);
			}
			var newScore = new ScoreComponent(deserialized, currentTimelines);
			return newScore;
		}

		//新規作成時のプレーンスコア（ポジション）
		ScoreComponent GenerateFirstScore(string id)
		{
			var tackPoints = new List<TackPoint>();
			tackPoints.Add(new TackPoint(0, 0, 10, (int)TimelineType.TL_POS));

			var timelines = new List<TimelineTrack>();
			timelines.Add(new TimelineTrack(0, (int)TimelineType.TL_POS, tackPoints));
			return new ScoreComponent(id, WindowSettings.DEFAULT_SCORE_INFO, timelines);
		}

		//セーブ
		void GenSaveData(ScoreComponent score)
		{
			if (!Directory.Exists(GetJsonPath()))
			{
				Directory.CreateDirectory(GetJsonPath());
			}

			if (score == null)return;

			var timelineList = new List<object>();
			foreach (var timeline in score.timelineTracks_)
			{
				//削除済み
				if (!timeline.IsExistTimeline_)continue;

				var tackList = new List<object>();
				//tack
				foreach (var tack in timeline.tackPoints_)
				{
					//削除済み
					if (!tack.IsExistTack_)continue;

					var tackDict = tack.OutputDict();
					tackList.Add(tackDict);
				}
				//timeline
				var timelineDict = timeline.OutputDict(tackList);

				timelineList.Add(timelineDict);
			}

			//score
			var scoreObject = score.OutputDict(timelineList);

			var dataStr = Json.Serialize(scoreObject);

			//var targetFilePath = Path.Combine(Application.dataPath, TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_FILEPATH);
			var targetFilePath = GetJsonPath() + "/" + score.id_ + ".json"; // "timeflowshiki.json";// + selectedFile;

			Debug.Log("<color=red>SaveActiveScore:" + targetFilePath + "</color>");

			using(var sw = new StreamWriter(targetFilePath))
			{
				sw.Write(dataStr);
			}

			//セーブしたときはとりあえず全ウインド書き換え
			RepaintAllWindow();
		}

		// ファイルで出力
		void CreateObject(bool isAll)
		{
			CreateObjectPack(isAll);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			// 完了ポップアップ
			//if (!isAll) EditorUtility.DisplayDialog("CreateObject", selectedDir + "を保存しました。", "ok");
		}

		//基礎モーションはひとまとめ
		void CreateObjectPack(bool isAll)
		{
			MotionScores obj = CreateInstance(typeof(MotionScores))as MotionScores;

			foreach (var item in scores_)
			{
				obj.scores.Add(item.CreateMotionScoreObject());
			}
			AssetDatabase.CreateAsset(obj, GetPackMotionScoresResourcesPath());
			Debug.Log("CreateObject:" + selectedDir_ + ".asset");
		}

		//必殺モーションは単体
		void CreateObjectSingle()
		{
			foreach (var item in scores_)
			{
				MotionScores obj = CreateInstance(typeof(MotionScores))as MotionScores;
				obj.scores.Add(item.CreateMotionScoreObject());
				//var obj = item.CreateMotionScoreObject();
				Debug.Log("CreateObject:" + selectedDir_ + "/" + item.id_ + ".asset");
				//AssetDatabase.CreateAsset(obj, GetMotionScorePath(item.id_));
				AssetDatabase.CreateAsset(obj, GetSingleMotionScoreResourcesPath(item.id_));
			}
		}

		// ファイルで出力
		void CreateObjectAll()
		{
			RefreshDirectory();
			foreach (var item in dirList_)
			{
				selectedDir_ = item;
				RefreshScoreFile();
				ReloadSavedData();
				CreateObject(true);
			}
			// 完了ポップアップ
			EditorUtility.DisplayDialog("CreateObjectAll", dirList_.Count.ToString() + "の技セットオブジェを保存しました。", "ok");
		}

		// ファイルで出力
		void CreateAniScriptSheet()
		{
			AniScriptSheetObject obj = CreateInstance(typeof(AniScriptSheetObject))as AniScriptSheetObject;
			obj.scripts = new List<AniScript>();

			foreach (NKKD.EnumMotion motion in Enum.GetValues(typeof(NKKD.EnumMotion)))
			{
				foreach (var item in scores_)
				{
					if (item.id_.IndexOf(motion.ToString()) != -1)
					{
						//Debug.Log(motion.ToString());
						obj.scripts.Add(item.CreateAniScriptObject());
						break;
					}
				}
			}

			//foreach (var item in obj.scripts) {
			//	Debug.Log(item.frames[0].ant);
			//}
			AssetDatabase.CreateAsset(obj, GetPackPath("AniScriptSheet"));
			//Debug.Log("CreateAniScript:" + selectedDir_ + ".asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			// 完了ポップアップ
			EditorUtility.DisplayDialog("CreateAniScript", "AniScriptSheetを保存しました。", "ok");
		}

		// ファイルで出力
		void CreateAniBaseScript()
		{
			AniBasePosObject obj = CreateInstance(typeof(AniBasePosObject))as AniBasePosObject;
			//obj.aniBasePos = new AniBasePos();
			obj.aniBasePos.FRONTDEPTH = BasePosition.OutputDepth(false);
			obj.aniBasePos.BACKDEPTH = BasePosition.OutputDepth(true);
			obj.aniBasePos.BODY_BASE = BasePosition.BODY_BASE;
			obj.aniBasePos.HEAD_BASE = BasePosition.HEAD_BASE;
			obj.aniBasePos.L_ARM_BASE = BasePosition.L_ARM_BASE;
			obj.aniBasePos.R_ARM_BASE = BasePosition.R_ARM_BASE;
			obj.aniBasePos.L_HAND_BASE = BasePosition.L_HAND_BASE;
			obj.aniBasePos.R_HAND_BASE = BasePosition.R_HAND_BASE;
			obj.aniBasePos.L_LEG_BASE = BasePosition.L_LEG_BASE;
			obj.aniBasePos.R_LEG_BASE = BasePosition.R_LEG_BASE;
			obj.aniBasePos.L_FOOT_BASE = BasePosition.L_FOOT_BASE;
			obj.aniBasePos.R_FOOT_BASE = BasePosition.R_FOOT_BASE;
			obj.aniBasePos.ANT_BASE = BasePosition.ANT_BASE;

			//obj.aniBasePos.LOOKDEPTH = BasePosition.OutputDepth(enPartsAngle.Look);
			//obj.aniBasePos.SIDEDEPTH = BasePosition.OutputDepth(enPartsAngle.Side);
			//obj.aniBasePos.REARDEPTH = BasePosition.OutputDepth(enPartsAngle.Rear);

			AssetDatabase.CreateAsset(obj, GetPackPath("AniBasePos"));
			Debug.Log("CreateAniBaseScript:" + selectedDir_ + ".asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			// 完了ポップアップ
			EditorUtility.DisplayDialog("CreateAniBaseScript", "AniBasePosを保存しました。", "ok");
		}

		//JSONパス
		string GetJsonPath()
		{
			return Path.Combine(Application.dataPath, TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_PATH + selectedDir_);
		}

		string GetMotionScorePath(string scoreId)
		{
			return TimeFlowShikiSettings.CREATEOBJECTPATH + selectedDir_ + "/" + scoreId + ".asset";
		}

		string GetPackMotionScoresResourcesPath()
		{
			string resourcesDir = TimeFlowShikiSettings.CREATEOBJECTPATH + "Resources/";
			if (!Directory.Exists(resourcesDir))Directory.CreateDirectory(resourcesDir);
			return resourcesDir + selectedDir_ + ".asset";
		}

		string GetPackPath(string objName)
		{
			string resourcesDir = TimeFlowShikiSettings.CREATEOBJECTPATH + "Resources/";
			if (!Directory.Exists(resourcesDir))Directory.CreateDirectory(resourcesDir);
			return resourcesDir + objName + ".asset";
		}

		string GetSingleMotionScoreResourcesPath(string scoreId)
		{
			string resourcesDir = TimeFlowShikiSettings.CREATEOBJECTPATH + "Resources/";
			string scoreDir = resourcesDir + selectedDir_ + "/";

			if (!Directory.Exists(resourcesDir))Directory.CreateDirectory(resourcesDir);
			if (!Directory.Exists(scoreDir))Directory.CreateDirectory(scoreDir);

			return scoreDir + scoreId + ".asset";
		}

		public Sprite GetSprite(enPartsType partsType, bool isBack, int faceNo)
		{

			int typeNo = 0;
			switch (partsType)
			{
				case enPartsType.Body:
					typeNo = 0;
					break;
				case enPartsType.LeftArm:
				case enPartsType.RightArm:
					typeNo = 1;
					break;
				case enPartsType.LeftHand:
				case enPartsType.RightHand:
					typeNo = 2;
					break;
				case enPartsType.LeftLeg:
				case enPartsType.RightLeg:
					typeNo = 3;
					break;
				case enPartsType.LeftFoot:
				case enPartsType.RightFoot:
					typeNo = 4;
					break;
				case enPartsType.Head:
					typeNo = 5;
					break;
				case enPartsType.Ant:
					typeNo = 6;
					break;
				default:
					Debug.LogError("other partsType_");
					break;
			}

			var spriteName = TimeFlowShikiSettings.SPRITE_NAME + "_" + typeNo.ToString();
			if (!spriteDic.ContainsKey(spriteName))
			{
				Debug.Log("GetSprite NotFound : " + spriteName);
				return null;
			}
			return spriteDic[spriteName];
		}

		public int LoadSprite()
		{
			spriteDic = new Dictionary<string, Sprite>();

			// 読み込み(Resources.LoadAllを使うのがミソ)
			UnityEngine.Object[] list = Resources.LoadAll(TimeFlowShikiSettings.SPRITE_NAME, typeof(Sprite));

			// listがnullまたは空ならエラーで返す
			if (list == null || list.Length == 0)return -1;

			int i, len = list.Length;

			// listを回してDictionaryに格納
			for (i = 0; i < len; ++i)
			{
				//Debug.Log("Add : " + list[i].name);
				if (spriteDic.ContainsKey(list[i].name))continue;

				spriteDic.Add(list[i].name, list[i] as Sprite);
			}
			return len;
		}

	}
}