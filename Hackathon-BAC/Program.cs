using Hackathon_BAC.Hub;
using Hackathon_BAC.Service.IServiceRepo;
using Hackathon_BAC.Service.ServiceRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IDoorService, DoorServiceRepository>();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowReactApp",policy =>
    {
        policy.WithOrigins("http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowReactApp");
app.MapControllers();
app.MapHub<DoorHub>("/doorHub");

app.Run();
