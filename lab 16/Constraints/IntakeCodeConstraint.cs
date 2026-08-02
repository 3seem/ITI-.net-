//Part D
using Microsoft.AspNetCore.Routing;
namespace lab_16.Constraints
{
    public class IntakeCodeConstraint : IRouteConstraint
    {

        private const string ValidCode = "itiA";

        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out var value) || value is null)
            {
                return false;
            }

            return string.Equals(value.ToString(), ValidCode,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}

