
namespace ITC.Repositories.ModelViews.UserModels
{
    public class ResponseUserModel
    {
        /// <summary>
        /// Id của người dùng
        /// </summary>
        public string? Id { get; set; } 

        /// <summary>
        /// Tên đăng nhập của người dùng
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Email của người dùng
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Họ tên người dùng
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Số điện thoại người dùng
        /// </summary>
        public string? PhoneNumber { get; set; }


        /// <summary>
        /// Vai trờ của người dùng
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTimeOffset CreatedTime { get; set; }
    }
}