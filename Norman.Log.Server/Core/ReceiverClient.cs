/*
 
 
 当日志接收端客户端连接到本服务器后,产生的客户端实例,比如web监视器.


*/

using System.Net.WebSockets;
using System.Text;
using Norman.Log.Model;
using Norman.Log.Server.CommonFacade;

namespace Norman.Log.Server.Core;

public class ReceiverClient :IClient
{
	public event ClientDisconnectedEventHandler? ClientDisconnected;
	public readonly string Id;
	private readonly WebSocket _webSocket;

	public static ReceiverClient FromSession(SessionCreatedEventArgs session)
	{
		return new ReceiverClient(session);
	}

	public ReceiverClient(SessionCreatedEventArgs sessionCreatedEventArgs)
	{
		Id = sessionCreatedEventArgs.SessionId;
		_webSocket = sessionCreatedEventArgs.Connection as WebSocket;
	}
	
	/// <summary>
	/// 启动运行
	/// </summary>
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
	 
	private void OnWebSocketMessageReceived(string message)
	{
		try
		{
			Console.WriteLine($"日志接收端发来的消息: {message}");
		}
		catch (Exception e)
		{
			Console.WriteLine($"解析日志失败:{e}");
		}
	}
	
	/// <summary>
	/// 发送文本消息
	/// </summary>
	/// <param name="message"></param>
	private async Task Send(string message)
	{
		try
		{
			var buffer = Encoding.UTF8.GetBytes(message);
			await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
		}
		catch (Exception e)
		{
			Console.WriteLine("发送消息失败,错误信息:" + e.Message);
		}
	}
	/// <summary>
	/// 发送日志
	/// </summary>
	/// <param name="log"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task Send(Log.Model.Log log)
	{
		var message = LogRecord4Net.FromLog(log).ToJson();
		await Send(message);
	}
}