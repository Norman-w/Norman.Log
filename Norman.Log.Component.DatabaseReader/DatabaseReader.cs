/*
 
 
 
 数据库读取器
 
 当Monitor需要筛选历史记录时,服务端需要从数据库中查询出对应的结果返回给Monitor展示.
 返回结果为业务模型



*/

using System.Collections.Generic;
using System.Linq;
using Norman.Log.Component.Database.Mysql.Context;

namespace Norman.Log.Component.DatabaseReader
{
	/// <summary>
	/// 数据库读取器
	/// </summary>
	public static class DatabaseReader
	{
		/// <summary>
		/// 获取日志
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static List<Component.Database.Mysql.Context.Log> GetLogsFromDb(LogQueryDatabaseArgs query)
		{
			using (var db = new NormanLogDbContext())
			{
				var queryable = db.Log.AsQueryable();
				if (query.StartTime.HasValue)
				{
					queryable = queryable.Where(x => x.CreateTime >= query.StartTime.Value);
				}
				if (query.EndTime.HasValue)
				{
					queryable = queryable.Where(x => x.CreateTime <= query.EndTime.Value);
				}
				if (query.LoggerNameList !=null && query.LoggerNameList.Count>0)
				{
					queryable = queryable.Where(x => query.LoggerNameList.Contains(x.LoggerName));
				}
				if (query.TypeList != null && query.TypeList.Count>0)
				{
					queryable = queryable.Where(x => query.TypeList.Contains(x.Type));
				}
				if (query.LayerList != null && query.LayerList.Count>0)
				{
					queryable = queryable.Where(x => query.LayerList.Contains(x.Layer));
				}
				if (query.ModuleList != null && query.ModuleList.Count>0)
				{
					queryable = queryable.Where(x => query.ModuleList.Contains(x.Module));
				}
				if (query.SummaryTags != null && query.SummaryTags.Count>0)
				{ 
					queryable = queryable.Where(x => query.SummaryTags.Any(tag => x.Summary.Contains(tag)));
				}
				if (query.DetailTags != null && query.DetailTags.Count>0)
				{
					queryable = queryable.Where(x => query.DetailTags.Any(tag => x.Detail.Contains(tag)));
				}
				if (query.ContextTags != null && query.ContextTags.Count>0)
				{
					queryable = queryable.Where(x => query.ContextTags.Any(tag => x.Context.Contains(tag)));
				}

				var orderedQueryable = queryable.OrderByDescending(x => x.CreateTime);
				var skipAndTakeQueryable = orderedQueryable.Skip(query.Skip).Take(query.Take);
				var dbResult = skipAndTakeQueryable.ToList();
				return dbResult;
			}
		}
	}
}