/*


 日志记录器设置.
 日志记录器是本日志系统的三大组成部分之一,也是最重要的部分.
 主要负责直接调用Logger的代码进行日志记录.具体的Write/Log到什么地方要看这个配置.
 什么样的规则等信息也都需要参照这个配置.



*/

using System;
using System.IO;
using Newtonsoft.Json;

namespace Norman.Log.Config
{
	/// <summary>
	/// 记录到文件的设置
	/// </summary>
	public class LogToFileConfig
	{
		/// <summary>
		/// 是否开启记录到文件
		/// </summary>
		public bool OnOff { get; set; }

		/// <summary>
		/// 日志文件的根目录
		/// </summary>
		public string RootPath { get; set; }

		/// <summary>
		/// 当日志记录器发生错误的时候记录到什么位置(不是正常需要Logger记录的东西,而是Logger本身要记录的)
		/// </summary>
		/// <returns></returns>
		public string ErrorFileName { get; set; }

		//好像可以通过失败后重写文件的方式来解决,所以这个配置项暂时不需要
		// /// <summary>
		// /// 独占进程的标记位,用于判断是否有其他进程在写入日志,这个文件中将会存放独占者写入的一些基本信息,用于排查谁在独占
		// /// </summary>
		// public string InstanceFileName { get; set; }

		public enum CreateFolderRuleEnum
		{
			/// <summary>
			/// 年/月/日方式,文件中的日期就应该是 "小时"的方式
			/// </summary>
			YearFolderMonthFolderDayFolder = 1,

			/// <summary>
			/// 年/月日方式,文件中的日期就应该是 "小时"的方式
			/// </summary>
			YearFolderMonthDayFolder = 2,

			/// <summary>
			/// 年月/日方式,文件中的日期就应该是 "小时"的方式
			/// </summary>
			YearMonthFolderDayFolder = 3,

			/// <summary>
			/// 年月日方式,文件中的日期就应该是 "小时"的方式
			/// </summary>
			YearMonthDayFolder = 4,

			/// <summary>
			/// 年/月方式,文件中的日期就应该是 "天+小时"的方式
			/// </summary>
			YearFolderMonthFolder = 5,

			/// <summary>
			/// 年/月日方式,文件中的日期就应该是 "小时"的方式
			/// </summary>
			YearFolderMonthDay = 6,
		}

		public enum CreateFileNameRuleEnum
		{
			/*
			 按照日志记录器名称+文件夹所没能表述的下一级日期来表述,比如文件夹是04月的,那么按照01号的日志记录器来表述: SessionLogger-2020-04-01.log
			 按照日志记录器名称表述 比如: SessionLogger.log
			 按照文件夹没能表述的下一级日期来表述 比如: 2020-04-01.log (所有的日志记录器都写入这个文件)
			 按照日期和日志记录器名称来表述 比如: 2020-04-01-SessionLogger.log
            */
			LoggerNameAndTime = 1,
			LoggerName = 2,
			Time = 3,
			TimeAndLoggerName = 4,
		}

		/// <summary>
		/// 创建文件夹的规则,比如按照年月日创建文件夹,按照年月创建文件夹等等
		/// </summary>
		public CreateFolderRuleEnum CreateFolderRule { get; set; }

		/// <summary>
		/// 创建文件的规则,比如按照日志记录器名称+文件夹所没能表述的下一级日期来表述,比如文件夹是04月的,那么按照01号的日志记录器来表述: SessionLogger-2020-04-01.log
		/// </summary>
		public CreateFileNameRuleEnum CreateFileNameRule { get; set; }

		// /// <summary>
		// /// 创建文件的方式,比如按照日志的类型分,按照日志的等级分,不按照任何等级或模块分只按照日期,按照模块分,按照模块分等等
		// /// 实际上这些都是没有什么意义的,因为写入到文件的话本身可阅读性就不好.所以不如简单点就是按照时间分.
		// /// 用日志查看器来筛选日志才是硬道理,那样的话就可以定制很多筛选条件或者订阅条件了.
		// /// 所以这个方案放弃了,就按照日期节点创建文件夹,然后下面按照文件名规则创建文件
		// /// </summary>
		// public enum CreateFileRuleEnum
		// {
		// 	
		// }

		/// <summary>
		/// 文件的最大大小,超过这个大小就会新建一个文件
		/// </summary>
		public uint MaxSizeKbPerFile { get; set; }

		/// <summary>
		/// 磁盘空间剩余多少Kb的时候发出一般提示
		/// </summary>
		/// <returns></returns>
		public double RemainKbTriggerNotify { get; set; }

		/// <summary>
		/// 磁盘空间剩余多少Kb的时候发出一般警告
		/// </summary>
		/// <returns></returns>
		public double RemainKbTriggerWarn { get; set; }

		/// <summary>
		/// 磁盘空间剩余多少Kb的时候发出严重警告(因为有时候并不是日志文件导致的磁盘空间变小,所以删了日志文件也不一定能解决问题)
		/// </summary>
		public double RemainKbTriggerDanger { get; set; }

		/// <summary>
		/// 磁盘空间剩余多少Kb的时候开启循环覆盖(删除最早的日志)
		/// </summary>
		/// <returns></returns>
		public double RemainKbDelete { get; set; }

