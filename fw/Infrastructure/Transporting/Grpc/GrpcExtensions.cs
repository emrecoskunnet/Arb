using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ArbTech.Infrastructure.Transporting.Grpc.Interceptors;

namespace ArbTech.Infrastructure.Transporting.Grpc;

public static class GrpcExtensions
{
    public static void AddCustomGrpc(this IServiceCollection services)
    {
        services.AddGrpcReflection();
        services.AddGrpcSwagger();
        services
            .AddGrpc(options => { options.Interceptors.Add<ServerLoggerInterceptor>(); })
            .AddJsonTranscoding();
    }

    public static void UseCustomGrpc(this IApplicationBuilder app)
    {
        app.UseCors();
        app.UseGrpcWeb();
    }
    
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policyBuilder => policyBuilder
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding")
            );
        });
    }

    public static void MapCustomGrpcService<TService>(this IEndpointRouteBuilder app, IWebHostEnvironment env,
        string protoFileName)
        where TService : class
    {
        app.MapGet("/_proto/", async ctx =>
        {
            ctx.Response.ContentType = "text/plain";
            await using FileStream fs = new(Path.Combine(env.ContentRootPath, "Protos", protoFileName),
                FileMode.Open, FileAccess.Read);
            using StreamReader sr = new(fs);
            while (!sr.EndOfStream)
            {
                string? line = await sr.ReadLineAsync();
                if (line is "/* >>" or "<< */") continue;
                if (!string.IsNullOrWhiteSpace(line)) await ctx.Response.WriteAsync(line);
            }
        });
        app.MapGrpcService<TService>()
            //.RequireCors("AllowAll")
            .RequireCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader())
            .EnableGrpcWeb();
    }
}
