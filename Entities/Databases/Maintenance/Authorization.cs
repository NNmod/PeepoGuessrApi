using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Maintenance;

public class Authorization
{
    [Key]
    public int Id { get; set; }
    public required string Code { get; set; }
}