namespace Service;

using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class CustomEmailValidator<TUser> : UserValidator<TUser> where TUser : class
{
    public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        var result = await base.ValidateAsync(manager, user);
        var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

        // Get the email property
        var email = await manager.GetEmailAsync(user);
        if (!IsValidEmail(email))
        {
            errors.Add(new IdentityError
            {
                Code = "InvalidEmailFormat",
                Description = "The email format is invalid."
            });
        }

        return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
    }

    private bool IsValidEmail(string email)
    {
        // Simple regex for validating email format
        var emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailRegex);
    }
}
