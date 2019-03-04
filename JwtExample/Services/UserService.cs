using JwtExample.Data;
using JwtExample.DTOs;
using JwtExample.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtExample.Services
{
    public interface IUserService
    {
        Task<UserDTO> Authenticate(LoginDTO loginDto);
        IEnumerable<UserDTO> GetAll();
    }

    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;

        public UserService(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        public async Task<UserDTO> Authenticate(LoginDTO loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            // Check if user exists or if passwords don't match
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return null;

            // Configure token generation. Can be changed.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            UserDTO userDto = new UserDTO(user, tokenHandler.WriteToken(token));

            return userDto;
        }

        public IEnumerable<UserDTO> GetAll()
        {
            List<UserDTO> userList = new List<UserDTO>();

            foreach (var user in _userManager.Users)
            {
                userList.Add(new UserDTO(user));
            }

            return userList;
        }
    }
}
