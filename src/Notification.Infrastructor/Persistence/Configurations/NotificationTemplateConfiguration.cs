using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities;

namespace Notification.Infrastructure.Persistence.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SubjectTemplate)
            .HasMaxLength(500);

        builder.Property(x => x.BodyTemplate)
            .IsRequired();

        builder.Property(x => x.Channel)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.IsHtml)
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.RequiredVariables)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasIndex(x => x.Channel);
        builder.HasIndex(x => x.IsActive);
    }
}
