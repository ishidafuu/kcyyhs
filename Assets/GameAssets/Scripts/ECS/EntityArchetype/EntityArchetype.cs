using Unity.Entities;
using Unity.Transforms;
namespace NKKD
{
	public static class ComponentTypes
	{
		/// <summary>
		/// プレーヤー
		/// </summary>
		/// <value></value>
		// public static ComponentType[] PlayerComponentType = {
		// 	typeof(PlayerTag), //プレイヤータグ
		// 	typeof(PlayerId), //プレイヤーId
		// 	typeof(PadInput), //入力
		// };
		/// <summary>
		/// キャラ
		/// </summary>
		/// <value></value>
		public static ComponentType[] CharaComponentType = {
			typeof(CharaTag), //キャラタグ
			// typeof(PadInput), //入力（全員が持っているわけではない）
			typeof(Position), //描画位置
			typeof(CharaMove), //座標
			typeof(CharaMuki), //向き
			typeof(CharaDash), //ダッシュ
			// typeof(CharaFlag), //フラグ
			typeof(CharaId), //ID
			// typeof(CharaBehave), //蟻行動タイプ
			typeof(CharaLook), //向き
			typeof(CharaMotion), //モーション
			// typeof(GridPos), //チップ位置
		};

		// //地面
		// public static ComponentType[] MapTipType = {
		// 	typeof(TipObj), //チップ
		// 	typeof(GridPos), //チップ位置
		// 	typeof(TipSurface), //チップ材質
		// 	typeof(TipPheromL), //フェロモン低揮発
		// 	typeof(TipPheromH), //フェロモン高揮発
		// 	//typeof(Position2D), //位置
		// 	//typeof(TransformMatrix), //描画位置
		// 	//typeof(MeshInstanceRenderer), //描画素材(SharedCompornentだけど追加しないとNG)
		// };

		// //食べもの
		// public static ComponentType[] FoodType = {
		// 	typeof(FoodObj), //蟻
		// 	typeof(Position), //位置
		// 	typeof(FoodData), //データ
		// };

		////地面描画
		//public static ComponentType[] GeoType = {
		//	typeof(GeoObj),
		//	typeof(GeoMeshMat),
		//};
	}
}