using Chat.Client;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddHttpClient<ChatHttpClient>(client =>
    {
        client.BaseAddress = new Uri("http://Chat.Api");
    })
    .AddUserAccessTokenHandler();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";
        // This will allow the container to reach the discovery endpoint
        options.MetadataAddress = "http://identity/.well-known/openid-configuration";
        options.RequireHttpsMetadata = false;

        options.Events.OnRedirectToIdentityProvider = context =>
        {
            // Intercept the redirection so the browser navigates to the right URL in your host
            context.ProtocolMessage.IssuerAddress = "https://localhost:5001/connect/authorize";
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToIdentityProviderForSignOut = context =>
        {
            context.ProtocolMessage.IssuerAddress = "https://localhost:5001/connect/endsession";
            return Task.CompletedTask;
        };
        

        options.ClientId = "web";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        options.SaveTokens = true;

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.Scope.Add("api1");

        options.GetClaimsFromUserInfoEndpoint = true;
    });

builder.Services.AddOpenIdConnectAccessTokenManagement();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
