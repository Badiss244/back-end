using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            var mediator = ctx.RequestServices.GetRequiredService<IMediator>();

            Guid a = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var inMaintenance0 = await mediator.Send(new GetGenericQuery<Parametres, Guid>(a));
            bool inMaintenance = inMaintenance0.Maintenance;
            var user = ctx.User;
            if (inMaintenance
                && user?.Identity?.IsAuthenticated == true
                && !user.IsInRole("Admin"))
            {
                ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsync("{ \"message\": \"site ta7t siyana\" }");
                return;
            }

            await _next(ctx);
        }
    }

}
