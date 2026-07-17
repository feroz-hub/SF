using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCL.CS.Domain;

public abstract class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual Guid Id { get; set; }

    public virtual bool IsDeleted { get; set; } = false;

    public virtual DateTime CreatedOn { get; set; }

    public virtual DateTime? ModifiedOn { get; set; }

    public virtual string CreatedBy { get; set; }

    public virtual string ModifiedBy { get; set; }

    [ConcurrencyCheck]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public byte[] RowVersion { get; set; }
}
