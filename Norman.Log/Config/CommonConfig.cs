using System.IO;
using Newtonsoft.Json;

namespace Norman.Log.Config
{
	public abstract partial class CommonConfig
	{
		/// <summary>
		/// 记录到文件的设置
		/// </summary>
		public LogToFileConfig LogToFile { get; set; }

		/// <summary>
		/// 记录到数据库的设置
		/// </summary>
		public LogToDatabaseConfig LogToDatabase { get; set; }
	}

	public abstract partial class CommonConfig
	{
		#region 构造,初始,填充和工厂方法

		/// <summary>
		/// 从json字符串中创建一个LoggerConfig
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		protected static T FromJson<T>(string json) where T : CommonConfig
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		/// <summary>
		/// 从文件中加载LoggerConfig
		/// </summary>
		/// <param name="path"></param>
		/// <param name="tryCreateIfNotExist"></param>
		/// <param name="defaultConfig"></param>
		protected void PopulateByFile<T>(string path, bool tryCreateIfNotExist = false,
			ICommonConfig<T> defaultConfig = null)
		{
			if (!File.Exists(path))
			{
				if (tryCreateIfNotExist && defaultConfig != null)
				{
					var defaultConfigJson = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
					File.WriteAllText(path, defaultConfigJson);
				}
				else
				{
					throw new FileNotFoundException("配置文件不存在", path);
				}
			}

			var json = File.ReadAllText(path);
			JsonConvert.PopulateObject(json, this);
		}

		public void Populate(string json)
		{
			JsonConvert.PopulateObject(json, this);
		}

		#endregion
	}
}