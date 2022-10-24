using Authentication.WebAPI.Infrastructure;
using Authentication.WebAPI.Models;
using Authentication.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Authentication.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {       
        private ITokenFactory _tokenFactory;
        private readonly IRouteService _routeService;

        public AuthController(IOptions<AppSettings> settings, ITokenFactory tokenFactory, IRouteService routeService)
        {  
            _tokenFactory = tokenFactory;
            _routeService = routeService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginModel loginCredentials)
        {
            if (loginCredentials == null)
            {
                return BadRequest("Invalid client request");
            }

            if (loginCredentials.domainAndLANId == "john" && loginCredentials.password == "123")
            {
                string jwt = _tokenFactory.GenerateAcessToken(loginCredentials.domainAndLANId, "StockOwner");
                string refreshToken = _tokenFactory.GenerateRefreshToken();
                List<string> allowedRoutesByRole = _routeService.GetAllowedRoutes(1);
                return Ok(new { JWT = jwt, RefreshToken = refreshToken, AllowedRoutesByRole = allowedRoutesByRole });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody]RefreshTokenHolder refreshTokenHolder)
        {
            // refresh tokens should be saved to the database whenever it is created against the user
            // retrieve the refresh token from the database with the user details 
            // check the refresh token present in the database            

            string jwt = _tokenFactory.GenerateAcessToken("john", "StockOwner");
            string refreshToken = _tokenFactory.GenerateRefreshToken();
            return Ok(new { JWT = jwt, RefreshToken = refreshToken });
        }

        [HttpPost("logout")]
        public IActionResult LogOut([FromBody]RefreshTokenHolder refreshTokenHolder)
        {
            // remove the refresh token from DB
            return Ok();
        }

        [HttpGet("getallowedroutes")]
        public IActionResult GetAllowedRoutes([FromBody] User user)
        {
            throw new NotImplementedException();
        }
    }
}