using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentra.Domain.Entities.Api;

namespace Zentra.Infrastructure.Data.Mapper.Api;

public class SecurityQuestionsMap
{
    public SecurityQuestionsMap(EntityTypeBuilder<SecurityQuestions> entityBuilder)
    {
        SecurityQuestionsMapping(entityBuilder);
    }

    private void SecurityQuestionsMapping(EntityTypeBuilder<SecurityQuestions> entityBuilder)
    {
        if (entityBuilder != null)
        {
            // Primary Key
            entityBuilder.HasKey(t => t.Id);
            entityBuilder.HasIndex(t => t.Question).HasDatabaseName("IX_SEC_QUESTION").IsUnique();

            // Properties
            entityBuilder.Property(t => t.Question).IsRequired().HasMaxLength(255);

            // Table & Column Mappings
            entityBuilder.ToTable("Zentra_SecurityQuestions");
            entityBuilder.Property(t => t.Id).HasColumnName("Id");
            entityBuilder.Property(t => t.IsDeleted).IsRequired().HasColumnName("IsDeleted");
            entityBuilder.Property(t => t.CreatedOn).IsRequired().HasColumnName("CreatedOn");
            entityBuilder.Property(t => t.CreatedBy).IsRequired().HasMaxLength(255).HasColumnName("CreatedBy");
            entityBuilder.Property(t => t.ModifiedBy).HasMaxLength(255).HasColumnName("ModifiedBy");
            entityBuilder.Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            entityBuilder.HasQueryFilter(m => EF.Property<bool>(m, "IsDeleted") == false);
        }
    }
}
