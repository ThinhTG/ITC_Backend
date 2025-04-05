
namespace ITC.Repositories.ModelViews.UserModels
{
    public class LoginRequestModel
    {
        /// <summary>
        /// Tên tai khoan
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// mat khau
        /// </summary>
        public required string Password { get; set; }
    }
}