using System.ComponentModel.DataAnnotations;

namespace Domain.Base;

public abstract class BaseEntity
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool Excluded { get; set; } = false;
}