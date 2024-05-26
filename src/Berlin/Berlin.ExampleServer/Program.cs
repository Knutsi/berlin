using Berlin.ExampleServer.Business;
using Berlin.Library.MethodExecution;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBerlin();
builder.Services.AddScoped<GameService>();

var app = builder.Build();
app.UseBerlin();
app.UseHttpsRedirection();

app.Run();
