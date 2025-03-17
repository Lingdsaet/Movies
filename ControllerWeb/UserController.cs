using Microsoft.AspNetCore.Mvc;
using Movie.Models;
using Movie.Repository;
using Movie.ResponseDTO;

namespace Movie.ControllerWeb
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Đăng ký người dùng
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserDTO userDTO)
        {
            if (string.IsNullOrEmpty(userDTO.UserName) || string.IsNullOrEmpty(userDTO.Password) || string.IsNullOrEmpty(userDTO.Email))
            {
                return BadRequest("Thông tin không hợp lệ");
            }

            // Kiểm tra xem tài khoản đã tồn tại chưa
            var existingUser = await _userRepository.GetUserByUsernameAsync(userDTO.UserName);
            if (existingUser != null)
            {
                return BadRequest("Tên tài khoản đã tồn tại");
            }

            // Mã hóa mật khẩu (Sử dụng Bcrypt để mã hóa)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            var user = new User
            {
                UserName = userDTO.UserName,
                Password = passwordHash,
                Email = userDTO.Email
            };

            await _userRepository.CreateUserAsync(user);

            return Ok(new
            {
                Message = "Đăng ký thành công",
                User = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email
                }
            });
        }

        // Đăng nhập người dùng
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.UserName) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return BadRequest("Thông tin không hợp lệ");
            }

            // Tìm kiếm tài khoản trong database
            var user = await _userRepository.GetUserByUsernameAsync(loginDTO.UserName);
            if (user == null)
            {
                return Unauthorized("Tài khoản không tồn tại");
            }

            // Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                return Unauthorized("Mật khẩu không đúng");
            }

            // Trả về thông tin người dùng khi đăng nhập thành công
            return Ok(new
            {
                Message = "Đăng nhập thành công",
                User = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email
                }
            });
        }
    }

}
