using System;
using System.IO;
using Newtonsoft.Json;

namespace Norman.Log.Config
{
	[Obsolete("Use CommonConfig instead")]
	public static class ConfigFactory
	{
		public static T CreateFromJson<T>(string json) where T : ICommonConfig<T>
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		// public static T CreateFromFile<T>(string path, bool tryCreateIfNotExist) where T : ICommonConfig<T>, new()
		// {
		// 	return
		// }
	}
}