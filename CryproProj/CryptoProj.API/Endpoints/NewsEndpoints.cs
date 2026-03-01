using Microsoft.AspNetCore.Mvc;

namespace CryptoProj.API.Endpoints;

public static class NewsEndpoints
{
    // Minimal API
    public static IEndpointRouteBuilder MapNewsEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoint = app.MapGroup("api/v1/news")
            .RequireAuthorization();

        endpoint.MapGet("/", GetNews);

        endpoint.MapGet("{id}", GetNewsById);
        
        return app;
    }

    public static async Task<IResult> GetNews()
    {
        return Results.Ok("news");
    }
    
    public static async Task<IResult> GetNewsById([FromRoute] int id)
    {
        return Results.Ok($"news {id}");
    }
}