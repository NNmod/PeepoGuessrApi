using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeepoGuessrApi.Entities.Databases.Account;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string TwitchId { get; set; }
    [ForeignKey("Division")]
    public int DivisionId { get; set; }
    public virtual Division? Division { get; set; }
    public required string Name { get; set; }
    public required string ImageUrl { get; set; }
    public int Score { get; set; }
    public int Wins { get; set; }
    public DateTime Update { get; set; }
    
    public List<Summary> Summaries { get; set; }

    public User()
    {
        Update = DateTime.UtcNow;
        Summaries = new List<Summary>();
    }
}