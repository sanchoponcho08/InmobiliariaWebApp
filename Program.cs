
using Microsoft.AspNetCore.Authentication.Cookies;
using InmobiliariaWebApp.Repositories;
using InmobiliariaWebApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configuración de Entity Framework DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Registrar Repositorios (patrón ADO.NET)
// Se registra como Scoped para que se cree una instancia por cada petición HTTP.
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Aquí registraremos los demás repositorios a medida que los migremos
// builder.Services.AddScoped<IPropietarioRepository, PropietarioRepository>();
// builder.Services.AddScoped<IInquilinoRepository, InquilinoRepository>();
// builder.Services.AddScoped<IInmuebleRepository, InmuebleRepository>();
// builder.Services.AddScoped<IContratoRepository, ContratoRepository>();
// builder.Services.AddScoped<IPagoRepository, PagoRepository>();

builder.Services.AddControllersWithViews();

// 3. Configurar la autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

// 4. Configuración de la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// 5. Usar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
