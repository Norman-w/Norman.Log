using System;
using System.ComponentModel.DataAnnotations;

namespace Norman.Log.Component.Database.Mysql.Context
{
	public class Log
	{
		[Key]
		public Guid Id { get; set; }
		
		public DateTime CreateTime { get; set; }
		
		public string LoggerName { get; set; }

		public int Type { get; set; }

		public int Layer { get; set; }

		public string Module { get; set; }

		public string Summary { get; set; }

		public string Detail { get; set; }

		public string Context { get; set; }
	}
}