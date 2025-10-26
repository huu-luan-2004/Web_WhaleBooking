var builder = WebApplication.CreateBuilder(args);

// Config sections
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<Web_WhaleBooking.Services.JwtOptions>(jwtSection);

// Authentication & Authorization (JWT)
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.GetValue<string>("Issuer"),
            ValidAudience = jwtSection.GetValue<string>("Audience"),
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key") ?? ""))
        };
    });

builder.Services.AddAuthorization();

// App services
builder.Services.AddSingleton<Web_WhaleBooking.Services.IFirebaseVerifier, Web_WhaleBooking.Services.FirebaseVerifier>();
builder.Services.AddSingleton<Web_WhaleBooking.Services.ITokenService, Web_WhaleBooking.Services.TokenService>();
builder.Services.AddSingleton<Web_WhaleBooking.Services.IUserStore, Web_WhaleBooking.Services.InMemoryUserStore>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add CORS support for API calls
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

// Enable CORS
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Attribute-routed API controllers
app.MapControllers();

// Simple health endpoint for diagnostics
app.MapGet("/healthz", () => Results.Json(new
{
    name = "WhaleBooking API",
    status = "ok",
    now = DateTime.UtcNow,
}));

app.Run();
