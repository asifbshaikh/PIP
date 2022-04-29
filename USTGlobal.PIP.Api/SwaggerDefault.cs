using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace USTGlobal.PIP.Api
{
    /// <summary>
    /// SwaggerDefault
    /// </summary>
    public class SwaggerDefault : IOperationFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Bearer Token",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = SecuritySchemeType.OAuth2.ToString(),
                }
            });
        }
    }
}
