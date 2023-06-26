using PeepoGuessrApi.Entities.Response.Account.User;

namespace PeepoGuessrApi.Entities.Response.Account.Game;

public class GameDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string GameTypeName { get; set; }
    public string GameStatusName { get; set; }
    public DateTime DateTime { get; set; }
    public List<GameRoundDto> Rounds { get; set; }
    public List<UserDto> Users { get; set; }

    public GameDto(int id, string code, string gameTypeName, string gameStatusName, DateTime dateTime,
        List<GameRoundDto> rounds, List<UserDto> users)
    {
        Id = id;
        Code = code;
        GameTypeName = gameTypeName;
        GameStatusName = gameStatusName;
        DateTime = dateTime;
        Rounds = rounds;
        Users = users;
    }
}