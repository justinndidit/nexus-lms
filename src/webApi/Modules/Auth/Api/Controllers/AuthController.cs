
using Microsoft.AspNetCore.Mvc;
using webApi.Application.Common.Responses;
using webApi.Application.Dtos.Auth.Request;
using webApi.Application.Dtos.Auth.Response;
using webApi.Application.Interfaces;
using webApi.Domain.Dtos.Auth;
using webApi.Modules.Auth.Application.Dtos;

namespace webApi.Api.Controllers;
[ApiController]
[Route("/api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private const string RegisterUserControllerName = "RegisterUser";
    private const string LoginUserControllerName = "Login";
    private const string UpdatePasswordControllerName = "ResetPassword";
    private const string VerifyPasswordResetControllerName= "VerifyPasswordResetToekn";
    private const string InitiatePasswordResetControllerName = "InitiatePasswordReset";
    private const string VerifyUserEmailControllerName = "VerifyUserEmail";
    private const string LogoutUserControllerName = "logoutUser";
    private const string RefreshTokenExchangeControllerName = "RefreshTokenExchange";
    public AuthController(ILogger<AuthController> logger,
                        IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register",Name = RegisterUserControllerName)]
    public async Task<IActionResult> Register(CreateUserRequest req)
    {
        try
        {
            _logger.LogInformation($"request body: {req}");
            var userData = await _authService.CreateUser(req);
            string message = "User created Successfully";
            string action = "Please proceed to verify your Email";
            var response = ApiResponse<CreateUserResponse>.Created(userData, message, "",action);
            return CreatedAtRoute("RegisterUser", new {id = userData.UserId}, response);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogError($"BadHttpException: {ex.Message}");
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"internal server error: {ex.Message}");
            return StatusCode(500, ApiResponse<string>.Error(ex.Message));
        }
    }

    [HttpPost("login", Name = LoginUserControllerName)]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        try
        {
            _logger.LogInformation($"Request Body: {req}");

            var loginData = await _authService.Authenticate(req);

            var message = "User authenticated successfully";
            var response = ApiResponse<LoginResponse>.SuccessResponse(
                data: loginData,
                message: message
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
        
    }

    [HttpPost("account/email/verify", Name = VerifyUserEmailControllerName)]
    public async Task<IActionResult> VerifySignupEmail(VerifyEmailRequest req)
    {
        try
        {
            _logger.LogInformation($"Request body: {req}");

            var verificationData = await _authService.VerifyUserEmail(req);

            var message  = "email verified successfully";
            var response = ApiResponse<VerifyEmailResponse>.SuccessResponse(
                data: verificationData,
                message: message,
                action: "Update profile"
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    [HttpPost("logout", Name =LogoutUserControllerName)]
    public async Task<IActionResult> Logout(LogoutRequest req)
    {
        try
        {
            _logger.LogInformation($"Request body: {req}");

            var logoutData = await _authService.InvalidateToken(req);

            var message  = "logged user out successfully";
            var response = ApiResponse<LogoutResponse>.SuccessResponse(
                data: logoutData,
                message: message
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    [HttpPost("refresh", Name = RefreshTokenExchangeControllerName)]
    public async Task<IActionResult> Refresh(ExchangeTokenRequest req)
    {
        try
        {
            _logger.LogInformation($"Request body: {req}");

            var exchangeTokenResponse = await _authService.ExchangeToken(req);

            var message  = "refresh token exchanged successfully";
            var response = ApiResponse<ExchangeTokenResponse>.SuccessResponse(
                data: exchangeTokenResponse,
                message: message
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"error occured whilst processing request");
            throw;
        }    
    }

    [HttpPost("password/forget/initiate", Name =InitiatePasswordResetControllerName)]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest req)
    {
        try
        {
            _logger.LogInformation($"Request body: {req}");

            var resetPasswordData = await _authService.InitiatePasswordReset(req);

            var message  = "Password reset initiated successfully";
            var action = "Check email for verification token";
            var response = ApiResponse<ForgetPasswordResponse>.SuccessResponse(
                data: resetPasswordData,
                message: message,
                action: action
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    [HttpPost("password/forget/verify", Name = VerifyPasswordResetControllerName)]
    public async Task<IActionResult> VerifyPasswordReset(VerifyPasswordResetRequest req)
    {
        try
        {
            _logger.LogInformation($"Request body: {req}");

            var resetPasswordData = await _authService.VerifyPasswordReset(req);

            var message  = "Reset password token verified successfully";
            var action = "Update Password";
            var response = ApiResponse<VerifyPasswordResetResponse>.SuccessResponse(
                data: resetPasswordData,
                message: message,
                action: action
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    [HttpPost("password/reset", Name =UpdatePasswordControllerName)]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
    {
        try
        {
            _logger.LogInformation($"Request body: {req}");

            var resetPasswordData = await _authService.ResetPassword(req);

            var message  = "Password reset initiated successfully";
            var action = "Login with updated credentials";
            var response = ApiResponse<ResetPasswordResponse>.SuccessResponse(
                data: resetPasswordData,
                message: message,
                action: action
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }
}
