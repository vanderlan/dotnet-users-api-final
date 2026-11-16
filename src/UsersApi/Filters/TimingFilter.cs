using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UsersApi.Filters;

// EX03 - Adicionando filters - Aqui é a classe do filter (mede o tempo de cada action)
public class TimingFilter(ILogger<TimingFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // EX03 - Adicionando filters - Aqui começo a cronometrar antes da action
        var timer = Stopwatch.StartNew();

        // EX03 - Adicionando filters - Aqui executo a action (e os filters seguintes)
        await next();

        // EX03 - Adicionando filters - Aqui registro no log quanto tempo levou
        logger.LogInformation("{Path} levou {Ms} ms",
            context.HttpContext.Request.Path, timer.ElapsedMilliseconds);
    }
}
