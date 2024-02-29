using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.AllowEmptyInputInBodyModelBinding = true;
});
builder.Services.AddDbContext<DistSysAcwServer.Models.UserContext>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "CustomAuthentication";
}).AddScheme<AuthenticationSchemeOptions, DistSysAcwServer.Auth.CustomAuthenticationHandler>
    ("CustomAuthentication", options => { });

builder.Services.AddTransient<IAuthorizationHandler, DistSysAcwServer.Auth.CustomAuthorizationHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();