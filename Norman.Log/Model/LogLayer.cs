/*
 日志级别的枚举,通常包含:
 系统层
 业务层
 数据层
 服务层
 控制器层
 外设层(如果是调用三方服务,就代表三方接口)
 */


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
	}
}