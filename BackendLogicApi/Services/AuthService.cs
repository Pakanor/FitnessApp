using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;

namespace BackendLogicApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserLogrepository _userRepo;
        public class ConflictException : Exception
        {
            public ConflictException(string message) : base(message) { }
        }



        public AuthService(UserLogrepository userRepo) { 
            _userRepo = userRepo;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var userExist = await _userRepo.UserExistsAsync(dto.Email, dto.Username);
            if (userExist)
            {
                throw new ConflictException("Użytkownik już istnieje.");
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
            var token = "12345";
            return token;
        }
    }

    


}
