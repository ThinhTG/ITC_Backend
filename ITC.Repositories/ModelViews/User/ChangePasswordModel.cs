using Microsoft.AspNetCore.Http;
using ITC.Core.Constants;
using ITC.Core.ExceptionCustom;
using System.Text.RegularExpressions;

namespace ITC.Repositories.ModelViews.UserModels
{
    public class ChangePasswordModel
    {
        public string? NewPassword { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(NewPassword) || NewPassword.Any(char.IsWhiteSpace)
                || NewPassword.Length < 6 
                || !Regex.IsMatch(NewPassword, @"[^a-zA-Z0-9]")
                ||!Regex.IsMatch(NewPassword, @"\d")
                ||!Regex.IsMatch(NewPassword, @"[A-Z]"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Mật khẩu phải chứa tối thiểu 6 kí tự, 1 chữ cái hoa,1 chữ số và 1 kí tự đặc biệt");
            }

        }
    }
}
