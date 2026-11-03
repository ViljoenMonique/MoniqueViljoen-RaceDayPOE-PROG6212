using RaceDay.Web.Services;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Shared CookieContainer so the session is kept between MVC and the API
var cookieContainer = new System.Net.CookieContainer();
builder.Services.AddSingleton(cookieContainer);
// HttpClient for calling the Part 2 API
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("http://api:8080/"); // Docker service name
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(sp =>
{
    var container = sp.GetRequiredService<System.Net.CookieContainer>();
    return new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = container
    };
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
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();