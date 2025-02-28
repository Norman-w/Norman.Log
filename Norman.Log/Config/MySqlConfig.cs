namespace Norman.Log.Config
{
	public class MySqlConfig : NormalDatabaseConfig
	{
		public override DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Mysql;
	}
}