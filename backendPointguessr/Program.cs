var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

app.MapControllers();

//Dva originy, protože se mi obèas projekt Vue.JS spouští z jiných portù
app.UseCors(builder => builder.WithOrigins(["http://localhost:5174", "http://localhost:5173"])
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

//app.UseCors(builder =>
//{
//    builder.AllowAnyOrigin()
//           .AllowAnyMethod()
//           .AllowAnyHeader();
//});

app.Run();