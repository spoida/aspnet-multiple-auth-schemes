using System;
using System.Linq;
using AuthTestApi.AuthSchemes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AuthTestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthTestApi", Version = "v1" });
                c.AddSecurityDefinition(AuthSchemeOneHandler.AuthSchemeOne, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = AuthSchemeOneHandler.AuthSchemeOne
                });
                c.AddSecurityDefinition(AuthSchemeTwoHandler.AuthSchemeTwo, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = AuthSchemeTwoHandler.AuthSchemeTwo
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, 
                                Id = AuthSchemeOneHandler.AuthSchemeOne
                            }
                        },
                        Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, 
                                Id = AuthSchemeTwoHandler.AuthSchemeTwo
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            
            services.AddSingleton<IPostConfigureOptions<AuthSchemeOneOptions>, AuthSchemeOnePostConfigureOptions>();
            services.AddSingleton<IPostConfigureOptions<AuthSchemeTwoOptions>, AuthSchemeTwoPostConfigureOptions>();

            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = "DynamicAuthScheme";
                    o.DefaultChallengeScheme = "DynamicAuthScheme";
                    o.DefaultAuthenticateScheme = "DynamicAuthScheme";
                })
                .AddPolicyScheme("DynamicAuthScheme", "Auth Scheme Selector", o =>
                {
                    // Dynamically select the Authentication scheme to evaluate.
                    // If we don't do this, all registered schemes are evaluated when processing a request, which
                    // can result in an identity being populated even when we have selected a specific scheme for authentication.
                    o.ForwardDefaultSelector = context =>
                    {
                        // Inspect the HTTP Authorization header and switch based on its value.
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (authHeader?.StartsWith($"{AuthSchemeOneHandler.AuthSchemeOne} ") == true)
                        {
                            return AuthSchemeOneHandler.AuthSchemeOne;
                        }

                        // Fallback to Authentication scheme AuthSchemeTwo - which also matches our FallbackPolicy below.
                        return AuthSchemeTwoHandler.AuthSchemeTwo;
                    };
                })
                .AddScheme<AuthSchemeOneOptions, AuthSchemeOneHandler>(AuthSchemeOneHandler.AuthSchemeOne, o =>
                {
                    o.ApiKey = "123";
                })
                .AddScheme<AuthSchemeTwoOptions, AuthSchemeTwoHandler>(AuthSchemeTwoHandler.AuthSchemeTwo, o =>
                {
                    o.Secret = "secure";
                });
            
            services.AddAuthorization(options =>
                {
                    // Setting the FallbackPolicy means that only AuthSchemeTwo will be used in the absence
                    // of any other Authorization directives.
                    options.FallbackPolicy = new AuthorizationPolicyBuilder(AuthSchemeTwoHandler.AuthSchemeTwo)
                        .RequireAuthenticatedUser()
                        .Build();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthTestApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Important: Don't specify RequireAuthorization() here because
                // the default authorization policy just allows ALL authenticated users.
                // We instead use a FallbackPolicy to add authorization to any controller
                // that doesn't explicitly set the Authorize attribute.
                endpoints.MapControllers();
            });
        }
    }
}
