using CarAPI.Data;
using CarAPI.DTOs;
using CarAPI.Models;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarAPI.Services
{
    public class AuthService
    {
        private readonly DatabaseService _dbService;
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryHours;

        public AuthService(DatabaseService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;

            IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
            _jwtKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JWT Key: Secret is not configured");
            _issuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("JWT Key: Issuer is not configured");
            _audience = jwtSettings["Audience"] ?? throw new ArgumentNullException("JWT Key: Audience is not configured");
            _expiryHours = int.Parse(jwtSettings["ExpiryInHours"] ?? "2");
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            using var connection = _dbService.CreateConnection();

            var dealer = await connection.QueryFirstOrDefaultAsync<Dealer>(
                "SELECT * FROM Dealers WHERE Username = @Username",
                new { Username = request.Username });

            if (dealer == null || !VerifyPassword(request.Password, dealer.PasswordHash))
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            var token = GenerateJwtToken(dealer);
            var expiresAt = DateTime.Now.AddHours(_expiryHours);

            return new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginResponse
                {
                    Token = token,
                    CompanyName = dealer.CompanyName,
                    ExpiresAt = expiresAt
                }
            };
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequest request)
        {
            using var connection = _dbService.CreateConnection();

            // Check if username exists
            var existingDealer = await connection.QueryFirstOrDefaultAsync<Dealer>(
                "SELECT Id FROM Dealers WHERE Username = @Username",
                new { Username = request.Username });

            if (existingDealer != null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            var passwordHash = HashPassword(request.Password);

            await connection.ExecuteAsync(
                "INSERT INTO Dealers (Username, PasswordHash, CompanyName) VALUES (@Username, @PasswordHash, @CompanyName)",
                new { Username = request.Username, PasswordHash = passwordHash, CompanyName = request.CompanyName });

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Registration successful"
            };
        }

        private string GenerateJwtToken(Dealer dealer)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, dealer.Id.ToString()),
                    new Claim(ClaimTypes.Name, dealer.Username),
                    new Claim("CompanyName", dealer.CompanyName)
                }),
                Expires = DateTime.Now.AddHours(_expiryHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer,
                Audience = _audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

    }
}
