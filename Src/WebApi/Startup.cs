﻿using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Extensions;
using WebApi.Security;
using Infra.EF;
using OpenTelemetry.Trace;
using Domain.Account;
using WebApi.Filters;
using Mapster;
using Domain.Portifolio;
using WebApi.Aplication;
using System.Linq;
using WebApi.Controllers;
using OpenTelemetry.Resources;

namespace WebApi
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
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllers()
                    .AddControllersAsServices()
                    .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>())
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization()
                    .AddJsonOptions(it =>
                    {
                        it.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
            services.AddOpenTelemetryTracing(
                (builder) =>
                {
                    builder
                 .SetResourceBuilder(
                      ResourceBuilder.CreateDefault().AddService("BackofficeApi"))
                  .AddAspNetCoreInstrumentation()
                  .AddJaegerExporter();
                });
            services.AddSwagger();
            services.AddSecurity();
            services.AddOptions(Configuration);
            services.AddVersioning();
            services.AddHttpClient();
            services.AddSqlLite(Configuration);
            services.AddMediatR(typeof(Startup));
            services.AddScoped<ErrorHandlerMiddleware>();
            services.AddSingleton<IFileBatchLotParser, BatchLotParser>();
            services.AddHealthChecks();
            services.AddCors(options =>
            {
                options.AddPolicy("mypolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
                options.AddPolicy("AllowMyOrigin",
                builder => builder.WithOrigins("https://localhost:3000"));
            });

            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider, TianaJoiasContextDB dataContext, IPasswordService passwordService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("mypolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseVersionedSwagger(provider);

            TianaJoiasContextDB.Seeding(dataContext, passwordService).Wait();
            TypeAdapterConfig<Product, ProductQueryResult>.NewConfig()
                .Map(dest => dest.Tags, src => src.Tags.Select(it => it.Id));
        }
    }
}
