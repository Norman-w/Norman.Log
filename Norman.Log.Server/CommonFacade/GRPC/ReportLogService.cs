using Newtonsoft.Json;
using Norman.Log.Model;
using Norman.Log.Server.CommonFacade.GRPC.Model;
using Norman.Log.Server.Core;
using Norman.Log.Server.Input;
using ProtoBuf.Grpc;

namespace Norman.Log.Server.CommonFacade.GRPC;

public class ReportLogService : IReportLogService
{
	private static class Log4GrpcExtension
	{
		/// <summary>
		/// 将grpc请求转换为日志业务模型
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static Log.Model.Log ToLog(ReportLogByGrpcRequest request)
		{
			var createTime = Constant.GreenwichTime1970.AddMilliseconds(request.CreateTime);
			var logType = LogType.FromValue((uint)request.Type);
			var logLayer = LogLayer.FromValue((uint)request.Layer);
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			var logContext = request.LogContext == null ? null : ToContext(request.LogContext);
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			var id = request.Id == null? Guid.NewGuid(): Guid.Parse(request.Id);
			
			return new Log.Model.Log
			{
				CreateTime = createTime, Id = id, Summary = request.Summary,
				Detail = request.Detail, Type = logType, Layer = logLayer, Module = request.Module, LoggerName = request.LoggerName, LogContext = logContext
			};
		}

		private static Log.Model.Log.Context? ToContext(LogContext grpcLogContext)
		{
			//确实有可能是空的
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if(grpcLogContext == null) return null;
			var logContextBll = new Log.Model.Log.Context();
			if (grpcLogContext.Role != null)
			{
				logContextBll.Role = JsonConvert.DeserializeObject(grpcLogContext.Role);
			}
			if (grpcLogContext.Site != null)
			{
				logContextBll.Site = JsonConvert.DeserializeObject(grpcLogContext.Site);
			}
			if (grpcLogContext.User != null)
			{
				logContextBll.User = JsonConvert.DeserializeObject(grpcLogContext.User);
			}
			if (grpcLogContext.Client != null)
			{
				logContextBll.Client = JsonConvert.DeserializeObject(grpcLogContext.Client);
			}
			if (grpcLogContext.Request != null)
			{
				logContextBll.Request = JsonConvert.DeserializeObject(grpcLogContext.Request);
			}
			if (grpcLogContext.Response != null)
			{
				logContextBll.Response = JsonConvert.DeserializeObject(grpcLogContext.Response);
			}

			if (grpcLogContext.Others == null) return logContextBll;
			logContextBll.Others = new List<object>();
			foreach (var otherArg in grpcLogContext.Others.Where(otherArg => string.IsNullOrEmpty(otherArg) == false))
			{
				logContextBll.Others.Add(JsonConvert.DeserializeObject(otherArg));
			}
			return logContextBll;
		}
	}
	public Task<ReportLogByGrpcResponse> ReportLogByGrpc(ReportLogByGrpcRequest request, CallContext context = default)
	{
		Console.WriteLine($"收到来自{context.ServerCallContext}的日志: {request}");
		var log = Log4GrpcExtension.ToLog(request);
		//TODO: 通过grpc上报日志时,根据CallContext的ServerCallContext信息来获取Reporter Client
		var mockupGrpcReporter = new ReporterClient(new SessionCreatedEventArgs("mockup grpc reporter client", new object()));
		App.Server.HandleLog(mockupGrpcReporter, log);
		return Task.FromResult(new ReportLogByGrpcResponse
		{
			Success = true
		});
	}
}