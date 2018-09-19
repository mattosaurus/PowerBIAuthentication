using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using PowerBIAuthentication.Extensions;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureADOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureADOptions _azureOptions;
            private readonly ITokenAcquisition _tokenAcquisition;

            public ConfigureAzureOptions(IOptions<AzureADOptions> azureOptions, ITokenAcquisition tokenAcquisition)
            {
                _azureOptions = azureOptions.Value;
                _tokenAcquisition = tokenAcquisition;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}/v2.0";   // V2 specific
                options.UseTokenLifetime = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateIssuer = false;     // accept several tenants
                options.Events = new OpenIdConnectEvents();
                options.Events.OnAuthorizationCodeReceived = OnAuthorizationCodeReceived;
                // Request Power BI rights as well as standard user read rights
                options.Scope.Add("user.read https://analysis.windows.net/powerbi/api/Dataset.Read.All");
                options.ResponseType = "code id_token";
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }

            private static Func<TContext, Task> WrapOpenIdConnectEvent<TContext>(Func<TContext, Task> baseEventHandler, Func<TContext, Task> thisEventHandler)
            {
                return new Func<TContext, Task>(async context =>
                {
                    await baseEventHandler(context);
                    await thisEventHandler(context);
                });
            }

            private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
            {
                await _tokenAcquisition.AddAccountToCacheFromAuthorizationCode(context, new string[] { "user.read" });
            }

            private Task OnRedirectToIdentityProvider(RedirectContext context)
            {
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                return Task.FromResult(0);
            }
        }
    }
}