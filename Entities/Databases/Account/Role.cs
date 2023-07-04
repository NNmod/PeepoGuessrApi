using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class Role
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
}