namespace Service.Security;

public interface IJwtService
{
    string GenerateJwtToken(string userId, string userName, List<string> roles);

}