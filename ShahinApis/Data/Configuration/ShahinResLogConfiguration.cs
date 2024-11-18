using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShahinApis.Data.Model;

namespace ShahinApis.Data.Configuration
{
    public class ShahinResLogConfiguration : IEntityTypeConfiguration<ShahinResLog>
    {
        public void Configure(EntityTypeBuilder<ShahinResLog> builder)
        {
            builder.ToTable("Shahin_Res");
            builder.HasKey(entity => entity.Id);
            builder.HasIndex(entity => entity.Id).IsUnique(true);
            builder.HasIndex(entity => entity.ReqLogId).IsUnique(true);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            builder.Property(entity => entity.ResCode).IsRequired();
            builder.Property(entity => entity.PublicReqId).IsRequired();
            builder.Property(entity => entity.HTTPStatusCode).IsRequired();
            builder.Property(entity => entity.ReqLogId).IsRequired();
            builder.Property(entity => entity.JsonRes).IsRequired();
            builder.HasOne(entity => entity.ReqLog).WithMany(entity => entity.ShahinResLogs)
                .HasForeignKey(c => c.ReqLogId);
        }
    }
}

