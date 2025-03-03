using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API_TSU_PassTracker.Filter;

public class FormFileSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(IFormFile))
        {
            schema.Type = "string";
            schema.Format = "binary"; 
        }
    }
}
