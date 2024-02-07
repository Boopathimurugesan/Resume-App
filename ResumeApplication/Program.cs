using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using ResumeDBTracker.Business.Interface;
using ResumeDBTracker.Business.Service;
using ResumeDBTracker.Core.Models;
using ResumeDBTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetService<IConfiguration>();
ConfigurationStringManager.ResumeTrackerDB = configuration.GetValue<string>("ConnectionStrings:SQLAuth");
builder.Services.AddTransient<ICandidate, CandidateService>();
builder.Services.AddTransient<ICandidateSearch, SolrCandidateSearch>();
builder.Services.AddTransient<IUser, CandidateService>();
builder.Services.AddMvc(Options => {
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    Options.Filters.Add(new AuthorizeFilter(policy));
}).AddXmlSerializerFormatters();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Home/Login";
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//app.UseStaticFiles(new StaticFileOptions
//{
//    RequestPath = "/myapps/test/v1"
//});
//app.UsePathBase("/myapps/test/v1");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
