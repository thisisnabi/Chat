using Chat.APIs.Common;
using Chat.APIs.Hubs;
using Chat.APIs.REST;
using Chat.APIs.SharedData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddDbContext<ChatDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SvcDbContext"));
});

builder.Services.AddSingleton<UserService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost",
         builder =>
         builder.WithOrigins("https://localhost:7241")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("localhost");

app.MapConversationEndpoint();
app.MapUserEndpoint();

app.UseHttpsRedirection();

app.MapHub<ChatHub>("/chats");

app.Run();
