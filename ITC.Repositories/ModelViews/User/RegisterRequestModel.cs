
using Microsoft.AspNetCore.Http;
using ITC.Core.Constants;
using ITC.Core.ExceptionCustom;
using System.Text.RegularExpressions;
using AutoMapper;
using ITC.BusinessObject.Identity;

namespace ITC.Repositories.ModelViews.UserModels
{
    public class RegisterRequestModel
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Email đăng ký tài khoản
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Họ tên người dùng
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Vai trò của người dùng
        /// </summary>
        public required string Role { get; set; }

        /// <summary>
        /// Số điện thoại đăng ký
        /// </summary>
        public string? PhoneNumber { get; set; }


        public void Validate()
        {
            // Check for null Username and Password
            if (Username == null || Password == null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng tiền đầy đủ username và password");
            }

            // Check FullName
            if (string.IsNullOrEmpty(FullName) || FullName.Any(ch => !char.IsLetter(ch) && !char.IsWhiteSpace(ch)))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Full name không hợp lệ");
            }

            // Check Username
            if (string.IsNullOrEmpty(Username) || Username.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Username không hợp lệ");
            }

            // Check Email
            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Email không hợp lệ");
            }

            // Check PhoneNumber
            if (string.IsNullOrEmpty(PhoneNumber) ||
                PhoneNumber.Any(ch => !char.IsDigit(ch)) ||
                !PhoneNumber.StartsWith("0") ||
                !PhoneNumber.All(char.IsDigit) ||
                PhoneNumber.Length != 10)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Số điện thoại không hợp lệ");
            }

            // Check Password
            if (string.IsNullOrEmpty(Password) || Password.Any(char.IsWhiteSpace) ||
                        Password.Length < 6 ||
                        !Regex.IsMatch(Password, @"[^a-zA-Z0-9]") ||
                        !Regex.IsMatch(Password, @"\d") ||
                        !Regex.IsMatch(Password, @"[A-Z]"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Mật khẩu chứa tối thiểu 6 kí tự, 1 chữ cái hoa, 1 chữ số và 1 kí tự đặc biệt");
            }

			
		}
	}
}