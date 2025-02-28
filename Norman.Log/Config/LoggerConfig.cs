/*


 日志记录器设置.
 日志记录器是本日志系统的三大组成部分之一,也是最重要的部分.
 主要负责直接调用Logger的代码进行日志记录.具体的Write/Log到什么地方要看这个配置.
 什么样的规则等信息也都需要参照这个配置.



*/

namespace Norman.Log.Config
{

	/// <summary>
	/// 日志记录器设置
	/// </summary>
	public class LoggerConfig : CommonConfig, ICommonConfig<LoggerConfig>
	{
		private LoggerConfig _default;

		public LoggerConfig GetDefault()
		{
			return _default ?? (_default = new LoggerConfig
			{
				LogToFile = LogToFileConfig.Default,
				LogToDatabase = LogToDatabaseConfig.Default,
				LogToServer = LogToLogServerConfig.Default,
			});
		}

		public void FromFile(string path, bool tryCreateIfNotExist = true)
		{
			PopulateByFile(path, tryCreateIfNotExist, _default);
		}

		/// <summary>
		/// 记录到日志服务器的设置
		/// </summary>
		public LogToLogServerConfig LogToServer { get; set; }
	}
}