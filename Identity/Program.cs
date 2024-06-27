using Identity.Models;
using Identity.Persistance;
using Identity.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailSendr, EmailSender>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddDbContextPool<SqlServerDb>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
},poolSize:16);


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
{
    //User Option
    option.User.RequireUniqueEmail = true;
    //SignIn Option
    option.SignIn.RequireConfirmedEmail = true;
    //Password Option
    option.Password.RequireUppercase = false;
    option.Password.RequireNonAlphanumeric = false;
    //Lockout Option
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);

}).AddEntityFrameworkStores<SqlServerDb>().AddDefaultTokenProviders().AddErrorDescriber<PersianIdentityErrors>();
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = "/Home/AccessDenied";
    option.LoginPath = "/Account/Login";
    option.LogoutPath = "/Account/LogOut";
    option.Cookie.HttpOnly = true;
    option.ExpireTimeSpan = TimeSpan.FromDays(3);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
