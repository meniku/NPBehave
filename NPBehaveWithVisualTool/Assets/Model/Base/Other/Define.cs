namespace ETModel
{
	public static class Define
	{
#if UNITY_EDITOR
		public static bool IsEditorMode = true;
#else
		public static bool IsEditorMode = false;
#endif

#if DEVELOPMENT_BUILD
		public static bool IsDevelopmentBuild = true;
#else
		public static bool IsDevelopmentBuild = false;
#endif

#if ILRuntime
		public static bool IsILRuntime = true;
#else
		public static bool IsILRuntime = false;
#endif

		/// <summary>
		/// 当前资源模式是否是编辑器模式，默认为是，打包时注意取消勾选
		/// </summary>
		public static bool ResModeIsEditor = true;
	}
}