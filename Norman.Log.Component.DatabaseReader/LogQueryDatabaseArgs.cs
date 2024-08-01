using System;
using System.Collections.Generic;

namespace Norman.Log.Component.DatabaseReader
{
	/// <summary>
	/// 日志查询参数
	/// </summary>
	public class LogQueryDatabaseArgs
	{
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public List<string> LoggerNameList { get; set; }
		public List<int> TypeList { get; set; }
		public List<int> LayerList { get; set; }
		public List<string> ModuleList { get; set; }
		/// <summary>
		/// 概要中匹配的关键字集合,只要有一个匹配即可
		/// </summary>
		public List<string> SummaryTags { get; set; }
		/// <summary>
		/// 详情中匹配的关键字集合,只要有一个匹配即可
		/// </summary>
		public List<string> DetailTags { get; set; }
		/// <summary>
		/// 上下文中包含的关键字集合,只要有一个匹配即可
		/// </summary>
		public List<string> ContextTags { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }
	}
}