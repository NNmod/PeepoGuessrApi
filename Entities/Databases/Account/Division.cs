using System.ComponentModel.DataAnnotations;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class Division
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
    
    public List<User> Users { get; set; }

    public Division()
    {
        Users = new List<User>();
    }
}