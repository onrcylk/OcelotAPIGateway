using System.Collections.Generic;

namespace Authentication.WebAPI.Services
{
    public interface IRouteService
    {
        List<string> GetAllowedRoutes(int roleId);
    }
}
