
using ITC.BusinessObject.Identity;

namespace ITC.Repositories.ModelViews.UserModels
{
    public class LoginQueryModel
    {
        /// <summary>
        /// lấy ra người dùng
        /// </summary>
        public ApplicationUser? User {  get; set; }  
        /// <summary>
        /// lấy ra vai trò của người dùng
        /// </summary>
        public string? RoleName { get; set; }
    }
}
