using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Infrastructure;
using TehnologijaWebaGrgaMasnec.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TehnologijaWebaGrgaMasnec.Repositories;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c => //omogucava nam u swaggeru authorize button
{
    c.SwaggerDoc("v1", new() { Title = "Hotel API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Upiši: Bearer {tvoj_token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



var cs = builder.Configuration.GetConnectionString("HotelReservationDb"); 

builder.Services.AddDbContext<AppDbContext>(options => // bira bazu koju cemo koristit
{
    if (!string.IsNullOrWhiteSpace(cs) &&
        cs.TrimStart().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(cs);
    }
    else
    {
        options.UseSqlServer(cs);
    }
});

builder.Services // bavi se dodavanjem identiteta,uloga 
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>(); // registracija repositoryja
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();


builder.Services.AddAuthentication(options => // JWT Autentifikacija
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!)
        ),

        ClockSkew = TimeSpan.Zero
    };
});


Log.Logger = new LoggerConfiguration() // sprema stvari kao greske,zahtjeve i slicno
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();



using (var scope = app.Services.CreateScope()) 
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();

    
    if (db.Database.IsSqlite()) // ak nema baze napravi ju 
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();

    await IdentitySeeder.SeedAsync(services); // ubacivanje pocetnih podataka
    await DataSeeder.SeedAsync(services);
}


app.UseMiddleware<TehnologijaWebaGrgaMasnec.Middleware.ExceptionMiddleware>(); // hvatanje gresaka


app.UseSwagger();
app.UseSwaggerUI();


app.UseDefaultFiles();
app.UseStaticFiles();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
