using Unity.Entities;
using Unity.Transforms;
namespace YYHS
{
	public static class ComponentTypes
	{
		/// <summary>
		/// キャラ
		/// </summary>
		/// <value></value>
		public static ComponentType[] CharaComponentType = {
			typeof(CharaTag), //キャラタグ
			// typeof(PadInput), //入力（CharaEntityFactoryで必要なキャラのみつける）
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
	}
}