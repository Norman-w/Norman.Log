using System.Runtime.Serialization;

namespace Norman.Log.Server.CommonFacade.GRPC.Model;

[DataContract]
public class LogContext
{
	[DataMember(Order = 1)]
	public string? Role { get; set; }
	/// <summary>
	/// Site/Portal/Platform
	/// </summary>
	[DataMember(Order = 2)]
	public string? Site { get; set; }
	[DataMember(Order = 3)]
	public string? User { get; set; }
	[DataMember(Order = 4)]
	public string? Client { get; set; }
	[DataMember(Order = 5)]
	public string? Request { get; set; }
	[DataMember(Order = 6)]
	public string? Response { get; set; }
	/// <summary>
	/// 其他的上下文信息
	/// </summary>
	[DataMember(Order = 7)]
	public List<string>? Others { get; set; }
}