using System.Runtime.Serialization;

namespace Norman.Log.Server.Input;

[DataContract]
public class ReportLogByGrpcResponse
{
	/// <summary>
	/// 是否成功
	/// </summary>
	[DataMember(Order = 1)]
	public bool Success { get; set; }
}