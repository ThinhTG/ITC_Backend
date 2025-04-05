using Microsoft.AspNetCore.Http;
using ITC.Core.Constants;
using ITC.Core.ExceptionCustom;

namespace ITC.Repositories.ModelViews.KeyModels
{
    public class KeyEndpointModel
    {
        public string? KeyPairId { get; set; }

        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ClaimValue) || string.IsNullOrWhiteSpace(ClaimType))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Không được để trống");
            }

            // Kiểm tra không chứa số
            if (ClaimValue.Any(char.IsDigit) || ClaimType.Any(char.IsDigit))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "không được chứa số.");
            }

            // Kiểm tra không chứa ký tự đặc biệt (bạn có thể điều chỉnh biểu thức chính quy theo yêu cầu cụ thể)
            if (System.Text.RegularExpressions.Regex.IsMatch(ClaimType, @"[\W_]+"))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "ClaimType không được chứa ký tự đặc biệt.");
            }
            return true;
        }
    }
}
