using System;

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
}