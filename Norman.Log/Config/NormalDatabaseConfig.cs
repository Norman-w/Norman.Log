using System;

namespace Norman.Log.Config
{
	/// <summary>
	/// 常规的数据库设置
	/// </summary>
	public abstract class NormalDatabaseConfig : IDatabaseConfig
	{
		public string Host { get; set; }
		public uint Port { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string DatabaseName { get; set; }
		public abstract DatabaseTypeEnum DatabaseType { get; }

		public string ServerVersion { get; set; }

		public new string ToConnectionString()
			=> $"Server={Host};Port={Port};Database={DatabaseName};User Id={UserName};Password={Password};";

		internal static Type GetDatabaseConfigType(DatabaseTypeEnum databaseType)
		{
			switch (databaseType)
			{
				case DatabaseTypeEnum.Mysql:
					return typeof(MySqlConfig);
				case DatabaseTypeEnum.MariaDb:
					return typeof(MariaDbConfig);
				case DatabaseTypeEnum.MsSql:
					return typeof(MsSqlConfig);
				case DatabaseTypeEnum.PostgreSql:
					return typeof(PostgreSqlConfig);
				case DatabaseTypeEnum.Oracle:
					return typeof(OracleConfig);
				case DatabaseTypeEnum.Sqlite:
					return typeof(SqlLiteConfig);
				case DatabaseTypeEnum.Unknown:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal Type GetDatabaseConfigType()
		{
			return GetDatabaseConfigType(DatabaseType);
		}
	}
}