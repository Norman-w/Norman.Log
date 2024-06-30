
namespace Norman.Log.Model
{
	public static class Constant
	{
		public const string DefaultLoggerName = "DefaultLogger";
		public const string DefaultLogFileExtension = "log";
		/// <summary>
		/// 默认日期分隔符
		/// </summary>
		public const string DefaultDateSeparator = "-";
		/// <summary>
		/// 时间与日期的分隔符
		/// </summary>
		public const string DefaultDateTimeSeparator = "_";
		/// <summary>
		/// 默认时间分隔符
		/// </summary>
		public const string DefaultTimeSeparator = "_";
		/// <summary>
		/// 时间在文件名中的默认格式
		/// </summary>
		public const string DefaultTimeFormatOfFile = "HH_mm_ss";
	}
	/// <summary>
	/// 已知静态日志类型
	/// </summary>
	public partial class LogType
	{
		#region 已知静态日志类型
		
		/// <summary>
		/// 日常信息,一些正常的信息
		/// </summary>
		public static readonly LogType Info = new LogType(nameof(Info), "日常信息", 1);
		public static readonly LogType Debug = new LogType(nameof(Debug), "调试", 2);
		public static readonly LogType Error = new LogType(nameof(Error), "错误", 3);
		public static readonly LogType Success = new LogType(nameof(Success), "成功", 4);
		public static readonly LogType Fail = new LogType(nameof(Fail), "失败", 5);
		public static readonly LogType Warning = new LogType(nameof(Warning), "警告", 6);
		public static readonly LogType Start = new LogType(nameof(Start), "开始", 7);
		public static readonly LogType Finish = new LogType(nameof(Finish), "结束", 8);
		public static readonly LogType Bug = new LogType(nameof(Bug), "Bug", 9);
		public static readonly LogType Simulate = new LogType(nameof(Simulate), "模拟", 10);

		#endregion
	}
	
	/// <summary>
	/// 已知静态日志级别
	/// </summary>
	public partial class LogLayer
	{
		#region 已知静态日志级别

		/// <summary>
		/// 未知,无法描述的,不知道属于哪一层的都可以临时的使用这个
		/// </summary>
		public static readonly LogLayer Unknown = new LogLayer(nameof(Unknown), "未知", 0);
		/// <summary>
		/// 系统层,比如应用程序的启动,关闭,配置等
		/// </summary>
		public static readonly LogLayer System = new LogLayer(nameof(System), "系统层", 1);
		/// <summary>
		/// 业务层,比如一些正常的逻辑,像用户登录,注册,购买等
		/// </summary>
		public static readonly LogLayer Business = new LogLayer(nameof(Business), "业务层", 2);
		/// <summary>
		/// 数据层,比如数据库的操作,文件的读写等
		/// </summary>
		public static readonly LogLayer Data = new LogLayer(nameof(Data), "数据层", 3);
		/// <summary>
		/// 一些对外提供的服务如API,WebSocket,gRPC等
		/// 或者是看门狗服务,定时器服务,消息队列服务等
		/// </summary>
		public static readonly LogLayer Service = new LogLayer(nameof(Service), "服务层", 4);
		/// <summary>
		/// 控制器层,比如MVC的控制器,WebAPI的控制器,防抖动控制器,定时器控制器等
		/// </summary>
		public static readonly LogLayer Controller = new LogLayer(nameof(Controller), "控制器层", 5);
		/// <summary>
		/// 外设层,比如一些外设的操作,比如打印机,扫描仪,摄像头等
		/// 或者是一些业务上的外设,如调用第三方的api,邮件,短信,或者什么Encompass,Surge,各种外部SDK等
		/// </summary>
		public static readonly LogLayer Peripheral = new LogLayer(nameof(Peripheral), "外设层", 6);

		#endregion
	}
}