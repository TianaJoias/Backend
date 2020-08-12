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
using WebApi.Infra;
using WebApi.Queries;
using WebApi.Security;
using GraphQL.Server;
using GraphQL.Types;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace WebApi
{
    public class GraphQLUserContext : Dictionary<string, object>
    {
        public ClaimsPrincipal User { get; set; }
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
                builder => builder.WithOrigins("http://mysite.com"));
            });

            services.AddScoped<ProductQuery>();
            services.AddScoped<ProductType>();
            services.AddHttpContextAccessor();

            services.AddScoped<TianaJoiasSchema>();
            services.AddGraphQL((provider, options) =>
            {
                options.EnableMetrics = true;
                options.ExposeExceptions = true;
                var logger = provider.GetRequiredService<ILogger<Startup>>();
                options.UnhandledExceptionDelegate = ctx => logger.LogError("{Error} occured", ctx.OriginalException.Message);
            })
              //  .AddNewtonsoftJson()
              .AddSystemTextJson()
              .AddGraphTypes(typeof(TianaJoiasSchema), ServiceLifetime.Scoped)
            //.AddNewtonsoftJson() // or use AddSystemTextJson for .NET Core 3+

            .AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User })
            .AddDataLoader();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider)
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
            app.UseGraphQL<TianaJoiasSchema>();

            // use graphql-playground at default url https://localhost:5001/ui/playground
            app.UseGraphQLPlayground();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseVersionedSwagger(provider);
        }
    }
}

