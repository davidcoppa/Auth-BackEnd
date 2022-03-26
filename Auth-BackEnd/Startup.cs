using Auth_BackEnd.Context;
using Auth_BackEnd.Helpers;
using Auth_BackEnd.Model;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace Auth_BackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddDataProtection();

            services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton(provider => new MapperConfiguration(config =>
             {
                 config.AddProfile(new AutoMapperProfiles());
             }).CreateMapper()
         );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JWT:key"])),
                    ClockSkew = TimeSpan.Zero

                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim("IsAdmin"));
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth_BackEnd", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.Use(async (contexto, next) =>
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        var originResponse = contexto.Response.Body;
            //        contexto.Response.Body = ms;

            //        await next.Invoke();

            //        ms.Seek(0, SeekOrigin.Begin);
            //        string response = new StreamReader(ms).ReadToEnd();
            //        ms.Seek(0, SeekOrigin.Begin);
            //        await ms.CopyToAsync(originResponse);
            //        contexto.Response.Body = originResponse;
            //    }
            //});

            //app.UseMiddleware<LogHTTPResponse>();

            //   app.UseLogHTTPResponse();
            app.UseHttpsRedirection();

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth_BackEnd v1"));
            }


            app.UseRouting();

            app.UseAuthorization();
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
