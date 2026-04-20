
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
using webApi.Modules.Auth.Application.Common.Security;
using webApi.Modules.Auth.Domain.Models;
namespace webApi.Application.Services;

public class AuthService: IAuthService
{
    private readonly LMSApiApplicationContext _dbContext;
    private readonly ILogger<IAuthService> _logger;
    private readonly IPasswordHasher _bcrypt;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IVerificationTokenRepository _verificationTokenRepo;
    private readonly IUserService _userService;

    public AuthService(
                        LMSApiApplicationContext dbContext,
                        IPasswordHasher bcrypt,
                        IRefreshTokenRepository refreshTokenRepo,
                        IJwtService jwtService,
                        ILogger<AuthService> logger,
                        IVerificationTokenRepository verificatonTokenRepo,
                        IUserService userService
                    )
    {
        _bcrypt = bcrypt;
        _refreshTokenRepo = refreshTokenRepo;
        _jwtService = jwtService;
        _dbContext = dbContext;
        _logger = logger;
        _verificationTokenRepo = verificatonTokenRepo;
        _userService = userService;
    }

    public async Task<CreateUserResponse> CreateUser(CreateUserRequest req)
    {
        string passwordHash = _bcrypt.Hash(req.Password);
        User user = await _userService.CreateUserWithDefaultRole(
            new CreateUserCommand(req.Email, passwordHash)
        );

        //send Email async - worker
        //generate verification token
        var verificationCode = AuthTokenGenerator.GenerateCode(6);

        //populate email data
        await _verificationTokenRepo.Create(
            new VerificationToken(
                req.Email,
                verificationCode
            )
        );
        //send email
        _logger.LogInformation($"verificationCode: {verificationCode}");
        //emit event

        return AuthMapper.ModelToCreateUserResponse(user, user.UserRoles.Select(ur => ur.Role).ToList());
    }
    public async Task<LoginResponse> Authenticate(LoginRequest req)
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

    public async Task<VerifyEmailResponse> VerifyUserEmail(VerifyEmailRequest req)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var verificationToken = await _verificationTokenRepo.GetTokenByToken(req.VerificationToken);

            if(verificationToken is null || !verificationToken.IsTokenValid(req.Email)) throw new UnauthorizedAccessException("Invalid token or expired token");

            User user = await _userService.GetUserByEmail(req.Email);
            user.Activate();
            user = await _userService.UpdateUserActiveStatus(
                new UpdateUserActiveStatusCommand(
                    user.Email,
                    true
                )
            );
            verificationToken.InvalidateVerificationToken();
            await _verificationTokenRepo.UpdateAsync(verificationToken);

            await transaction.CommitAsync();

            var tokenData = await _GenerateJwtToken(
                user,
                AuthMapper.ModelToRoleNames(user.UserRoles));

            return new VerifyEmailResponse(
                user.Email,
                user.UserRoles.Select(ur=> ur.Role.RoleName).ToList(),//problem
                tokenData
            );
        }
        catch(Exception e)
        {
            _logger.LogError(e, "something went wrong");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<LogoutResponse> InvalidateToken(LogoutRequest req)
    {
        RefreshToken? token = await _refreshTokenRepo.GetByToken(req.RefreshToken);
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

    public async Task<ExchangeTokenResponse> ExchangeToken(ExchangeTokenRequest req)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        _logger.LogInformation("db transaction started successfuully");
        try
        {
            var token = await _refreshTokenRepo.GetByToken(req.RefreshToken)
                                    ?? throw new UnauthorizedAccessException("refresh token is invalid");
            _logger.LogInformation($"refresh token {token.Id} fetched successfully");

            if (token.IsRevoked)
                    throw new UnauthorizedAccessException("refresh token is revoked");

            if(token.ExpiryDate <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("refresh token expired");

            token.InvalidateRefreshToken();
            await _refreshTokenRepo.UpdateAsync(token);

            var tokenData = await _GenerateJwtToken(
                token.User,
                UserMapper.ToUserRoles(token.User)
            );
            var refreshToken = RefreshToken.Create(
                tokenData.RefreshToken,
                token.UserId
            );
            await _refreshTokenRepo.AddAsync(refreshToken);
            await transaction.CommitAsync();

            return new ExchangeTokenResponse(
                tokenData.JwtToken,
                tokenData.RefreshToken
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"something went wrong.");
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ForgetPasswordResponse> InitiatePasswordReset(ForgetPasswordRequest req)
    {
        var userExists = await _userService.UserWithEmailExists(req.Email);

        if (userExists)
        {
            //send Email async - worker
            //generate verification token
            var verificationCode = AuthTokenGenerator.GenerateCode(6);

            //populate email data
            await _verificationTokenRepo.Create(
                new VerificationToken(
                    req.Email,
                    verificationCode
                )
            );
            //send email
            _logger.LogInformation($"verificationCode: {verificationCode}");
            //emit event
        }

        return new ForgetPasswordResponse(Message : "If the email exists, a password reset link has been sent.");
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
            if(!(req.Password == req.RePassword)) throw new BadHttpRequestException("Password and RePassword must match");
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
            _logger.LogError(e,$"error occured whilst processing request");
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
