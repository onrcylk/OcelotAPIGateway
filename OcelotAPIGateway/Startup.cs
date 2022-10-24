using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Authorisation;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace OcelotAPIGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;// remove the line when deploying to production
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("*").AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    /*.AllowCredentials()*/);
            });

            var audienceConfig = Configuration.GetSection("Audience");

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            services.AddAuthorization();
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddOcelot();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var configuration = new OcelotPipelineConfiguration
            {
                AuthorisationMiddleware = async (ctx, next) =>
                {
                    if (this.Authorize(ctx))
                    {
                        await next.Invoke();

                    }
                    else
                    {
                        ctx.Errors.Add(new UnauthorisedError($"Fail to authorize"));
                    }

                }
            };

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseOcelot(configuration);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool Authorize(DownstreamContext ctx)
        {
            if (ctx.DownstreamReRoute.AuthenticationOptions.AuthenticationProviderKey == null) return true;
            else
            {
                //flag for authorization
                bool auth = false;

                //where are stored the claims of the jwt token
                Claim[] claims = ctx.HttpContext.User.Claims.ToArray<Claim>();

                //where are stored the required claims for the route
                Dictionary<string, string> required = ctx.DownstreamReRoute.RouteClaimsRequirement;

                auth = claims.Any(claim => required.ContainsKey(claim.Type) && required[claim.Type].Split(',', StringSplitOptions.RemoveEmptyEntries).Contains(claim.Value));

                return auth;
            }
        }
    }
}
