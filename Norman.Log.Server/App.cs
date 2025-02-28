using Newtonsoft.Json;
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
		Setting = CommonConfig.CreateFromFile<Setting>("LogServer.setting", true);
		Server = new Core.Server();
		
		Init();
	}

	#region 全局初始化

	private static void Init()
	{
		if (Setting.LogToDatabase is { OnOff: true, DatabaseConfig: not null })
		{

			NormanLogDbContext.ConnectionString =
				Setting.LogToDatabase.DatabaseConfig.ToConnectionString();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"数据库连接字符串:{NormanLogDbContext.ConnectionString}");
			Console.ResetColor();
		}
		var currentSettingJson = JsonConvert.SerializeObject(Setting, Formatting.Indented);
		//控制台输出当前所使用的配置文件都是哪些,方便调试
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"当前使用的LogServer设置文件(Setting):{Setting.CurrentConfigFilePath}");
		Console.WriteLine($"当前使用的LogServer设置:{Environment.NewLine}{currentSettingJson}");
		Console.ResetColor();
	}

	#endregion

	/// <summary>
	///     core server的实例
	/// </summary>
	internal static readonly Core.Server Server;


	#region 全局配置

	#region Setting

	/// <summary>
	///    应用程序主设置,也就是Server的设置
	/// </summary>
	internal static Setting Setting { get; }

	#endregion

	#endregion
}