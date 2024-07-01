namespace Norman.Log.Server;

/// <summary>
/// 全局可用的应用程序类
/// </summary>
public static class App
{
	/// <summary>
	/// 应用程序的设置
	/// </summary>
	internal static Setting Setting { get; } = new();
}