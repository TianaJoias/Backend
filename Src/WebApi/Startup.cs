using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Extensions;
using Microsoft.Extensions.Logging;
using WebApi.Filters.GlobalErrorHandling.Extensions;
using System;
using Application.Common;
using Infra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Application;
using FluentValidation;
using Application.Products.Commands;
using Application.Common.Behaviors;
using BuildingBlocks.EventBus;
using Infra.Application;

namespace WebApi
{
    public struct POLICIES
    {
        public const string USER = "AccessAsUser";
        public const string ADMIN = "AccessAsAdmin";
    }
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllers()
                    .AddControllersAsServices()
                    .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>()
                    .RegisterValidatorsFromAssemblyContaining<ProductQueryValidator>())
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization()
                    .AddJsonOptions(it =>
                    {
                        it.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
            services.AddOpenTelemetry(Configuration);
            services.AddSwagger();
            services.AddOptions(Configuration);
            services.AddVersioning();
            services.AddHttpClient();
            services.AddSqlLite(Configuration);
            services.AddMediatR(typeof(Startup), typeof(IQuery<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
            services.AddScoped<IOrderingIntegrationEventService, OrderingIntegrationEventService>();
            services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
            services.AddScoped<IEventBus, EventBusServiceBus>();
            services.AddValidatorsFromAssembly(typeof(IUnitOfWork).Assembly);
            services.AddCache();
            services.AddCorsCustom();
            services.AddHttpContextAccessor();
            services.AddHealthChecksCustom();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddMicrosoftIdentityWebApi(options =>
                    {
                        Configuration.Bind("AzureAdB2C", options);
                        options.TokenValidationParameters.NameClaimType = "name";
                    },
            options => { Configuration.Bind("AzureAdB2C", options); });
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<IAuthorizationHandler, ScopesHandler>();
            services.AddAuthorization(options =>
            {
                var userAuthPolicyBuilder = new AuthorizationPolicyBuilder();
                options.AddPolicy(POLICIES.USER,
                        policy => policy.Requirements.Add(new ScopesRequirement(SCOPES.USER)));
                options.AddPolicy(POLICIES.ADMIN,
                        policy => policy.Requirements.Add(new ScopesRequirement(SCOPES.ADMIN)));
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,  IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseCors();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseGlobalExceptionHandler(serviceProvider);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.UseHealthChecks(Configuration);
            });
            app.UseVersionedSwagger(serviceProvider);
            app.UserTypeAdapter();
            app.UseEF(serviceScopeFactory);
        }
    }
}
