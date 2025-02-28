using System.IO;

namespace Norman.Log.Config
{
	/// <summary>
	/// SqlLite数据库的设置
	/// StoragePath和DbPath 分别是数据库文件的存储路径和数据库文件的名称,比如 StoragePath = "D:\Logs", DbPath = "Logs.db"
	/// </summary>
	public class SqlLiteConfig : IDatabaseConfig
	{
		public string StoragePath { get; set; }
		public string DbPath { get; set; }
		public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Sqlite;
		public string ServerVersion { get; set; }

		public new string ToConnectionString()
			=> $"Data Source={Path.Combine(StoragePath, DbPath)}";
	}
}