namespace Addon_Service_Intern.Interfaces;

public interface IMockAuthService
{
    bool ValidateToken(string token);
}
