using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using testprojekt.Data;
using testprojekt.Services;
using testprojekt.Hubs;
using testprojekt.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры, Razor Pages и SignalR.
// По умолчанию все страницы требуют авторизации, кроме /Index, /Login и /Register.
builder.Services.AddControllers();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Register");
});
builder.Services.AddSignalR();

// Настраиваем базу данных (например, SQLite; строка подключения должна быть указана в appsettings.json)
// Пример appsettings.json: { "ConnectionStrings": { "DefaultConnection": "Data Source=app.db" } }
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<JwtService>();

// Настраиваем аутентификацию: куки для Razor Pages, JWT для API.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.Redirect("/AccessDenied");
            return System.Threading.Tasks.Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.Redirect("/AccessDenied");
            return System.Threading.Tasks.Task.CompletedTask;
        }
    };
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    var secretKey = "ThisIsASuperSecretKeyThatIsAtLeast32BytesLong!";
    var key = Encoding.UTF8.GetBytes(secretKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "yourdomain.com",
        ValidAudience = "yourdomain.com",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// Настраиваем Swagger для API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Введите JWT Bearer токен: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
    };
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, new string[] { } } });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Подключаем SignalR-хаб (например, для комментариев).
app.MapHub<CommentsHub>("/commentshub");

app.MapControllers();
app.MapRazorPages();

// Если пользователь заходит на корневой URL, перенаправляем его на /Index.
app.MapGet("/", context =>
{
    context.Response.Redirect("/Index");
    return System.Threading.Tasks.Task.CompletedTask;
});

// Инициализация базы данных с тестовыми данными.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Если вы используете миграции, можно вызвать context.Database.Migrate();
    // Если миграции не используются, вызываем EnsureCreated().
    context.Database.EnsureCreated();

    // Если таблица рабочих пространств пуста, добавляем тестовые данные.
    if (!context.Workspaces.Any())
    {
        context.Workspaces.AddRange(
            new Workspace
            {
                Name = "Коворкинг для фрилансеров",
                Description = "Пространство с открытой планировкой для фрилансеров. Подходит для дизайнеров, программистов, копирайтеров и других специалистов.",
                Capacity = 10,
                PricePerHour = 50,
                IsAvailable = true
            },
            new Workspace
            {
                Name = "Креативная студия для художников",
                Description = "Пространство с мольбертами, столами для работы с материалами, хорошим освещением и зоной для выставки работ.",
                Capacity = 8,
                PricePerHour = 40,
                IsAvailable = true
            },
            new Workspace
            {
                Name = "Технологический хаб для стартапов",
                Description = "Современное пространство для IT-стартапов с мощными компьютерами, серверами и досками для планирования проектов.",
                Capacity = 12,
                PricePerHour = 60,
                IsAvailable = true
            }
        );
    }

    // Если пользователь с ролью Admin не существует, добавляем его.
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        var adminUser = new User
        {
            Name = "Administrator",
            Email = "artem@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
            Role = "Admin"
        };

        context.Users.Add(adminUser);
    }

    context.SaveChanges();
}

app.Run();
