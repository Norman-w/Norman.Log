/*

 日志记录器的主入口.

 使用时,using Norman.Log.Logger
 然后调用Logger.Write方法记录日志.
 其他的一些Logger.xxx的方法都是语法糖,最终都会调用Logger.Write方法,生成Log对象并记录.

 记录的方式主要有三种
 记录到文件
 推送到日志服务器
 记录到数据库

 如果是轻量dll的模式更常用的话,Logger类应当主要将日志推送到服务器,由日志服务器处理缓存池,推送到文件或数据库,分发到客户端等.
 如果是多功能dll的模式更常用的话,Logger可以直接写文件,写数据库,并且可以推送到日志服务器,由日志服务器做进一步处理(比如推送到客户端,缓存池,分发到文件等,定期写入数据库等)

 TODO,或许会把能用到Logger又能用到Server中的部分(比如磁盘监测,写数据库组件,写文件组件等)提取出来,
 放到sln的Component文件夹内的csproj中.
 这样就能灵活的给Logger和Server分工,更方便的组合各种应用场景.


*/

using System;
using Norman.Log.Config;
using Norman.Log.Model;

namespace Norman.Log.Logger
{
	/// <summary>
	/// 命名的Logger,用于区分不同的日志记录器
	/// </summary>
	public class NamedLogger : IDisposable, IReporter
	{
		public string Name { get; }

		public NamedLogger(string name)
		{
			Name = name;
		}

		/// <summary>
		/// 记录/写日志,传入Log对象
		/// </summary>
		/// <param name="log"></param>
		public virtual void Write(Model.Log log)
		{
			log.LoggerName = Name;
			if (AppConfig.LoggerConfig.LogToFile?.OnOff == true)
				App.LogFileWriter.AddLogToWaitingToWriteQueue(log);
			if (AppConfig.LoggerConfig.LogToDatabase?.OnOff == true)
				App.LogDatabaseWriter.AddLogToWaitingToWriteQueue(log);
			// //TODO 添加推送到日志服务器等的代码
			// if(AppConfig.LoggerConfig.LogToDatabase?.OnOff == true)
			//     App.LogDatabaseWriter.AddLogToWaitingToWriteQueue(log);
		}

		/// <summary>
		/// 记录/写日志,传入参数,生成Log对象,最终会调用Write(Log log)方法
		/// </summary>
		/// <param name="logType"></param>
		/// <param name="logLayer"></param>
		/// <param name="moduleName"></param>
		/// <param name="summary"></param>
		/// <param name="detail"></param>
		/// <param name="context"></param>
		public void Write(LogType logType, LogLayer logLayer, string moduleName, string summary, string detail,
			Model.Log.Context context = null)
		{
			var log = new Model.Log
			{
				Type = logType,
				Layer = logLayer,
				Summary = summary,
				Detail = detail,
				Module = moduleName,
				LoggerName = Name,
				LogContext = context
			};
			Write(log);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public event LogReceivedEventHandler LogReceived;
		public event ReporterSessionCreatedEventHandler ReporterSessionCreated;
	}
}