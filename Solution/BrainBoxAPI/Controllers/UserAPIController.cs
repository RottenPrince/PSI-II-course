using BrainBoxAPI.Data;
using BrainBoxAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedModels.User;

namespace BrainBoxAPI.Controllers
{
    [Route("api/User/[action]")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;

        public UserAPIController(UserManager<ApplicationUser> userManager, JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManager.CreateAsync(
                new ApplicationUser() { UserName = user.UserName, Email = user.Email },
                user.Password
            );

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.First().Description);
            }

            user.Password = null;
            return Created("", user);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserDTO>> GetUser(string username)
        {
            IdentityUser user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email
            };
        }

        [HttpPost]
        public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad credentials");
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var token = _jwtService.CreateToken(user);


            return Ok(token);
        }
    }
}
