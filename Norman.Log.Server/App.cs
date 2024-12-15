using Norman.Log.Config;

namespace Norman.Log.Server;

/// <summary>
/// 全局可用的应用程序类
/// </summary>
public static class App
{
	/// <summary>
	///     core server的实例
	/// </summary>
	internal static readonly Core.Server Server = new();

	/// <summary>
	///     日志记录器设置
	/// </summary>
	public static LoggerConfig LoggerConfig { get; } =
		ConfigFactory.CreateFromFile<LoggerConfig>("LoggerConfig.config", true);

	/// <summary>
	/// 应用程序主设置,也就是Server的设置
	/// </summary>
	internal static Setting Setting { get; } = ConfigFactory.CreateFromFile<Setting>("Setting.config", true);
}