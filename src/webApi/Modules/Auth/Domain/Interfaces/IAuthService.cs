using webApi.Modules.Auth.Application.Dtos;
using webApi.Modules.Users.Application.Dtos;

namespace webApi.Modules.Auth.Domain.Interfaces;

public interface IAuthService
{
    public Task<CreateUserResponse> RegisterUser(string email, string password);
    public Task<LoginResponse> Authenticate(LoginRequest req);
    public Task<VerifyEmailResponse> VerifyUserEmail(VerifyEmailRequest req);
    // public Task<DisableUserResponse> DisableUser(DisableUserRequest req); 
    public Task<ExchangeTokenResponse> ExchangeToken(ExchangeTokenRequest req) ;
    public Task<LogoutResponse> InvalidateToken(LogoutRequest req);
    public Task<ForgetPasswordResponse> InitiatePasswordReset(ForgetPasswordRequest req);
    public Task<VerifyPasswordResetResponse> VerifyPasswordReset(VerifyPasswordResetRequest req);
    public Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest req);
}
