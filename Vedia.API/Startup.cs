using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Vedia.API.Middleware;
using Vedia.API.Services;
using Vedia.API.Services.Runnable;

namespace Vedia.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("corsPolicy", builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
                });
            });

            services.AddAuthentication("VediaAuthentication")
                .AddScheme<AuthenticationSchemeOptions, VediaAuthenticationHandler>("VediaAuthentication",null);
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Vedia.API", Version = "v1"});
            });
            services.AddHostedService<DatabaseSetupService>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Services
            builder.RegisterType<WordService>().InstancePerLifetimeScope();
            
            // Mediator & its handlers
            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder
                .RegisterAssemblyTypes(typeof(Startup).Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IRequestHandler<,>)))
                .AsImplementedInterfaces();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Vedia.API v1"));

            app.UseRouting();
            app.UseCors("corsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}