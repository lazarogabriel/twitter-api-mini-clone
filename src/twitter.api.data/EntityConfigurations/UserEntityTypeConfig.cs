using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using twitter.api.domain.Models;

namespace twitter.api.data.EntityConfigurations
{
    public class UserEntityTypeConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.Property(u => u.Username)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasMany(u => u.Tweets)
                .WithOne(p => p.Author);

            builder.Property(u => u.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        }
    }
}
