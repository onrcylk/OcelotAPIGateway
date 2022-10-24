using System.Collections.Generic;

namespace Authentication.WebAPI.Services
{
    public class RouteService : IRouteService
    {
        public List<string> GetAllowedRoutes(int roleId)
        {
            List<string> allowedRoutes = new List<string>();
            if(roleId == 1)
            {
                allowedRoutes.Add("Home");
                allowedRoutes.Add("StockStatusConfig");
            }
            else if(roleId == 2)
            {
                allowedRoutes.Add("Home");
                allowedRoutes.Add("ReorderRules");
            }
            return allowedRoutes;

        }
    }
}
