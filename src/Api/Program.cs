
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "MyProjectAPI");
    // Configure additional Swagger UI options here
});

app.UseHttpsRedirection();


app.UseCors();

app.MapControllers();


app.Run();