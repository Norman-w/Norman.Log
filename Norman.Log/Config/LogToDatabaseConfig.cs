using System;

namespace Norman.Log.Config
{
	/// <summary>
	/// 记录到数据库的设置
	/// </summary>
	public class LogToDatabaseConfig
	{
		public bool OnOff { get; set; }

		/// <summary>
		/// 数据库的设置
		/// </summary>
		public IDatabaseConfig DatabaseConfig { get; set; }

		public string TableName { get; set; }

		/// <summary>
		/// 当缓存中的日志条数大于等于多少条的时候,即使没到时间也写入到数据库
		/// </summary>
		/// <returns></returns>
		public uint MaxLogCountInCache { get; set; }

		/// <summary>
		/// 不管日志缓存中的日志条数是多少,只要距离上次写入数据库的时间超过这个时间就写入到数据库
		/// 每次写入日志后,计时器会重新计时,到了这个时间就写入数据库
		/// </summary>
		public TimeSpan WriteToDatabaseInterval { get; set; }

		/// <summary>
		/// 每次写入数据库的最大条数,超过这个条数就分批写入
		/// </summary>
		public uint MaxFlushCountPerTime { get; set; }

		public static LogToDatabaseConfig Default { get; } = new LogToDatabaseConfig
		{
			OnOff = false,
			DatabaseConfig = new MySqlConfig()
			{
				Host = "localhost",
				Port = 3306,
				UserName = "root",
				Password = "root",
				DatabaseName = "NormanLog",
			},
			TableName = "Logs",
			MaxLogCountInCache = 100,
			WriteToDatabaseInterval = TimeSpan.FromSeconds(5),
			MaxFlushCountPerTime = 1000,
		};
	}
}