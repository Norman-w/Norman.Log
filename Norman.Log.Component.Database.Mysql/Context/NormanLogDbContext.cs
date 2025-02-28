using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Norman.Log.Component.Database.Mysql.Context
{
    public partial class NormanLogDbContext : DbContext
    {
        /// <summary>
        /// 全局的连接字符串,默认是server=localhost;port=3306;database=norman.log;user=root, 请在使用前修改
        /// </summary>
        public static string ConnectionString { get; set; } = "server=localhost;port=3306;database=norman.log;user=root";
        /// <summary>
        /// 全局的数据库版本,默认是8.3.0-mysql
        /// </summary>
        public static string ServerVersion { get; set; } = "8.3.0-mysql";
        public NormanLogDbContext()
        {
        }
        
        /// <summary>
        /// 日志表
        /// </summary>
        public DbSet<Log> Log { get; set; }

        public NormanLogDbContext(DbContextOptions<NormanLogDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql(ConnectionString, x => x.ServerVersion(ServerVersion));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
