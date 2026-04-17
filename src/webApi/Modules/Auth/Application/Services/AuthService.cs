
using webApi.Application.Common.Mappers;
using webApi.Domain.Models;
using webApi.Application.Dtos.Auth.Request;
using webApi.Application.Dtos.Auth.Response;
using webApi.Application.Interfaces;
using webApi.Domain.Dtos.Auth;
using webApi.Data;
using webApi.Modules.Auth.Application.Dtos;
using webApi.Modules.Auth.Application.Common.Mappers;
using webApi.Modules.Auth.Domain.Interfaces;
using webApi.Application.Dtos;
using webApi.Modules.Users.Application.Dtos;

namespace webApi.Application.Services;

public class AuthService: IAuthService
{
    private readonly LMSApiApplicationContext _dbContext;
    private readonly ILogger<IAuthService> _logger;
    private readonly IPasswordHasher _bcrypt;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly IVerificationTokenRepository _verificationTokenRepo;
    private readonly IUserService _userService;

    public AuthService(
                        LMSApiApplicationContext dbContext,
                        IPasswordHasher bcrypt,
                        IRefreshTokenRepository refreshTokenRepo,
                        IJwtService jwtService,
                        IRoleRepository roleRepo,
                        ILogger<AuthService> logger,
                        IVerificationTokenRepository verificatonTokenRepo,
                        IUserService userService
                    )
    {
        _bcrypt = bcrypt;
        _refreshTokenRepo = refreshTokenRepo;
        _jwtService = jwtService;
        _roleRepo = roleRepo;
        _dbContext = dbContext;
        _logger = logger;
        _verificationTokenRepo = verificatonTokenRepo;
        _userService = userService;
    }

    public async Task<CreateUserResponse> CreateUser(CreateUserRequest req)
    {
        try
        {
            string passwordHash = _bcrypt.Hash(req.Password);
            User user = await _userService.CreateUserWithDefaultRole(
                new CreateUserCommand(req.Email, passwordHash)
            );

            return AuthMapper.ModelToCreateUserResponse(user, user.UserRoles.Select(ur => ur.Role).ToList());
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing: {e.Message}");
            throw;
        }
    }
    public async Task<LoginResponse> Authenticate(LoginRequest req)
    {

        try
        {
            var user = await _userService.GetUserByEmail(req.Email);
            if( !_bcrypt.Verify(req.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid credentials");
            if(!user.IsActive) throw new UnauthorizedAccessException("This is user is disabled please contact supports");
        
            List<string> roles = UserMapper.ToUserRoles(user); 
            var tokenData = await _GenerateJwtToken(user, roles);

            return AuthMapper.ToLoginResponse(
                user.Email,
                roles,
                tokenData
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    public async Task<VerifyEmailResponse> VerifyUserEmail(VerifyEmailRequest req)
    {
        var verificationToken = await _verificationTokenRepo.GetTokenByToken(req.VerificationToken);
        if(verificationToken is null)
        {
            _logger.LogInformation("token not found!");
            throw new BadHttpRequestException("Invalid verification token");  
        } 

        if(!(verificationToken.Email == req.Email)) throw new BadHttpRequestException("Invalid verification token");

        User user = await _userService.GetUserByEmail(req.Email);


        var tokenData = await _GenerateJwtToken(
            user,
            AuthMapper.ModelToRoleNames(user.UserRoles));
        
        return new VerifyEmailResponse(
            user.Email,
            user.UserRoles.Select(ur=> ur.Role.RoleName).ToList(),
            tokenData
        );
    }

    public async Task<LogoutResponse> InvalidateToken(LogoutRequest req)
    {
        RefreshToken? token = await _refreshTokenRepo.GetByToken(req.RrfreshToken);
        if(token is not null)
        {
            token.IsRevoked = true;
            await _refreshTokenRepo.UpdateAsync(token);
        }
            
        return new LogoutResponse($"refresh token revoked successfully!");
    }
    // public Task<DisableUserResponse> DisableUser(DisableUserRequest req)
    // {
    //     throw new NotImplementedException();
    // }

    public Task<ExchangeTokenResponse> ExchangeToken(ExchangeTokenRequest req)
    {
        throw new NotImplementedException();
    }

    public async Task<ForgetPasswordResponse> InitiatePasswordReset(ForgetPasswordRequest req)
    {
        try
        {
            if(await _userService.UserWithEmailExists(req.Email))
            {
                //send Email async - worker
                //generate verification token
                //populate email data
                //send email
                //emit event
            }

            throw new DllNotFoundException($"user with email {req.Email} does not exist");
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    public async Task<VerifyPasswordResetResponse> VerifyPasswordReset(VerifyPasswordResetRequest req)
    {
        var verificationToken = await _verificationTokenRepo.GetTokenByToken(req.VerificationToken);
        if(verificationToken is null)
        {
            _logger.LogInformation("token not found!");
            throw new BadHttpRequestException("Invalid verification token");  
        } 

        if(!(verificationToken.Email == req.Email)) throw new BadHttpRequestException("Invalid verification token");

        return new VerifyPasswordResetResponse("token verified successfully");

    }
    public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest req)
    {
        try
        {

            var user = await _userService.UpdateUserPassword(
                new UpdateUserPasswordCommand(
                    req.Email,
                    _bcrypt.Hash(req.Password)
                )
            );

            return new ResetPasswordResponse(
                $"user {user.Id} updated successfully"
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"error occured whilst processing request: {e.Message}");
            throw;
        }
    }

    private async Task<Token> _GenerateJwtToken(User user, List<string> roles)
    {
        string jwtToken = _jwtService.GenerateAccessToken(
            new JwtPayload(user.Id,user.Email,roles)
        );
        string refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepo.AddAsync(RefreshToken.Create(refreshToken, user.Id));
        return AuthMapper.ToTokenData(jwtToken, refreshToken);
    }

}
