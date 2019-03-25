using AutoMapper;
using JwtExample.Data;
using JwtExample.DTOs;
using JwtExample.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtExample.Services
{
    public interface IUserService
    {
        Task<UserDTO> Authenticate(LoginDTO loginDto);
        Task<IdentityResult> SignUp(RegisterDTO dto);
        Task<dynamic> Update(RegisterDTO dto, string id);
    }

    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;
        private readonly AppSettings _appSettings;

        public UserService(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _userManager = userManager;
            this.mapper = mapper;
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

            UserDTO userDto = mapper.Map<ApplicationUser, UserDTO>(user);
            userDto.Token = tokenHandler.WriteToken(token);
            userDto.Roles = await _userManager.GetRolesAsync(user);

            return userDto;
        }

        public async Task<IdentityResult> SignUp(RegisterDTO dto)
        {
            var user = dto.User;

            var result = await _userManager.CreateAsync(user, dto.Password);

            return result;
        }

        public async Task<dynamic> Update(RegisterDTO dto, string id)
        {
            var username = await _userManager.FindByNameAsync(dto.User.UserName);
            var email = await _userManager.FindByEmailAsync(dto.User.Email);

            if (string.IsNullOrWhiteSpace(dto.Password))
                return "The password is required";

            if (username != null && username != dto.User)
                return $"Username \"{dto.User.UserName}\" is already taken";
            else if (email != null && email != dto.User)
                return "The email address is already in use";

            var user = await _userManager.FindByIdAsync(id);
            user.FirstName = dto.User.FirstName;
            user.LastName = dto.User.LastName;
            user.UserName = dto.User.UserName;
            user.Email = dto.User.Email;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, dto.Password); ;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return mapper.Map<ApplicationUser, UserDTO>(user);

            return null;
        }

    }
}
