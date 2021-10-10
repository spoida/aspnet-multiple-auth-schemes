using System;
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

            // Set the default authentication scheme to AuthSchemeTwo
            services.AddAuthentication(AuthSchemeTwoHandler.AuthSchemeTwo)
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
                    var fallbackPolicy = new AuthorizationPolicyBuilder(AuthSchemeTwoHandler.AuthSchemeTwo)
                        .RequireAuthenticatedUser()
                        .Build();
                    
                    // Important: Don't set the DefaultPolicy here.
                    // If the DefaultPolicy is set then it automatically acts as though [Authorize] has been applied to all controllers. 
                    // Doing this undermines any [Authorize(AuthenticationSchemes = "SchemeName")] set on a controller because the runtime
                    // evaluates both [Authorize(AuthenticationSchemes = "SchemeName")] AND [Authorize] and will succeed if the request has
                    // been authenticated by the default scheme.
                    //options.DefaultPolicy = fallbackPolicy;
                    
                    // Setting the FallbackPolicy means that only AuthSchemeTwo will be used in the absence
                    // of any other authorization directives.
                    options.FallbackPolicy = fallbackPolicy;
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
                // Important: Don't specify RequireAuthorization() here because:
                // 1. It invokes the DefaultPolicy and we are explicitly NOT setting that because it
                //    authorizes ALL authenticated users regardless of authentication scheme.
                // 2. We prefer to use a FallbackPolicy to add authorization to any controller
                //    that doesn't explicitly set the Authorize attribute.
                endpoints.MapControllers();
            });
        }
    }
}
