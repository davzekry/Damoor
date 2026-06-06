// API/Filters/HttpMethodOperationFilter.cs
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Damoor.Application.Common.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Damoor.API.Filters;

public class HttpMethodOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var errorSchema = context.SchemaGenerator
            .GenerateSchema(typeof(ApiResponse<object>), context.SchemaRepository);

        var errorMedia = new OpenApiMediaType { Schema = errorSchema };

        // Derive expected status codes purely from the HTTP verb —
        // no convention class, no name matching, no maintenance
        var method = context.ApiDescription.HttpMethod?.ToUpperInvariant();

        var codes = method switch
        {
            "GET" => new[] { 401, 404 },
            "POST" => new[] { 400, 401 },
            "PUT" => new[] { 400, 401, 404 },
            "PATCH" => new[] { 400, 401, 404 },
            "DELETE" => new[] { 401, 404 },
            _ => new[] { 400, 401 }
        };

        // Ensure 2xx response exists (Swagger adds it from return type,
        // but guard here in case it hasn't resolved yet)
        var successCode = method == "POST" ? "201" : "200";

        if (!operation.Responses.ContainsKey(successCode))
            operation.Responses[successCode] = new OpenApiResponse
            { Description = "Success" };

        // Stamp error codes with the ApiResponse<object> schema
        foreach (var code in codes)
        {
            var key = code.ToString();
            if (!operation.Responses.ContainsKey(key))
                operation.Responses[key] = new OpenApiResponse
                { Description = GetDescription(code) };

            operation.Responses[key].Content["application/json"] = errorMedia;
        }

        // Remove the generic undocumented "200" Swagger adds to POST actions
        // when we already have a 201
        if (method == "POST" && operation.Responses.ContainsKey("200"))
            operation.Responses.Remove("200");
    }

    private static string GetDescription(int code) => code switch
    {
        400 => "Bad Request — validation failed",
        401 => "Unauthorized — missing or invalid token",
        403 => "Forbidden — insufficient permissions",
        404 => "Not Found",
        500 => "Internal Server Error",
        _ => "Error"
    };
}