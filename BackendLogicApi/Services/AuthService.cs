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
        private readonly JwtService _jwtService;
        private readonly IEmailService _emailService;

        public class ConflictException : Exception
        {
            public ConflictException(string message) : base(message) { }
        }



        public AuthService(UserLogrepository userRepo, IConfiguration configuration,JwtService jwtService,IEmailService emailService) { 
            _userRepo = userRepo;
            _configuration = configuration;
            _emailService = emailService;
            _jwtService = jwtService;

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
            var token = _jwtService.GenerateEmailVerificationToken(user);
            var verificationLink = $"http://localhost:5142/api/auth/verify?token={token}";
            await _emailService.SendEmailAsync(
    user.Email,
    "Potwierdzenie rejestracji",
    $"Kliknij <a href=\"{verificationLink}\">tutaj</a>, aby potwierdzić rejestrację."
);



        }


    
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user =  await _userRepo.GetByEmailOrLoginAsync(dto.EmailOrLogin);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new Exception("Nieprawidłowy login lub hasło.");
            }


            return _jwtService.GenerateToken(user);


        }
        
    }

    


}
