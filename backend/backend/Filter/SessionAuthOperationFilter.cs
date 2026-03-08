using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace backend.Filter;

public class SessionAuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasSkip = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<SkipSessionAuthAttribute>()
            .Any();
        
        if (hasSkip) return;

        operation.Responses.TryAdd("401", new OpenApiResponse
        {
            Description = "Unauthorized - No valid session or remember token present"
        });

        operation.Responses.TryAdd("503", new OpenApiResponse
        {
            Description = "Service Unavailable - Database is unreachable"
        });
    }
}