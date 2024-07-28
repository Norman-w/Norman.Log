/*
 
 
 任何继承此接口的类都可以作为日志报送器使用



*/

namespace Norman.Log
{
	/// <summary>
	/// 当日志从报送器那边接收到时触发
	/// </summary>
	public delegate void LogReceivedEventHandler(object sender, string loggerName, Model.Log log);
	/// <summary>
	/// 当报送器会话创建时触发
	/// </summary>
	public delegate void ReporterSessionCreatedEventHandler(Model.Log log);
	public interface IReporter
	{
		/// <summary>
		/// 写入/记录日志
		/// </summary>
		/// <param name="log"></param>
		void Write(Model.Log log);
		/// <summary>
		/// 当日志从报送器那边接收到时触发
		/// </summary>
		event LogReceivedEventHandler LogReceived;
		/// <summary>
		/// 当报送器会话创建时触发
		/// </summary>
		event ReporterSessionCreatedEventHandler ReporterSessionCreated;
	}
}