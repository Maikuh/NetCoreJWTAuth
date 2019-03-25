using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JwtExample.Data;
using JwtExample.DTOs;
using JwtExample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly IMapper mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            this.mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _userService.Authenticate(loginDto);

            if (user == null)
                return BadRequest(new { error = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _userService.SignUp(dto);

            if (result.Succeeded)
                return Created("", mapper.Map<ApplicationUser, UserDTO>(dto.User));
            else
            {
                List<string> errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return BadRequest(errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromBody] RegisterDTO dto, string id)
        {
            var result = await _userService.Update(dto, id);

            if (result is UserDTO)
                return Ok(result);
            else
                return BadRequest(new { error = result });
        }
    }
}
