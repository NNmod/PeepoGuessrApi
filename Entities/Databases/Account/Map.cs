using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class Map
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public bool IsClassic { get; set; }
}