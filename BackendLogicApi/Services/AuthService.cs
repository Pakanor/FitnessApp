using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendLogicApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserLogrepository _userRepo;
        private readonly IConfiguration _configuration;

        public class ConflictException : Exception
        {
            public ConflictException(string message) : base(message) { }
        }



        public AuthService(UserLogrepository userRepo, IConfiguration configuration) { 
            _userRepo = userRepo;
            _configuration = configuration;

        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var userExist = await _userRepo.UserExistsAsync(dto.Email, dto.Username);
            if (userExist)
            {
                throw new ConflictException("Użytkownik już istnieje");
            }
            var user = new User { Username = dto.Username,
                Email=dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) };

            await _userRepo.AddUserAsync(user);

        }
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user =  await _userRepo.GetByEmailOrLoginAsync(dto.EmailOrLogin);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new Exception("Nieprawidłowy login lub hasło.");
            }
            var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()), 
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, "User")
    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }

    


}
