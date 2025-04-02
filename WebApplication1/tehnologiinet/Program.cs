using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using tehnologiinet;
using tehnologiinet.Interfaces;
using tehnologiinet.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMyFirstServiceInterface, MyFirstService>();
builder.Services.AddScoped<IStudentsRepository, StudentsRepository>();

// Register both database contexts
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql("Host=localhost;Database=tehnologiinet;Username=postgres;Password=parkingshare"));

builder.Services.AddDbContext<DatabaseContext>(options => 
    options.UseNpgsql("Host=localhost;Database=tehnologiinet;Username=postgres;Password=parkingshare"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "ace.ucv.ro",
        ValidIssuer = "ace.ucv.ro",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("lkgjnsdfkljngfdiyerthiyortegjklfjdnbvxcbxfdkogjrpoiyuteroyigjdf4359859046845")),
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Update migrations to use ApplicationDbContext
using (var db = new ApplicationDbContext())
{
   db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// authentication middleware inainte de authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();