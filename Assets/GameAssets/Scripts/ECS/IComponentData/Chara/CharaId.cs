using Unity.Entities;
namespace NKKD
{
	/// <summary>
	/// キャラ識別情報
	/// </summary>
	public struct CharaId : IComponentData
	{
		public int familyId;
		public int myId;
	}
}