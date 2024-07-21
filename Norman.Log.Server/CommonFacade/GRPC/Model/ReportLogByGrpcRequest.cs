/*
 通过GRPC上报日志的请求
 数据格式都使用简单类型.
*/


using System.Runtime.Serialization;
using Norman.Log.Server.CommonFacade.GRPC.Model;

namespace Norman.Log.Server.Input;

[DataContract]
public class ReportLogByGrpcRequest
{
	/// <summary>
	/// 日志Id
	/// </summary>
	[DataMember(Order = 1)]
	public string Id { get; set; }
	/// <summary>
	/// 创建时间,使1970-01-01 00:00:00到现在的毫秒数(格林尼治时间)
	/// </summary>
	[DataMember(Order = 2)]
	public long CreateTime { get; set; }
	
	/// <summary>
	/// 日志记录器的名称,比如 AuthLogger, SessionLogger, RequestLogger等,默认的是DefaultLogger
	///  </summary>
	[DataMember(Order = 3)]
	public string LoggerName { get; set; }
	
	/// <summary>
	/// 日志类型,比如:错误,警告,信息等
	/// </summary>
	[DataMember(Order = 4)]
	public int Type { get; set; }
	
	/// <summary>
	/// 日志发生在哪一层,比如系统层,外设层,业务层等
	/// </summary>
	[DataMember(Order = 5)]
	public int Layer { get; set; }
	
	/// <summary>
	/// 日志发生在哪个模块,比如xxxController, xxxManager, xxxHandler等
	/// </summary>
	[DataMember(Order = 6)]
	public string Module { get; set; }
	
	/// <summary>
	/// 日志发生时携带的描述消息,最好是一句话可以看明白的.
	/// </summary>
	[DataMember(Order = 7)]
	public string Summary { get; set; }
	
	/// <summary>
	/// 如果一句话描述不清楚,可以在这里详细描述
	/// </summary>
	[DataMember(Order = 8)]
	public string Detail { get; set; }
	
	/// <summary>
	/// 日志的上下文信息
	/// 比如 角色(类型),站点,用户(name, id, session),客户端信息,请求信息,响应信息等
	/// </summary>
	[DataMember(Order = 9)]
	public LogContext LogContext { get; set; }
}