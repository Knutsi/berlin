using Berlin.ExampleServer.Business;
using Berlin.Library;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BerlinConfig>((_) => new BerlinConfig
{
    RpcUrlRoutePrefix = "/api/rpc"
});
builder.Services.AddSingleton<RpcMethodCache>();

builder.Services.AddScoped<GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// add middleware to handle RPC requests:
app.AddBerlin();

app.UseHttpsRedirection();

app.Run();