		/// <summary>
		/// 当缓存中的日志条数大于等于多少条的时候,即使没到时间也写入文件
		/// </summary>
		/// <returns></returns>
		public uint MaxLogCountInCache { get; set; }

		/// <summary>
		/// 不管日志缓存中的日志条数是多少,只要距离上次写入文件的时间超过这个时间就写入文件
		/// 每次写入日志后,计时器会重新计时,到了这个时间就写入文件
		/// </summary>
		public TimeSpan WriteToFileInterval { get; set; }

		public static LogToFileConfig Default { get; } = new LogToFileConfig
		{
			OnOff = true,
			RootPath = "Logs",
			ErrorFileName = "Error.log",
			CreateFolderRule = LogToFileConfig.CreateFolderRuleEnum.YearFolderMonthFolderDayFolder,
			CreateFileNameRule = LogToFileConfig.CreateFileNameRuleEnum.LoggerNameAndTime,
			MaxSizeKbPerFile = 1024 * 10, //10Mb,太大了打开文件会很慢,太小了文件会很多
			RemainKbTriggerNotify = 1024 * 1024,
			RemainKbTriggerWarn = 1024 * 1024 * 2,
			RemainKbTriggerDanger = 1024 * 1024 * 4,
			RemainKbDelete = 1024 * 1024 * 8,
			MaxLogCountInCache = 1000, //太大了会浪费内存,太小了会频繁写入文件
			WriteToFileInterval = TimeSpan.FromMinutes(5), //延迟一点对人来说没啥感觉,对程序来说可以减少写入文件的次数
		};
	}

	/// <summary>
	/// 记录到数据库的设置
	/// </summary>
	public class LogToDatabaseConfig
	{
		public bool OnOff { get; set; }
		public string ConnectionString { get; set; }
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
			ConnectionString = "Server=localhost:",
			TableName = "Logs",
			MaxLogCountInCache = 100,
			WriteToDatabaseInterval = TimeSpan.FromSeconds(5),
			MaxFlushCountPerTime = 1000,
		};
	}

	/// <summary>
	/// 记录到日志服务器的设置
	/// </summary>
	public class LogToLogServerConfig
	{
		public bool OnOff { get; set; }

		public enum SendToLogServerProtocolEnum
		{
			WebSocket = 1,
			Grpc = 2,
			Http = 3,
		}

		public SendToLogServerProtocolEnum Protocol { get; set; }

		public class LogServerWebSocketSetting
		{
			public string Url { get; set; }
		}

		public class LogServerGrpcSetting
		{
			public string Url { get; set; }
		}

		public class LogServerHttpSetting
		{
			public string Url { get; set; }
		}

		public LogServerWebSocketSetting WebSocket { get; set; }
		public LogServerGrpcSetting Grpc { get; set; }
		public LogServerHttpSetting Http { get; set; }
		
		public static LogToLogServerConfig Default { get; } = new LogToLogServerConfig
		{
			OnOff = false,
			Protocol = LogToLogServerConfig.SendToLogServerProtocolEnum.WebSocket,
			WebSocket = new LogToLogServerConfig.LogServerWebSocketSetting
			{
				Url = "ws://localhost:5000/ws",
			},
			Grpc = new LogToLogServerConfig.LogServerGrpcSetting
			{
				Url = "localhost:5000",
			},
			Http = new LogToLogServerConfig.LogServerHttpSetting
			{
				Url = "http://localhost:5000",
			},
		};
	}

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
					var defaultConfigJson = JsonConvert.SerializeObject(new T().Default, Formatting.Indented);
					File.WriteAllText(path, defaultConfigJson);
				}
				else
				{
					throw new FileNotFoundException("配置文件不存在", path);
				}
			}

			var json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<T>(json);
		}
		
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
		protected void PopulateByFile<T>(string path, bool tryCreateIfNotExist = false, ICommonConfig<T> defaultConfig = null)
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

	public interface ICommonConfig<T>
	{
		T Default { get; }
		void Populate(string json);
		void FromFile(string path, bool tryCreateIfNotExist = true);
	}

	/// <summary>
	/// 日志记录器设置
	/// </summary>
	public class LoggerConfig : CommonConfig, ICommonConfig<LoggerConfig>
	{
		public LoggerConfig Default { get; } = new LoggerConfig
		{
			LogToFile = LogToFileConfig.Default,
			LogToDatabase = LogToDatabaseConfig.Default,
			LogToServer = LogToLogServerConfig.Default,
		};

		public void FromFile(string path, bool tryCreateIfNotExist = true)
		{
			PopulateByFile(path, tryCreateIfNotExist, Default);
		}

		/// <summary>
		/// 记录到日志服务器的设置
		/// </summary>
		public LogToLogServerConfig LogToServer { get; set; }
	}

	/// <summary>
	/// 日志服务器的设置,日志服务器的设置不包含记录到日志服务器的设置.因为他本身就是一个日志服务器,不需要记录到其他服务器,只需要记录到文件或者数据库
	/// </summary>
	public class LogServerConfig : CommonConfig, ICommonConfig<LogServerConfig>
	{
		public LogServerConfig Default { get; } = new LogServerConfig
		{
			LogToFile = LogToFileConfig.Default,
			LogToDatabase = LogToDatabaseConfig.Default,
		};
		public void FromFile(string path, bool tryCreateIfNotExist = true)
		{
			PopulateByFile(path, tryCreateIfNotExist, Default);
		}
	}
}