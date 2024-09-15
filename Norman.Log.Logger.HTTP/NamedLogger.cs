using System;
using System.Net;

namespace Norman.Log.Logger.HTTP
{
	/// <summary>
	/// 命名的Logger,用于区分不同的日志记录器
	/// </summary>
	public class NamedLogger : IDisposable
	{
		public string Name { get; }
		
		private readonly string _reportLogUrl;

		public NamedLogger(string name, string reportLogUrl)
		{
			Name = name;
			_reportLogUrl = reportLogUrl;
		}

		/// <summary>
		/// 记录/写日志,传入Log对象
		/// </summary>
		/// <param name="log"></param>
		public virtual void Write(Log log)
		{
			var logRecord4Net = log.ToLogRecord4Net();
			var client = new WebClient();
			client.Headers.Add("Content-Type", "application/json");
			var json = logRecord4Net.ToJson();
			var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
			client.UploadData(_reportLogUrl, "POST", jsonBytes);
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
		public void Write(LogTypeEnum logType, LogLayerEnum logLayer, string moduleName, string summary, string detail,
			Log.Context context = null)
		{
			var log = new Log(Name)
			{
				Type = logType,
				Layer = logLayer,
				Summary = summary,
				Detail = detail,
				Module = moduleName,
				LogContext = context
			};
			Write(log);
		}

		public void Dispose()
		{
		}
	}
}