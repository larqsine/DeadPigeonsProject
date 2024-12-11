namespace Service.DTOs.UserDto;

public class LoginResultDto
{
    public String Message { get; set; }
    public String UserName { get; set; }
    public IList<String> Roles { get; set; }
    public String Token { get; set; }
    public bool PasswordChangeRequired { get; set; }
}