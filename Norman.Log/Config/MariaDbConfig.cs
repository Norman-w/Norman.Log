namespace Norman.Log.Config
{
	public class MariaDbConfig : NormalDatabaseConfig
	{
		public override DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.MariaDb;
	}
}