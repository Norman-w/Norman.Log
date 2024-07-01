using System.ServiceModel;
using Norman.Log.Server.Input;
using ProtoBuf.Grpc;

namespace Norman.Log.Server.CommonFacade.GRPC;
[ServiceContract(Name = "Norman.Log.Server.Input.ReportLogService")]
public interface IReportLogService
{
	/// <summary>
	/// 通过gRPC上报日志
	/// </summary>
	/// <param name="request"></param>
	/// <param name="context"></param>
	/// <returns></returns>
	[OperationContract]
	Task<ReportLogByGrpcResponse> ReportLogByGrpc(ReportLogByGrpcRequest request, CallContext context = default);
}