using System.Reflection;
using API.Hubs;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Data;
using Entities.Framework;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services.Services;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using WebFramework.Middlewares;
using WebFramework.Swagger;
using WebFramework.UserData;

namespace API
{
    public class Startup
    {
        private readonly SiteSettings _siteSettings;
        public IConfiguration Configuration;
        public ILifetimeScope AutofacContainer { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AutoMapperConfiguration.InitializeAutoMapper();
            _siteSettings = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.AddDbContext(Configuration);
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200").WithMethods("GET", "POST", "DELETE", "PUT")
                    .AllowAnyMethod().WithExposedHeaders("X-Pagination")
                    .AllowAnyHeader().AllowCredentials();
            }));
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserValidator>())
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddJwtAuthentication(_siteSettings.JwtSettings);
            services.AddCustomApiVersioning();
            //services.AddHostedService<TimedAuditLogHostedService>();
            //services.AddHostedService<TimedHardwareHostedService>();
            services.AddSwagger();
            services.AddSignalR();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomExceptionHandler();
            app.UseHosts(env);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("MyPolicy");
            app.UseMvc();
            app.UseSwaggerAndUi();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<AxHub>("/AxHub");
            });

            AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            AutoFacSingleton.Instance = AutofacContainer;
        }


        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.Populate(services);

            //Register Services to Autofac ContainerBuilder
            builder.AddServices();

        }

    }
}
