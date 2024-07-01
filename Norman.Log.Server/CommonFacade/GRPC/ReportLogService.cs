using Norman.Log.Server.Input;
using ProtoBuf.Grpc;

namespace Norman.Log.Server.CommonFacade.GRPC;

public class ReportLogService : IReportLogService
{
	public Task<ReportLogByGrpcResponse> ReportLogByGrpc(ReportLogByGrpcRequest request, CallContext context = default)
	{
		throw new NotImplementedException();
	}
}