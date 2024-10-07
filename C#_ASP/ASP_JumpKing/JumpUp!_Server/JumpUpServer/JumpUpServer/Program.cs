var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/Add/{name}", );

app.Run();


static Task<IResult> HandleAdd()
{

    return TypedResults.Ok();
}