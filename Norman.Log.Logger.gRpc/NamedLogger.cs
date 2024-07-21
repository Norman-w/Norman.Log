/*
 使用grpc传输数据的命名日志记录器
 */

using System;
using System.Net.Http;
using Grpc.Net.Client;
using Norman.Log.Server.Input;
using BaseNamedLogger = Norman.Log.Logger.NamedLogger;

namespace Norman.Log.Logger.gRpc
{
	public class NamedLogger : BaseNamedLogger
	{
		/// <summary>
		///     gRPC客户端
		/// </summary>
		private readonly ReportLogService.ReportLogServiceClient _client;

		/// <summary>
		///     创建一个新的命名日志记录器
		///     名字是在记录的时候用于区分哪个记录器记录的
		///     grpcServerAddress是远程服务器的地址.注意http和https.
		///     TODO 当前默认使用不安全的证书，后续需要修改为安全证书
		/// </summary>
		/// <param name="name"></param>
		/// <param name="grpcServerAddress"></param>
		public NamedLogger(string name, string grpcServerAddress) : base(name)
		{
			var grpcChannelOptions = new GrpcChannelOptions
			{
				//insecure,不验证服务端证书
				HttpHandler = new HttpClientHandler
				{
					ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
				}
			};

			var channel = GrpcChannel.ForAddress(grpcServerAddress, grpcChannelOptions);
			_client = new ReportLogService.ReportLogServiceClient(channel);
		}

		public override void Write(Model.Log log)
		{
			base.Write(log);
			//在父类完成了日志记录后，再将日志通过grpc传输到远程服务器
			var request = log.ToReportLogByGrpcRequest();

			// 异步调用 ReportLogByGrpc 方法
			try
			{
				var response = _client.ReportLogByGrpcAsync(request).GetAwaiter().GetResult();
				if (!response.Success)
				{
					Console.WriteLine($"通过 gRPC 发送日志接收到服务端失败响应,Response:{response}");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"通过 gRPC 发送日志成功,Response:{response}");
					Console.ResetColor();
				}
			}
			catch (Exception error)
			{
				Console.WriteLine($"通过 gRPC 发送日志失败:{error}");
			}
		}
	}
}