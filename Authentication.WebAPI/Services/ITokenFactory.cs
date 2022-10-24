namespace Authentication.WebAPI.Services
{
    public interface ITokenFactory
    {
        string GenerateRefreshToken(int size = 32);
        string GenerateAcessToken(string domainAndLANId, string role);
    }
}
