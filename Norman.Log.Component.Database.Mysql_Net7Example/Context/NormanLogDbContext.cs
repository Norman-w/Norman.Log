using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Norman.Log.Component.Database.Mysql.Context;

public enum LogTypeEnum
{
    Info,
    Debug,
    Warn,
    Error,
    Fatal
}
public enum LogLevelEnum
{
    Info,
    Debug,
    Warn,
    Error,
    Fatal
}

public class Log
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string Detail { get; set; }
    public LogTypeEnum Type { get; set; }
    public LogLevelEnum Level { get; set; }
}

public partial class NormanLogDbContext : DbContext
{
    public DbSet<Log> Logs { get; set; }
    
    public NormanLogDbContext()
    {
    }

    public NormanLogDbContext(DbContextOptions<NormanLogDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=norman.log;user=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.3.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
