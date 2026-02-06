using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;
using Notification.Domain.Enums;

namespace Notification.Infrastructure.Persistence.Configurations;

public class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
{
    public void Configure(EntityTypeBuilder<NotificationMessage> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.OwnsOne(x => x.Recipient, recipient =>
        {
            recipient.Property(r => r.Address)
                .HasColumnName("RecipientAddress")
                .HasMaxLength(500)
                .IsRequired();

            recipient.Property(r => r.Name)
                .HasColumnName("RecipientName")
                .HasMaxLength(200);

            recipient.Property(r => r.Channel)
                .HasColumnName("Channel")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Content, content =>
        {
            content.Property(c => c.Subject)
                .HasColumnName("Subject")
                .HasMaxLength(500);

            content.Property(c => c.Body)
                .HasColumnName("Body")
                .IsRequired();

            content.Property(c => c.IsHtml)
                .HasColumnName("IsHtml")
                .HasDefaultValue(false);
        });

        builder.Property(x => x.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ScheduledAt);
        builder.HasIndex(x => x.CreatedAt);
    }
}
