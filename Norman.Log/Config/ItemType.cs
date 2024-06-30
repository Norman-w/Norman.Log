/*
 
 
 配置中的项/条目 元信息,不包含值.
 包含配置的元信息模板版本,元信息模板的编码
 以及 
 
 记录一条配置的一些信息,如
 配置的编码(Code), eg: 1001 或 1001.1 或 time.zone
 配置的友好名称,用于前台显示
 配置的完整名称
 配置的描述,后期考虑可以是Markdown或Html
 配置的路径(相对root节点的路径)
 配置的值类型,如String,Int,Double,Boolean,DateTime,Enum,Array,Dictionary
 abstract的配置的有效区间(如果适用),配置的验证器(如果适用)

*/

using System;

namespace Norman.Log.Config
{
	public abstract class ItemType<T>
	{
		protected string TemplateVersion { get; set; }
		protected string TemplateCode { get; set; }
		
		public string Code { get; set; }
		public string Name { get; set; }
		public string FullName { get; set; }
		public string Description { get; set; }
		public string Path { get; set; }
		
		protected readonly Type ValueType = typeof(T);
		
		public T DefaultValue { get; set; }
		public static void DefaultValidate(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
		}
		public abstract void Validate(object value);
	}

	public class UintRangeConfigItem : ItemType<uint>
	{
		public uint Min { get; set; }
		public uint Max { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (uint)value;
			if (v < Min || v > Max)
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Value {v} is out of range [{Min}, {Max}]");
			}
		}
	}
	public class IntRangeConfigItem : ItemType<int>
	{
		public int Min { get; set; }
		public int Max { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (int)value;
			if (v < Min || v > Max)
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Value {v} is out of range [{Min}, {Max}]");
			}
		}
	}
	
	public class StringLengthConfigItem : ItemType<string>
	{
		public int Min { get; set; }
		public int Max { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (string)value;
			if (v.Length < Min || v.Length > Max)
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Value {v} is out of range [{Min}, {Max}]");
			}
		}
	}
	public class StringRegexConfigItem : ItemType<string>
	{
		public string Regex { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (string)value;
			if (!System.Text.RegularExpressions.Regex.IsMatch(v, Regex))
			{
				throw new ArgumentException(nameof(value), $"Value {v} does not match regex {Regex}");
			}
		}
	}
	public class EnumConfigItem : ItemType<string>
	{
		public string[] Values { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (string)value;
			if (Array.IndexOf(Values, v) < 0)
			{
				throw new ArgumentException(nameof(value), $"Value {v} is not in enum {string.Join(",", Values)}");
			}
		}
	}
	public class ArrayConfigItem : ItemType<string>
	{
		public ItemType<string> ItemType { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (string)value;
			if (v == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			var values = v.Split(',');
			foreach (var item in values)
			{
				ItemType.Validate(item);
			}
		}
	}
	public class DictionaryConfigItem : ItemType<string>
	{
		public ItemType<string> KeyType { get; set; }
		public ItemType<string> ValueType { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (string)value;
			if (v == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			var values = v.Split(',');
			foreach (var item in values)
			{
				var kv = item.Split(':');
				if (kv.Length != 2)
				{
					throw new ArgumentException(nameof(value), $"Value {v} is not a valid dictionary");
				}
				KeyType.Validate(kv[0]);
				ValueType.Validate(kv[1]);
			}
		}
	}
	public class DateTimeRangeConfigItem : ItemType<DateTime>
	{
		public DateTime Min { get; set; }
		public DateTime Max { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (DateTime)value;
			if (v < Min || v > Max)
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Value {v} is out of range [{Min}, {Max}]");
			}
		}
	}
	public class BooleanConfigItem : ItemType<bool>
	{
		public BooleanConfigItem()
		{
			TemplateVersion = "1.0";
			TemplateCode = "Boolean";
			DefaultValue = false;
		}
		public override void Validate(object value)
		{
			DefaultValidate(value);
		}
	}
	public class DoubleRangeConfigItem : ItemType<double>
	{
		public double Min { get; set; }
		public double Max { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (double)value;
			if (v < Min || v > Max)
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Value {v} is out of range [{Min}, {Max}]");
			}
		}
	}
	public class TimeSpanRangeConfigItem : ItemType<TimeSpan>
	{
		public TimeSpan Min { get; set; }
		public TimeSpan Max { get; set; }
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (TimeSpan)value;
			if (v < Min || v > Max)
			{
				throw new ArgumentOutOfRangeException(nameof(value), $"Value {v} is out of range [{Min}, {Max}]");
			}
		}
	}
	public class TimeConfigItem : ItemType<string>
	{
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (string)value;
			if (!DateTime.TryParse(v, out _))
			{
				throw new ArgumentException(nameof(value), $"Value {v} is not a valid DateTime");
			}
		}
	}

	#region 可选值集合设置项,类似于前端的Select
	
	public class SelectOption
	{
		public string Value { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
	
	public class SelectConfigItem : ItemType<string>
	{
		public SelectOption[] Options { get; set; } = Array.Empty<SelectOption>();
		public override void Validate(object value)
		{
			DefaultValidate(value);
			var v = (string)value;
			if (Array.Find(Options, o => o.Value == v) == null)
			{
				throw new ArgumentException(nameof(value), $"Value {v} is not in select options");
			}
		}
	}

	#endregion
}