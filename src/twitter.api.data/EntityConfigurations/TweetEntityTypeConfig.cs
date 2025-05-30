using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using twitter.api.domain.Models;

namespace twitter.api.data.EntityConfigurations
{
    public class TweetEntityTypeConfig : IEntityTypeConfiguration<Tweet>
    {
        public void Configure(EntityTypeBuilder<Tweet> builder)
        {
            builder.ToTable("Tweets");

            builder.HasKey(u => u.Id);

            builder.HasIndex(t => t.AuthorId);
            builder.HasIndex(t => t.CreatedAt);
            builder.HasIndex(t => new { t.AuthorId, t.CreatedAt });

            builder.Property(u => u.Content)
                .HasMaxLength(280)
                .IsRequired();

            builder.HasOne(u => u.Author)
                .WithMany(p => p.Tweets)
                .HasForeignKey(u => u.AuthorId)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        }
    }
}
