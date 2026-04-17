using webApi.Application.Dtos.Auth.Request;
using webApi.Application.Dtos.Auth.Response;
using webApi.Domain.Dtos.Auth;
using webApi.Modules.Auth.Application.Dtos;

namespace webApi.Application.Interfaces;

public interface IAuthService
{
    public Task<CreateUserResponse> CreateUser(CreateUserRequest userData);
    public Task<LoginResponse> Authenticate(LoginRequest req);
    public Task<VerifyEmailResponse> VerifyUserEmail(VerifyEmailRequest req);
    // public Task<DisableUserResponse> DisableUser(DisableUserRequest req); 
    public Task<ExchangeTokenResponse> ExchangeToken(ExchangeTokenRequest req) ;
    public Task<LogoutResponse> InvalidateToken(LogoutRequest req);
    public Task<ForgetPasswordResponse> InitiatePasswordReset(ForgetPasswordRequest req);
    public Task<VerifyPasswordResetResponse> VerifyPasswordReset(VerifyPasswordResetRequest req);
    public Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest req);
}
