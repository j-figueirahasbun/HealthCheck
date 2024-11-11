//We use/add global to the using keyword which allows us to define some common using statements 
//that will automatically be availabl for use within the entire project
global using HealthCheck.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add HealthCheck middleware to the program:
builder.Services.AddHealthChecks().AddCheck<ICMPHealthCheck>("ICMP");
//The AddCheck method adds the ICMP health check to the web application pipeline, and adds it to the the HealthChecks middleware.
//When you add something here you need to add it to the HTTP pipeline below as well

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseDefaultFiles();
//app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHealthChecks(new PathString("/api/health"));
//HealthCheck was added before the controllers so the controllers dont interfere with the 
//Healthcheck and occur before those take place.

//The /api/health path passed to the UseHealthChecks middleware will create a server-side
//route for the health checks.
//Thus this means that a new route was added to the BACKEND server to serve the healthcheck.

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
