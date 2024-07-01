/*

 网络入口,日志的进入可以从这里来
 也可以客户端或者监视器之类的连接上以后,通过这里来获取日志
 或者服务端调用客户端推送日志.
 
 所有经由网络的请求都从这里出去或者进来,然后交给对应的组件处理.

*/


using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Norman.Log.Server.CommonFacade.GRPC;
using ProtoBuf.Grpc.Reflection;
using ProtoBuf.Grpc.Server;

namespace Norman.Log.Server.CommonFacade;

/// <summary>
/// 网络相关请求的入口.
/// 目前包括http,grpc,websocket.
/// 比如控制面板使用http接口,grpc接口用于日志的上报,websocket用于日志的推送.
/// 或者以后可能使用gRpc的服务端远程调用客户端的函数进行日志的推送.
/// </summary>
public class Net
{
	private readonly BackgroundWorker _worker = new();

	public Net()
	{
		_worker.DoWork += WorkerOnDoWork;
	}

	/// <summary>
	/// 停止
	/// </summary>
	public void Stop()
	{
		_worker.CancelAsync();
	}

	/// <summary>
	/// 启动
	/// </summary>
	public void Start()
	{
		_worker.RunWorkerAsync();
	}

	private void WorkerOnDoWork(object? sender, DoWorkEventArgs e)
	{
		var builder = InitWebApplicationBuilder();

		using var app = InitWebApplication(builder);

		ConfigGrpc(app);

		ConfigSwagger(app);

		ConfigWebsocket(app);

		GenerateGrpcProtoFile(app);

		#region 启动服务器

		Console.WriteLine("应用程序启动成功,请使用gRPC客户端访问gRPC服务.");
		//这里添加正常的其他逻辑.因为本项目是一个控制台应用程序,其他的业务方面的服务应当先启动,然后再启动grpc服务
		app.Run();

		#endregion
	}

	private static void ConfigGrpc(WebApplication app)
	{
		#region 使用app依次Map服务,包括 http get, grpc, grpc web等

		app.MapGet("/",
			() =>
				"使用gRPC客户端访问gRPC服务,不能使用get请求. 看到此文本,说明gRPC服务已经启动成功. 请使用gRPC客户端访问gRPC服务.");


		app.MapGrpcService<ReportLogService>()
			.EnableGrpcWeb()
			.RequireCors("AllowAll");

		//可以继续map其他的service,并可以对不同的service进行 grpc web, 跨域, 以及其他的配置

		#endregion
	}

	private static void GenerateGrpcProtoFile(WebApplication app)
	{
		#region 使用SchemaGenerator生成proto文件(如果是开发环境)

//然后判断一下是否是开发环境,如果是开发环境,则执行下面的代码,
//IsDevelopment()方法来自于using Microsoft.Extensions.Hosting;
//此处的代码是用来反序列化的,用来通过C#代码优先的cs文件生成proto文件
// if (app.Environment.IsDevelopment())
//或者 也可以检查有没有调试器附加到进程上,如果有,则执行下面的代码
		if (System.Diagnostics.Debugger.IsAttached)
		{
			//执行app的MapCodeFirstGrpcReflectionService方法,这个方法是用来反序列化的,是用来通过C#代码优先的cs文件生成proto文件的
			app.MapCodeFirstGrpcReflectionService();

			//尝试生成一次schema,这个schema是proto文件的内容,然后使用Console.WriteLine输出到控制台
			//功能来自 using ProtoBuf.Grpc.Reflection;
			var generator = new SchemaGenerator();
			var schema = generator.GetSchema<IReportLogService>();
			AutoGenerateProtoFile(schema, nameof(IReportLogService));
			Console.WriteLine("以下是通过C#代码优先的cs文件生成的proto文件的内容:");
			Console.WriteLine(schema);
		}

		#endregion
	}

	private static void ConfigSwagger(WebApplication app)
	{
		#region swagger

		app.UseSwagger();

		app.UseSwaggerUI();

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();

		#endregion
	}

