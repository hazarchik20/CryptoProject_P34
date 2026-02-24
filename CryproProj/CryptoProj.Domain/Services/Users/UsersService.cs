using CryptoProj.Domain.Abstractions;
using CryptoProj.Domain.Exceptions;
using CryptoProj.Domain.Models;
using CryptoProj.Domain.Services.Auth;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CryptoProj.Domain.Services.Users;

public class UsersService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<UsersService> _logger;

    public UsersService(IUserRepository userRepository, ILogger<UsersService> logger, JwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;  

    }

    public async Task<UserResponse> Register(RegisterUserRequest request)
    {
        if (await _userRepository.IsEmailTaken(request.Email))
        {
            throw new EmailAlreadyTakenException(request.Email);
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Username = request.Username,
            CreatedAt = DateTime.UtcNow
        };

        user = await _userRepository.Register(user);
        _logger.LogInformation($"User {user.Username} registered successfully.");
        
        var token = _jwtTokenGenerator.GenerateToken(user);

        return MapToResponse(user, token);
    }

    public async Task<UserResponse> Login(LoginUserRequest request)
    {
        var user = await _userRepository.GetUserByEmail(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }
        
        _logger.LogInformation($"User {user.Username} logged in successfully.");
        
        var token = _jwtTokenGenerator.GenerateToken(user);

        return MapToResponse(user, token);
    }
    public async Task<string> AuthenticateWithGoogle(string IdToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(IdToken);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                new Claim(ClaimTypes.Email, payload.Email),
                new Claim(ClaimTypes.Name, payload.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MyApi",
                audience: "MyApiClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserResponse?> GetById(int userId)
    {
        var user = await _userRepository.Get(userId);
        
        return user == null
            ? null
            : MapToResponse(user);
    }

    private UserResponse MapToResponse(User user, string? token = null) =>
        new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Balance = user.Balance,
            Token = token
        };
}