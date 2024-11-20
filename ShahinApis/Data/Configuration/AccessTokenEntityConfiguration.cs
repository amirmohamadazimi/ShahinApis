using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahinApis.Data.Model;

namespace ShahinApis.Data.Configuration
{
    public class AccessTokenEntityConfiguration : IEntityTypeConfiguration<AccessTokenEntity>
    {
        public void Configure(EntityTypeBuilder<AccessTokenEntity> builder)
        {
            builder.ToTable("NAJI_ACCESS_TOCKEN");
            builder.HasKey(entity => entity.Id);
            builder.HasIndex(entity => entity.Id).IsUnique(true);
            builder.Property(entity => entity.TokenDateTime).IsRequired().HasColumnName("TockenDateTime");
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            builder.Property(entity => entity.AccessToken).IsRequired().HasColumnName("accessTocken");
            builder.Property(entity => entity.TokenName).IsRequired();
        }
    }
}
