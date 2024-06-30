/*
 
 应用程序的主配置文件,通过静态类的静态字段访问
 使用方式:
 1. 在程序启动时读取配置
 2. 使用AppConfig.LoggerConfig等静态字段访问配置信息

*/

namespace Norman.Log.Config
{
	/// <summary>
	/// 应用程序的主配置文件
	/// </summary>
	public static class AppConfig
	{
		/// <summary>
		/// 日志记录器的配置
		/// </summary>
		public static LoggerConfig LoggerConfig { get; } = LoggerConfig.Default;
		
		static AppConfig()
		{
			//从文件加载配置,默认的,如果文件不存在则会自动创建,并存储为默认配置
			LoggerConfig.LoadFromFile("logger.config");
		}
	}
}