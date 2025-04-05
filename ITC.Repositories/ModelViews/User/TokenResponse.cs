namespace ITC.Repositories.ModelViews.UserModels
{
    public class TokenResponse
    {
        /// <summary>
        /// token cho người dùng truy cập
        /// </summary>
        public required string AccessToken { get; set; }

        /// <summary>
        /// token làm mới 
        /// </summary>
        public required string RefreshToken { get; set; }

        /// <summary>
        /// thông tin người dùng
        /// </summary>
        public ResponseUserModel? User { get; set; }
    }
}