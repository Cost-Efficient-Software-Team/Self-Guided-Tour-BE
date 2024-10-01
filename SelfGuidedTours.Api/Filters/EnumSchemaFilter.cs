using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SelfGuidedTours.Api.Filters
{
    /// <summary>
    /// Swagger filter that processes Enum types and represents them as strings in the documentation.
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies the filter to the OpenAPI schema.
        /// </summary>
        /// <param name="schema">The OpenAPI schema.</param>
        /// <param name="context">The context for the schema filter.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                foreach (var enumName in Enum.GetNames(context.Type))
                {
                    schema.Enum.Add(new OpenApiString(enumName));
                }
                schema.Type = "string";
                schema.Format = null;
            }
        }
    }
}