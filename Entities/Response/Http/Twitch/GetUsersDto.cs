namespace PeepoGuessrApi.Entities.Response.Http.Twitch;

public class GetUsersDto
{
    public List<UserDto> Data { get; set; }

    public GetUsersDto()
    {
        Data = new List<UserDto>();
    }
}