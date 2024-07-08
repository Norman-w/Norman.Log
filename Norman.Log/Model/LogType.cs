using System.Linq;

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

		#region 遍历方法,通过使用 LogType[1] 的方式获取已知的 LogType,或者使用code: LogType["Info"] 的方式获取已知的 LogType 以及使用名字 LogType["Info", true] 的方式获取已知的 LogType

		private static LogType[] _knownLogTypes;
		private static LogType[] MapKnownLogTypes()
		{
			//反射获取所有的静态字段
			var fields = typeof(LogType).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			var logTypes = new LogType[fields.Length];
			for (var i = 0; i < fields.Length; i++)
			{
				logTypes[i] = fields[i].GetValue(null) as LogType;
			}
			return logTypes;
		}
		
		public static LogType[] KnownLogTypes => _knownLogTypes ?? (_knownLogTypes = MapKnownLogTypes());
		
		public LogType this[uint value] => KnownLogTypes.FirstOrDefault(lt => lt.Value == value);
		public LogType this[string code] => KnownLogTypes.FirstOrDefault(lt => lt.Code == code);
		public LogType this[string name, bool isName] => KnownLogTypes.FirstOrDefault(lt => lt.Name == name);
		
		#endregion
	}
}