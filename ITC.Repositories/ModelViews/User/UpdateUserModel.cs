using Microsoft.AspNetCore.Http;
using ITC.Core.Constants;
using ITC.Core.ExceptionCustom;
using System.Text.RegularExpressions;

namespace ITC.Repositories.ModelViews.UserModels
{
    public class UpdateUserModel
    {
        /// <summary>
        /// Họ tên người dùng để cập nhật
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Địa chỉ email để cập nhật
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Số điện thoại để cập nhật
        /// </summary>
        public string? PhoneNumber {get; set; }

        /// <summary>
        /// Vai trò để cập nhật
        /// </summary>
        public required string Role { get; set; }
        public void Validate()
        {
            // Check FullName
            if (string.IsNullOrWhiteSpace(FullName) ||
                FullName.Any(ch => !char.IsLetter(ch) && !char.IsWhiteSpace(ch)))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Tên không hợp lệ");
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

            // Check Email
            if (string.IsNullOrWhiteSpace(Email) ||
                !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Email không hợp lệ");
            }
        }
    }
}