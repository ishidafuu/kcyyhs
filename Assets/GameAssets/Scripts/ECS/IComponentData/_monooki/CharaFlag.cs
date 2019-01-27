// using Unity.Entities;
// namespace NKKD
// {
// 	/// <summary>
// 	/// キャラの状態フラグ
// 	/// </summary>
// 	public struct CharaFlag : IComponentData
// 	{
// 		public EnumFlagMotion motionFlags;

// 		public bool HasFlag(EnumFlagMotion flag)
// 		{
// 			return motionFlags.HasFlag(flag);
// 		}

// 		/// <summary>
// 		/// フラグON
// 		/// </summary>
// 		/// <param name="flag"></param>
// 		public void AddFlag(EnumFlagMotion flag)
// 		{
// 			motionFlags |= flag;
// 		}

// 		/// <summary>
// 		/// フラグOFF
// 		/// </summary>
// 		/// <param name="flag"></param>
// 		public void SubFlag(EnumFlagMotion flag)
// 		{
// 			motionFlags &= ~flag;
// 		}
// 	}
// }