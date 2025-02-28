using Newtonsoft.Json;

namespace Norman.Log.Config
{
	[JsonConverter(typeof(IDatabaseConfigConfigJsonConverter))]
	public interface IDatabaseConfig
	{
		DatabaseTypeEnum DatabaseType { get; }

		string ServerVersion { get; set; }

		string ToConnectionString();
	}
}