using dotnet_ts_support.Models;
using dotnet_ts_support.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_ts_support.Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("getToken")]
        public async Task<ActionResult> GetToken([FromBody] User userIn)
        {
            var user = _userService.GetByName(userIn.name);
            if (user == null) NotFound();
            if (userIn.pw == user.pw)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaa");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.name)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized("try again");
            }
        }

        [HttpGet]
        public ActionResult<List<User>> Get() => _userService.Get();

        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);
            if (user == null) NotFound();
            return user;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            _userService.Create(user);
            return CreatedAtRoute("GetUser", new { id = user.id.ToString() }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, User userIn)
        {
            var user = _userService.Get(id);
            if (user == null) NotFound();
            userIn.id = id;
            _userService.Update(id, userIn);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);
            if (user == null) NotFound();
            _userService.Remove(id);
            return NoContent();
        }
    }
}