	private static void ConfigWebsocket(WebApplication app)
	{
		#region 添加websocket支持

		app.UseWebSockets(
			//支持SubProtocol
			new WebSocketOptions
			{
				KeepAliveInterval = TimeSpan.FromSeconds(120),
			});
		// 添加WebSocket中间件
		app.Map("ws", async (context) =>
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				//如果访问位置是CampLauncher等支持的,则添加SubProtocol
				if (context.Request.Path == "/ws/")
				{
					// var subProtocol = context.Request.Headers["Sec-WebSocket-Protocol"];
					// if (subProtocol.Count > 0)
					// {
					//     context.WebSockets.WebSocketRequestedProtocols.Add(subProtocol);
					// }
				}

				//如果是有效的WebSocket请求,则接受WebSocket
				if (context.WebSockets.WebSocketRequestedProtocols.Count == 0)
				{
					context.Response.ContentType = "text/plain; charset=utf-8";
					Console.WriteLine("无效的WebSocket请求,没有SubProtocol");
					await context.Response.WriteAsync("无效的WebSocket请求");
					// context.Response.StatusCode = 400;
					return;
				}
				//列出所有的SubProtocol
				// foreach (var subProtocol in context.WebSockets.WebSocketRequestedProtocols)
				// {
				//     Console.WriteLine("SubProtocol:" + subProtocol);
				// }

				try
				{
					var webSocket =
						await context.WebSockets.AcceptWebSocketAsync(context.WebSockets
							.WebSocketRequestedProtocols[0]);
					// await LauncherServerWebSocketServer.Instance.OnClientConnected(webSocket);
					//TODO: 这里是处理WebSocket的代码,可以在这里处理WebSocket的连接
				}
				catch (Exception err)
				{
					Console.WriteLine("OnClientConnected异常:" + err.Message);
					// throw;
				}
			}
			else
			{
				//设置字符集,防止乱码
				context.Response.ContentType = "text/plain; charset=utf-8";
				//输出提示,只能使用ws访问
				await context.Response.WriteAsync("只能使用ws访问, Only ws is supported");
				// context.Response.StatusCode = 400;
			}
		});

		#endregion
	}

	private static WebApplication InitWebApplication(WebApplicationBuilder builder)
	{
		WebApplication? app = null;
		try
		{
			#region 使用builder创建和配置app

			app = builder.Build();
			app.UseGrpcWeb();
//还需要支持跨域请求
			app.UseCors();

			#endregion

			return app;
		}
		catch
		{
			((IDisposable?)app)?.Dispose();
			throw;
		}
	}

	private static WebApplicationBuilder InitWebApplicationBuilder()
	{
		#region 创建和配置Builder

		#region 创建Builder

		//创建WebApplication的Builder,  WebApplication 是 using Microsoft.AspNetCore.Builder;
		var builder = WebApplication.CreateBuilder();

		#region Api controller和swagger

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		#endregion

		#endregion

		#region 添加下面的UseKestrel(微软的一种web服务引擎)需要用到using Microsoft.AspNetCore.Hosting;

		builder.WebHost.UseKestrel(options =>
		{
			options.ListenAnyIP(App.Setting.GrpcPort, listenOptions =>
			{
				//如果不使用grpc web,可以只使用http2
				listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
			});
		});

		#endregion

		#region 为grpc web添加跨域支持

		builder.Services.AddCors(o => o.AddPolicy("AllowAll", corsPolicyBuilder =>
		{
			corsPolicyBuilder.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
		}));

		#endregion

		#region 添加两个服务,一个是grpc的正常的服务,一个是反序列化服务,是用来通过C#代码优先的cs文件生成proto文件的

// //AddGrpc来自using Microsoft.Extensions.DependencyInjection; 注意如果是直接用Rider创建的gRPC项目,则是AddCodeFirstGrpc
// builder.Services.AddGrpc();
//
// //要使用protobuf-net.Grpc.AspNetCore,才可以使用AddCodeFirstGrpc();
		builder.Services.AddCodeFirstGrpc();
// //AddCodeFirstGrpcReflection来自using ProtoBuf.Grpc.Server; 这个和Rider创建的gRPC项目是一样的
		builder.Services.AddCodeFirstGrpcReflection();

		#endregion

		#endregion

		return builder;
	}

	/// <summary>
	/// 自动生成proto文件,如果是开发环境,则生成proto文件
	/// </summary>
	/// <param name="schema"></param>
	/// <param name="fileName"></param>
	/// <exception cref="Exception"></exception>
	private static void AutoGenerateProtoFile(string schema, string fileName)
	{
		//获取当前运行文件夹,然后../../../三级可以获取到csproj文件所在的文件夹
		var currentDirectory = Directory.GetCurrentDirectory();
		if (currentDirectory == null)
		{
			throw new Exception("获取当前运行文件夹失败");
		}

		var projectDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent;
		if (projectDirectory == null)
		{
			throw new Exception("获取项目文件夹失败");
		}

		//判断proto文件夹是否存在
		var protoDirectory = Path.Combine(projectDirectory.FullName, "CommonFacade", "GRPC", "Proto");
		if (!Directory.Exists(protoDirectory))
		{
			//如果不存在,则创建
			Directory.CreateDirectory(protoDirectory);
		}

		//然后判断proto文件夹中是否存在xx.proto文件,也就是为Ixx服务生成的proto文件
		var protoFilePath = Path.Combine(protoDirectory, $"{fileName.TrimStart('I')}.proto");

		//写入proto文件的内容
		File.WriteAllText(protoFilePath, schema);
	}
}