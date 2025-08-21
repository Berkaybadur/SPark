using Microsoft.AspNetCore.Mvc;

namespace SParkApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;
        public AuthController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var user = await _userService.Register(model);
            return Ok(new { message = "User created", user.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userService.Authenticate(model);
            if (user == null) return Unauthorized();

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users.Select(u => new { u.Username }));
        }
    }
}
