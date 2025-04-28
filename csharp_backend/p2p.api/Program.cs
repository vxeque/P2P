using Microsoft.EntityFrameworkCore;
using p2p.models;
using p2p.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IP2PService, UniversalDeviceScanner>();

builder.Services.AddDbContext<P2PContext>(options =>
    options.UseSqlite("Data Source=DB.db",
    b => b.MigrationsAssembly("p2p.api")));

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader()
                                                  .AllowAnyMethod();
                      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
