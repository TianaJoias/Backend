using System.Text.Json.Serialization;
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
using WebApi.Queries;
using WebApi.Security;
using GraphQL.Server;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Infra.EF;
using GraphQL.Types;
using Mapster;
using Domain;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using GraphQL.Validation;
using GraphQL.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Infra;
using OpenTelemetry.Trace;
using Domain.Account;
using Domain.Portifolio;
using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Extensions;
using WebApi.Aplication.Catalog;
using Domain.Catalog;
using System.Linq;

namespace WebApi
{
    public class GraphQLUserContext : Dictionary<string, object>, IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
    public static class GraphQLAuthExtensions
    {
        public static void AddGraphQLAuth(this IServiceCollection services, Action<AuthorizationSettings, IServiceProvider> configure)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.TryAddTransient(s =>
            {
                var authSettings = new AuthorizationSettings();
                configure(authSettings, s);
                return authSettings;
            });
        }

        public static void AddGraphQLAuth(this IServiceCollection services, Action<AuthorizationSettings> configure)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.TryAddSingleton(s =>
            {
                var authSettings = new AuthorizationSettings();
                configure(authSettings);
                return authSettings;
            });
        }
    }
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
            services.AddOpenTelemetryTracing((builder) => builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter());
            services.AddSwagger();
            services.AddSecurity();
            services.AddOptions(Configuration);
            services.AddVersioning();
            services.AddHttpClient();
            services.AddSqlLite(Configuration);
            services.AddMediatR(typeof(Startup));
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
            services.AddSingleton<ProductQuery>();
            services.AddSingleton<TianaJoiasSchema>();
            services.AddSingleton<ProductType>();
            services.AddSingleton<ProductSortType>();
            services.AddSingleton<ProductWhereClauseType>();
            services.AddSingleton<ObjectGraphType<PagedResultType<Product, ProductType>>>();
            services.AddSingleton<EnumerationGraphType<Sort>>();
            services.AddGraphQLAuth((_, s) =>
            {
                _.AddPolicy("AdminPolicy", p => p.RequireClaim("role", "Admin"));
            });
            services.AddHttpContextAccessor();
            services.AddGraphQL((options, provider) =>
            {
                options.EnableMetrics = true;
                //options.excep ExposeExceptions = true;
                var logger = provider.GetRequiredService<ILogger<Startup>>();
                options.NameConverter = new GraphQL.Conversion.CamelCaseNameConverter();
                options.UnhandledExceptionDelegate = ctx => logger.LogError("{Error} occured", ctx.OriginalException.Message);
            })
            .AddSystemTextJson()
            .AddUserContextBuilder(context =>
            {
                return new GraphQLUserContext { User = context.User };
            })
            .AddGraphTypes(typeof(TianaJoiasSchema))
            .AddDataLoader();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider, TianaJoiasContextDB dataContext, IPasswordService passwordService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var validationRules = app.ApplicationServices.GetServices<IValidationRule>();
            app.UseHttpsRedirection();
            app.UseCors("mypolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseGraphQL<TianaJoiasSchema>();

            // use graphql-playground at default url https://localhost:5001/ui/playground
            app.UseGraphQLPlayground();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseVersionedSwagger(provider);
            TianaJoiasContextDB.Seeding(dataContext, passwordService).Wait();
            TypeAdapterConfig<ProductCategory, Guid>
                .NewConfig()
                .MapWith(orgin => orgin.TagId);
            TypeAdapterConfig<Catalog, CatalogsByAgentQueryResult>
                .NewConfig()
                .Map(dest => dest.TotalValue,
                    src =>
                    src.Items.Sum(it => it.Price * it.InitialQuantity))
                .Map(dest => dest.ItemsQuantity, src => src.Items.Sum(it => it.InitialQuantity));
        }
    }
}

