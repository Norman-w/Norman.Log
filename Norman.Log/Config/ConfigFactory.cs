using System.IO;
using Newtonsoft.Json;

namespace Norman.Log.Config
{
	public static class ConfigFactory
	{
		public static T CreateFromJson<T>(string json) where T : ICommonConfig<T>
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static T CreateFromFile<T>(string path, bool tryCreateIfNotExist) where T : ICommonConfig<T>, new()
		{
			if (!File.Exists(path))
			{
				if (tryCreateIfNotExist)
				{
					var defaultConfigJson = JsonConvert.SerializeObject(new T().GetDefault(), Formatting.Indented);
					File.WriteAllText(path, defaultConfigJson);
				}
				else
				{
					throw new FileNotFoundException("配置文件不存在", path);
				}
			}

			var json = File.ReadAllText(path);
			if (string.IsNullOrWhiteSpace(json))
			{
				if (tryCreateIfNotExist)
				{
					var defaultConfigJson = JsonConvert.SerializeObject(new T().GetDefault(), Formatting.Indented);
					File.WriteAllText(path, defaultConfigJson);
					json = defaultConfigJson;
				}
				else
				{
					throw new InvalidDataException($"配置文件为空: {path}");
				}
			}
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}