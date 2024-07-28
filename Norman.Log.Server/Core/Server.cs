/*
 
 主服务核心,主要处理接收和广播推送
 
 目前的逻辑比较简单,收到日志后,交给需要接收的客户端去处理,具体有没有缓存池,是否需要异步处理,是否需要持久化,是否需要分布式等等,都是客户端对应的自己处理.

*/

namespace Norman.Log.Server.Core;

public class Server
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public Server()
    {
        
    }
    //报告者列表锁
    private readonly object _reporterClientsLocker = new();
    //接收者列表锁
    private readonly object _receiverClientsLocker = new();
    //报告者列表
    private readonly List<ReporterClient> _reporterClients = new();
    //接收者列表
    private readonly List<ReceiverClient> _receiverClients = new();

    /// <summary>
    /// 处理日志,当从网络/命名管道/内部调用的方式收到日志时,调用此函数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="loggerName"></param>
    /// <param name="logEntry"></param>
    public void HandleLog(object sender,string loggerName, Log.Model.Log logEntry)
    {
        var client = sender as ReporterClient;
        if (client == null)
        {
            Console.WriteLine("Invalid sender");
            return;
        }
        //发送给所有的接收者
        lock (_receiverClientsLocker)
        {
            foreach (var receiver in _receiverClients)
            {
                _ = receiver.Send(loggerName, logEntry);
            }
        }
    }
    
    /// <summary>
    /// 获取当前的报告者列表,一行一个
    /// </summary>
    public string Reporters {
        get{
            lock(_reporterClientsLocker){
               return string.Join("\n", _reporterClients.Select(c => c.Id));   
            }
        }
    }
    /// <summary>
    /// 获取当前的接收者列表,一行一个
    /// </summary>
    public string Receivers {
        get{
            lock(_receiverClientsLocker){
                return string.Join("\n", _receiverClients.Select(c => c.Id));
            }
        }
    }

    /// <summary>
    /// 从报告者列表中移除指定的报告者
    /// </summary>
    /// <param name="client"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void RemoveReporter(ReporterClient client)
    {
        lock (_reporterClientsLocker)
        {
            _reporterClients.Remove(client ?? throw new InvalidOperationException("Invalid client"));
        }
    }

    /// <summary>
    /// 添加报告者到报告者列表
    /// </summary>
    /// <param name="client"></param>
    public void AddReporter(ReporterClient client)
    {
        lock (_reporterClientsLocker)
        {
            _reporterClients.Add(client);
        }
    }

    /// <summary>
    /// 从接收者列表中移除指定的接收者
    /// </summary>
    /// <param name="sender"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void RemoveReceiver(ReceiverClient sender)
    {
        lock (_receiverClientsLocker)
        {
            _receiverClients.Remove(sender ?? throw new InvalidOperationException("Invalid client"));
        }
    }

    /// <summary>
    /// 添加接收者到接收者列表
    /// </summary>
    /// <param name="client"></param>
    public void AddReceiver(ReceiverClient client)
    {
        lock (_receiverClientsLocker)
        {
            _receiverClients.Add(client);
        }
    }
}