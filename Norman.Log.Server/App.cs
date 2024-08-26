using Norman.Log.Config;

namespace Norman.Log.Server;

/// <summary>
/// 全局可用的应用程序类
/// </summary>
public static class App
{
	public static LoggerConfig LoggerConfig { get; } = ConfigFactory.CreateFromFile<LoggerConfig>("logServer.config", true);

	/// <summary>
	/// 应用程序的设置
	/// </summary>
	internal static Setting Setting { get; } = new();

	/// <summary>
	/// core server的实例
	/// </summary>
	internal static readonly Core.Server Server = new();
}