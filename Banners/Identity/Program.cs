using Application.Services;
using Identity.API.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(connection));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = 
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
        ValidateIssuerSigningKey = true
        
    };

});
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Authenticate>();
    x.AddConsumer<Register>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://localhost"));
        cfg.ReceiveEndpoint("identityQueue", e =>
        {
            e.PrefetchCount = 20;
            e.UseMessageRetry(r => r.Interval(2, 100));
            e.ConfigureConsumer<Authenticate>(context);
            e.ConfigureConsumer<Register>(context);
        });
       // cfg.ConfigureEndpoints(context);

    });
}) ;
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
try
{
    await SeedRoles.Seed(serviceProvider);
}
catch (Exception ex)
{

    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error occurs during seeding database");
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

