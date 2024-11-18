using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ShahinApis.Data.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCR.Data.Configuration
{
    public class ShahinReqLogConfiguration : IEntityTypeConfiguration<ShahinReqLog>
    {
        public void Configure(EntityTypeBuilder<ShahinReqLog> builder)
        {
            builder.ToTable("Shahin_Req");
            builder.HasKey(entity => entity.Id);
            builder.HasIndex(entity => entity.Id).IsUnique(true);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();
            builder.Property(entity => entity.UserId).IsRequired();
            builder.Property(entity => entity.ServiceId).IsRequired();
            builder.Property(entity => entity.PublicReqId).IsRequired();
            builder.Property(entity => entity.LogDateTime).IsRequired();
            builder.Property(entity => entity.PublicAppId).IsRequired();
            builder.Property(entity => entity.JsonReq).IsRequired(false);
        }
    } 
}
