/*
 
 
 
 当日志来源处客户端连接到本服务器后,产生的客户端实例,比如通过gRpc/ws/命名管道/内部调用等方式报送日志的客户端.



*/

using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Norman.Log.Model;
using Norman.Log.Server.CommonFacade;

namespace Norman.Log.Server.Core;

public class ReporterClient : IClient
{
	public event ClientDisconnectedEventHandler? ClientDisconnected;

	
	public readonly string Id;
	private readonly WebSocket _webSocket;

	public static ReporterClient FromSession(SessionCreatedEventArgs session)
	{
		return new ReporterClient(session);
	}


	public ReporterClient(SessionCreatedEventArgs sessionCreatedEventArgs)
	{
		Id = sessionCreatedEventArgs.SessionId;
		_webSocket = sessionCreatedEventArgs.Connection as WebSocket;
	}

	/// <summary>
	/// 启动运行(WebSocket接收消息的回调函数)
	/// </summary>
	/// <returns></returns>
	public async Task StartWorking()
	{
		try
		{
			var buffer = new byte[1024 * 4];
			while (_webSocket.State == WebSocketState.Open)
			{
				var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Close)
				{
					await OnClientDisconnected();
				}
				else if (result.MessageType == WebSocketMessageType.Text)
				{
					var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
					Console.WriteLine($"接收到客户端消息: {message}");
					OnWebSocketMessageReceived(message);
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("读取客户端消息失败,错误信息:" + e.Message);
		}
		finally
		{
			if (_webSocket.State != WebSocketState.Closed)
			{
				_webSocket.Abort();
				_webSocket.Dispose();
			}
		}
	}
	private async Task OnClientDisconnected()
	{
		ClientDisconnected?.Invoke(this);
		await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnected", CancellationToken.None);
	}
	
	/// <summary>
	/// 当接收到WebSocket消息时(接受到的为log4net网络传输模型的json字符串)
	/// </summary>
	/// <param name="message"></param>
	private void OnWebSocketMessageReceived(string message)
	{
		try
		{
			var log4NetModel = JsonConvert.DeserializeObject<LogRecord4Net>(message);
			if (log4NetModel == null)
			{
				Console.WriteLine($"{nameof(OnWebSocketMessageReceived)}:解析日志失败, 无法将消息反序列化为LogRecord4Net, 消息内容:{message}");
				return;
			}
			var logBusinessModel = Log.Model.Log.FromLogRecord4Net(log4NetModel);
			try
			{
				LogReceived?.Invoke(this,log4NetModel.LoggerName, logBusinessModel);
			}
			catch (Exception e)
			{
				Console.WriteLine($"处理日志失败:{e}");
			}
		}
		catch (Exception e)
		{
			Console.WriteLine($"解析日志失败:{e}");
		}
	}
	public event LogReceivedEventHandler? LogReceived;
}