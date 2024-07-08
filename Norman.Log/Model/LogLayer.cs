/*
 日志级别的枚举,通常包含:
 系统层
 业务层
 数据层
 服务层
 控制器层
 外设层(如果是调用三方服务,就代表三方接口)
 */
using System.Linq;
using System.Reflection;


namespace Norman.Log.Model
{
	public partial class LogLayer
	{
		public LogLayer(string code, string name, uint value)
		{
			Code = code;
			Name = name;
			Value = value;
		}

		/// <summary>
		/// 日志级别编码
		/// </summary>
		public readonly string Code;
		/// <summary>
		/// 日志级别名称
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// 日志级别值
		/// </summary>
		public readonly uint Value;

		#region 重载运算符

		public static bool operator ==(LogLayer left, LogLayer right)
		{
			return left?.Equals(right) ?? ReferenceEquals(right, null);
		}

		public static bool operator !=(LogLayer left, LogLayer right)
		{
			return !(left == right);
		}
		
		public override bool Equals(object obj)
		{
			if (obj is LogLayer logLevel)
			{
				return logLevel.Code == Code && logLevel.Name == Name && logLevel.Value == Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Code.GetHashCode() ^ Name.GetHashCode() ^ Value.GetHashCode();
		}

		#endregion

		#region 遍历方法,通过使用 LogLayer[1] 的方式获取已知的 LogLayer,或者使用code: LogLayer["Info"] 的方式获取已知的 LogLayer 以及使用名字 LogLayer["Info", true] 的方式获取已知的 LogLayer
		
		private static LogLayer[] _knownLogLayers;
		private static LogLayer[] MapKnownLogLayers()
		{
			//反射获取所有的静态字段
			var fields = typeof(LogLayer).GetFields(BindingFlags.Public | BindingFlags.Static);
			var logLayers = new LogLayer[fields.Length];
			for (var i = 0; i < fields.Length; i++)
			{
				logLayers[i] = fields[i].GetValue(null) as LogLayer;
			}
			return logLayers;
		}
		
		public static LogLayer[] KnownLogLayers
		{
			get
			{
				if (_knownLogLayers == null)
				{
					_knownLogLayers = MapKnownLogLayers();
				}
				return _knownLogLayers;
			}
		}
		
		public LogLayer this[int index]
		{
			get
			{
				if (index < 0 || index >= KnownLogLayers.Length)
				{
					return null;
				}
				return KnownLogLayers[index];
			}
		}
		
		public LogLayer this[string code]
		{
			get
			{
				return KnownLogLayers.FirstOrDefault(l => l.Code == code);
			}
		}
		 
		public LogLayer this[string name, bool isName]
		{
			get
			{
				return KnownLogLayers.FirstOrDefault(l => l.Name == name);
			}
		}
		 

		#endregion
	}
}