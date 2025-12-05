using GardenBookingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Enable Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Garden Service
builder.Services.AddScoped<IGardenService, GardenService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// CRITICAL: Session must be enabled BEFORE authentication middleware
app.UseSession();

// Add Session Auth Middleware (authenticates user from session)
app.UseMiddleware<SessionAuthMiddleware>();

// Authorization must come AFTER authentication
app.UseAuthorization();

// Default route set to Calendar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Calendar}/{id?}");

app.Run();