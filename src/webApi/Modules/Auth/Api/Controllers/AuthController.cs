
using Microsoft.AspNetCore.Mvc;
using webApi.Application.Common.Responses;
using webApi.Application.Dtos.Auth.Response;
using webApi.Application.Interfaces;
using webApi.Application.Services;
using webApi.Domain.Dtos.Auth;

namespace webApi.Api.Controllers;
[ApiController]
[Route("/api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger,
                        IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register",Name = "RegisterUser")]
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

    [HttpPost("login", Name = "Login")]
    public string Login()
    {
        return "login user";
    }

    [HttpPost("refresh", Name = "RefreshToken")]
    public string Refresh()
    {
        return "refresh token";
    }

    [HttpPost("logout", Name ="Logout")]
    public string Logout()
    {
        return "logout";
    }
    [HttpPost("password/forget", Name ="ForgetPassword")]
    public string ForgetPassword()
    {
        return "forget user";
    }
    [HttpPost("password/reset", Name ="ResetPassword")]
    public string ResetPassword()
    {
        return "reset password";
    }
}
