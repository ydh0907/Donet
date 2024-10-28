using JumpUpServer;
Dictionary<string, PlayerInfo> players = new();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.MapGet("/Add/{name}", HandleAdd);

app.Run();

async Task<IResult> HandleAdd(string id)
{
    players.Add(id, new PlayerInfo());
    return TypedResults.Ok(players.Values.ToArray());
}
