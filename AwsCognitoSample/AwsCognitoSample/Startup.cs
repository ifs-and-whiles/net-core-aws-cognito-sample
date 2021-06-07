using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AwsCognitoSample
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtConfig = _configuration.GetSection(JwtConfig.JwtConfigName).Get<JwtConfig>();
            
            services.AddControllers();
            
             services
                .AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
            {
                var key = Encoding.ASCII.GetBytes(jwtConfig.SecretKey);

                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey= true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, 
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
            });
             
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo()
                        {
                            Title = "AwsCognitoSample",
                            Version = "v1"
                        }
                    );
                    c.CustomSchemaIds(x => x.FullName);
                    
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                        In = ParameterLocation.Header, 
                        Description = "Please insert JWT token into field",
                        Name = "Jwt Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });
                    
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                        { 
                            new OpenApiSecurityScheme 
                            { 
                                Reference = new OpenApiReference 
                                { 
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer" 
                                } 
                            },
                            new string[] { } 
                        } 
                    });
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                endpoints.MapControllers();
            });
            
            app.UseSwagger();

            app.UseSwaggerUI(
                c => c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json", "AwsCognitoSample v1"
                )
            );
        }
    }
}