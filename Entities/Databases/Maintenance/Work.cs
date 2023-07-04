using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Maintenance;

public class Work
{
    [Key]
    public int Id { get; set; }
    public required string Reason { get; set; }
    public DateTime Expire { get; set; }
}