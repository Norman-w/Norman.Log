namespace Norman.Log.Config
{
	/// <summary>
	/// 日志服务器的设置,日志服务器的设置不包含记录到日志服务器的设置.因为他本身就是一个日志服务器,不需要记录到其他服务器,只需要记录到文件或者数据库
	/// </summary>
	public class LogPersistenceConfig : CommonConfig, ICommonConfig<LogPersistenceConfig>
	{
		private LogPersistenceConfig _default;

		public LogPersistenceConfig GetDefault()
		{
			return _default ?? (_default = new LogPersistenceConfig
			{
				LogToFile = LogToFileConfig.Default,
				LogToDatabase = LogToDatabaseConfig.Default,
			});
		}

		public void FromFile(string path, bool tryCreateIfNotExist = true)
		{
			PopulateByFile(path, tryCreateIfNotExist, _default);
		}
	}
}