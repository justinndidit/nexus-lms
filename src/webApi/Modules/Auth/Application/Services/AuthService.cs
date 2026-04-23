namespace webApi.Modules.Auth.Application.Services;
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

    public async Task<CreateUserResponse> RegisterUser(string email, string password)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {            
            string passwordHash = _bcrypt.Hash(password);
            var result = await _userService.CreateUserWithDefaultRole(email, passwordHash);

            //send Email async - worker
            //generate verification token
            var verificationCode = AuthTokenGenerator.GenerateCode(6);

            //populate email data
            await _verificationTokenRepo.Create(new VerificationToken(email,verificationCode));
            //send email
            _logger.LogInformation($"verificationCode: {verificationCode}");
            //emit event

            await transaction.CommitAsync();
            return new CreateUserResponse(
                result.userId,
                result.email,
                result.roleNames
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong registering user");
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<LoginResponse> Authenticate(LoginRequest req)
    {
        var user = await _userService.GetUserByEmail(req.Email);
        if( !_bcrypt.Verify(req.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Invalid credentials");
        if(!user.IsActive) throw new UnauthorizedAccessException("This is user is disabled please contact supports");

        var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
        var tokenData = await _GenerateJwtToken(user.Id.ToString(), user.Email, roles);

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

            var user = await _userService.GetUserByEmail(req.Email);
            user.Activate();

            user = await _userService.UpdateUserActiveStatus(user.Email,true);
            
            verificationToken.InvalidateVerificationToken();
            await _verificationTokenRepo.UpdateAsync(verificationToken);

            await transaction.CommitAsync();

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

            var tokenData = await _GenerateJwtToken(
                user.Id.ToString(),
                user.Email,
                roles
            );

            return new VerifyEmailResponse(
                user.Email,
                roles,
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
                token.User.Id.ToString(),
                token.User.Email,
                [.. token.User.UserRoles.Select(ur => ur.Role.RoleName)]
            );
            var refreshToken = RefreshToken.Create(
                tokenData.RefreshToken,
                token.UserId.ToString()
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
            await _verificationTokenRepo.Create( new VerificationToken(req.Email,verificationCode));
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
        if(!(req.Password == req.RePassword)) throw new BadHttpRequestException("Password and RePassword must match");
        var user = await _userService.UpdateUserPassword(req.Email,_bcrypt.Hash(req.Password));

        return new ResetPasswordResponse(
            $"user {user.Id} updated successfully"
        );
    }

    private async Task<Token> _GenerateJwtToken(string userId, string email, List<string> roles)
    {
        string jwtToken = _jwtService.GenerateAccessToken(
            new JwtPayload(userId,email,roles)
        );
        string refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepo.AddAsync(RefreshToken.Create(refreshToken, userId));
        return AuthMapper.ToTokenData(jwtToken, refreshToken);
    }

}
