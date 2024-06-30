namespace Norman.Log.Model
{
	public partial class LogType
	{
		public LogType(string code, string name, uint value)
		{
			Code = code;
			Name = name;
			Value = value;
		}

		/// <summary>
		/// 日志类型编码
		/// </summary>
		public readonly string Code;
		/// <summary>
		/// 日志类型名称
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// 日志类型值
		/// </summary>
		public readonly uint Value;
		
		

		#region 重载运算符

		public static bool operator ==(LogType left, LogType right)
		{
			return left?.Equals(right) ?? ReferenceEquals(right, null);
		}

		public static bool operator !=(LogType left, LogType right)
		{
			return !(left == right);
		}
		
		public override bool Equals(object obj)
		{
			if (obj is LogType logType)
			{
				return logType.Code == Code && logType.Name == Name && logType.Value == Value;
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