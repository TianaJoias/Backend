using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Controllers;

namespace WebApi.Extensions
{


    public class FromQueryModelFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var description = context.ApiDescription;
            if (description.HttpMethod.ToLower() != HttpMethod.Get.ToString().ToLower())
            {
                // We only want to do this for GET requests, if this is not a
                // GET request, leave this operation as is, do not modify
                return;
            }

            var actionParameters = description.ActionDescriptor.Parameters;
            var apiParameters = description.ParameterDescriptions
                    .Where(p => p.Source.IsFromRequest)
                    .ToList();
            if (actionParameters.Count == apiParameters.Count)
            {
                // If no complex query parameters detected, leave this operation as is, do not modify
                return;
            }
            if (context.ApiDescription.ActionDescriptor.Parameters
                .Any(ad => ad.ParameterType == typeof(FilterPaged)))
                operation.Parameters = CreateParameters(actionParameters, operation.Parameters, context);

        }

        private IList<OpenApiParameter> CreateParameters(
            IList<ParameterDescriptor> actionParameters,
            IList<OpenApiParameter> operationParameters,
            OperationFilterContext context)
        {
            var newParameters = actionParameters
                .Select(p => CreateParameter(p, operationParameters, context))
                .Where(p => p != null)
                .ToList();

            return newParameters.Any() ? newParameters : null;
        }

        private OpenApiParameter CreateParameter(
            ParameterDescriptor actionParameter,
            IList<OpenApiParameter> operationParameters,
            OperationFilterContext context)
        {
            var operationParamNames = operationParameters.Select(p => p.Name);
            if (operationParamNames.Contains(actionParameter.Name))
            {
                // If param is defined as the action method argument, just pass it through
                return operationParameters.First(p => p.Name == actionParameter.Name);
            }

            if (actionParameter.BindingInfo == null)
            {
                return null;
            }

            var generatedSchema = context.SchemaGenerator.GenerateSchema(actionParameter.ParameterType, context.SchemaRepository);

            var newParameter = new OpenApiParameter
            {
                Name = actionParameter.Name,
                In = ParameterLocation.Query,
                Schema = generatedSchema
            };

            return newParameter;
        }
    }

    public class JsonQueryOperationFilter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var jsonQueryParams = context.ApiDescription.ActionDescriptor.Parameters
                .Where(ad => ad.BindingInfo.BinderType == typeof(SortQueryBinder))
                .Select(ad => ad.Name)
                .ToList();

            if (!jsonQueryParams.Any())
            {
                return;
            }

            foreach (var p in operation.Parameters.Where(p => jsonQueryParams.Contains(p.Name)))
            {
                // move the schema under application/json content type
                p.Content = new Dictionary<string, OpenApiMediaType>()
                {
                    [MediaTypeNames.Application.Json] = new OpenApiMediaType()
                    {
                        Schema = p.Schema
                    }
                };
                // then clear it
                p.Schema = null;
            }
        }
    }
}
