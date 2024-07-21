using Newtonsoft.Json;
using Norman.Log.Model;
using Norman.Log.Server.Input;

namespace Norman.Log.Logger.gRpc
{
	public static class Log4GrpcExtension
	{
		/// <summary>
		///     将业务模型转换为grpc网络传输模型.
		/// </summary>
		/// <param name="log"></param>
		/// <returns></returns>
		public static ReportLogByGrpcRequest ToReportLogByGrpcRequest(this Model.Log log)
		{
			return new ReportLogByGrpcRequest
			{
				CreateTime = (long)(log.CreateTime - Constant.GreenwichTime1970).TotalMilliseconds,
				Id = log.Id.ToString(),
				Detail = log.Detail,
				Layer = (int)log.Layer.Value,
				Type = (int)log.Type.Value, Module = log.Module, Summary = log.Summary, LoggerName = log.LoggerName,
				LogContext = log.LogContext.ToGrpcLogContext()
			};
		}

		private static LogContext ToGrpcLogContext(this Model.Log.Context context)
		{
			#region 值有效性验证

			if (context == null) return new LogContext();

			#endregion

			

			#region 构建和返回结果

			var result = new LogContext
			{
				Role = context.Role == null ? "" : JsonConvert.SerializeObject(context.Role),
				Client = context.Client == null ? "" : JsonConvert.SerializeObject(context.Client),
				User = context.User == null ? "" : JsonConvert.SerializeObject(context.User),
				Request = context.Request == null ? "" : JsonConvert.SerializeObject(context.Request),
				Response = context.Response == null ? "" : JsonConvert.SerializeObject(context.Response),
				Site = context.Site == null ? "" : JsonConvert.SerializeObject(context.Site),
			};

			#endregion
			
			#region 转换 others部分

			foreach (var other in context.Others)
			{
				if (other == null)
				{
					result.Others.Add("");
					continue;
				}

				var s = JsonConvert.SerializeObject(other);
				result.Others.Add(s);
			}

			#endregion

			return result;
		}
	}
}