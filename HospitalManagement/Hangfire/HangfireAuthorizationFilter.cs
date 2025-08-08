using Hangfire.Dashboard;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (!httpContext.User.Identity.IsAuthenticated)
        {
            return false;
        }

        if (httpContext.User.IsInRole("Sport Administrator") || httpContext.User.IsInRole("System Administrator"))
        {
            return true;
        }

        return false;
    }
}
