using Norman.Log.Component.Database.Mysql.Context;
using Norman.Log.Config;

namespace Norman.Log.Server;

/// <summary>
/// 全局可用的应用程序类
/// </summary>
public static class App
{
	static App()
	{
		LogPersistenceConfig = ConfigFactory.CreateFromFile<LogPersistenceConfig>("LogPersistence.config", true);
		Setting = ConfigFactory.CreateFromFile<Setting>("LogServer.setting", true);
		Server = new Core.Server();
		
		Init();
	}

	#region 全局初始化

	private static void Init()
	{
		if (LogPersistenceConfig.LogToDatabase is not { OnOff: true, DatabaseConfig: not null }) return;
		
		NormanLogDbContext.ConnectionString =
			LogPersistenceConfig.LogToDatabase.DatabaseConfig.ToConnectionString();
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"数据库连接字符串:{NormanLogDbContext.ConnectionString}");
		Console.ResetColor();
	}

	#endregion

	/// <summary>
	///     core server的实例
	/// </summary>
	internal static readonly Core.Server Server;


	#region 全局配置

	#region LoggerConfig

	/// <summary>
	/// 日志持久化配置
	/// </summary>
	public static LogPersistenceConfig LogPersistenceConfig { get; }

	#endregion

	#region Setting

	/// <summary>
	///    应用程序主设置,也就是Server的设置
	/// </summary>
	internal static Setting Setting { get; }

	#endregion

	#endregion
}