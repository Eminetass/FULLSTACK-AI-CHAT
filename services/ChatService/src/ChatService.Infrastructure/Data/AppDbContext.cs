using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ChatService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Message> Messages => Set<Message>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Message>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Nickname).IsRequired().HasMaxLength(50);
            b.Property(x => x.Text).IsRequired().HasMaxLength(2000);
            b.Property(x => x.SentimentLabel).HasMaxLength(32);

        // DateTimeOffset -> long (Unix Seconds) dönüşümü
        var sentAtConverter = new ValueConverter<DateTimeOffset, long>(
            v => v.ToUnixTimeSeconds(),
            v => DateTimeOffset.FromUnixTimeSeconds(v)
        );
            b.Property(x => x.SentAtUtc)
            .HasConversion(sentAtConverter)
            .HasColumnType("INTEGER");

            b.HasIndex(x => x.SentAtUtc);
       });
    }
}