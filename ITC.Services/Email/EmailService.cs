using ITC.BusinessObject.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ITC.Services.Email
{


	public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
       

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email), "Recipient email cannot be null or empty");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Inter Trans Connect Website", _configuration["EmailSettings:SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"],
                                          int.Parse(_configuration["EmailSettings:Port"]),
                                          MailKit.Security.SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"],
                                               _configuration["EmailSettings:SenderPassword"]);

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }


		//     public async Task SendConfirmationEmailAsync(ApplicationUser user, string token)
		//     {
		//var encodedToken = Uri.EscapeDataString(token);
		//         var confirmationLink = $"http://localhost:5000/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";
		//         var message = $"Vui lòng nhấp vào link để xác thực email: <a href='{confirmationLink}'>Xác nhận</a>";
		//         await SendEmailAsync(user.Email, "Xác thực email", message);
		//     }
	//	https://thinhdb.felixtien.dev/api/auth/confirm-email

		public async Task SendConfirmationEmailAsync(ApplicationUser user, string token)
		{
			var encodedToken = Uri.EscapeDataString(token);
			var confirmationLink = $"https://thinhdb.felixtien.dev/api/auth?userId={user.Id}&token={encodedToken}";

			var message = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='UTF-8'>
        <style>
            body {{
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                background-color: #f4f4f4;
                padding: 20px;
            }}
            .email-container {{
                max-width: 600px;
                margin: auto;
                background-color: #ffffff;
                border-radius: 8px;
                box-shadow: 0 0 10px rgba(0,0,0,0.1);
                padding: 30px;
            }}
            h2 {{
                color: #333;
            }}
            p {{
                font-size: 16px;
                color: #555;
            }}
            .button {{
                display: inline-block;
                padding: 12px 20px;
                margin-top: 20px;
                background-color: #007bff;
                color: #fff;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
            }}
            .footer {{
                margin-top: 30px;
                font-size: 12px;
                color: #888;
                text-align: center;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <h2>Xác thực email của bạn</h2>
            <p>Chào <strong>{user.FullName ?? user.Email}</strong>,</p>
            <p>Cảm ơn bạn đã đăng ký tài khoản tại Inter Trans Connect. Vui lòng nhấn nút bên dưới để xác thực địa chỉ email của bạn:</p>
            <a href='{confirmationLink}' class='button'>Xác nhận email</a>
            <p>Nếu bạn không tạo tài khoản, bạn có thể bỏ qua email này.</p>
            <div class='footer'>
                &copy; 2025 Inter Trans Connect. All rights reserved.
            </div>
        </div>
    </body>
    </html>";

			await SendEmailAsync(user.Email, "Xác thực email", message);
		}


		public async Task ResendConfirmationEmailAsync(ApplicationUser user, string token)
        {
            var encodedToken = Uri.EscapeDataString(token);
            var confirmationLink = $"https://railwaydeploysrc-production.up.railway.app/api/Auth/confirm-email?userId={user.Id}&token={encodedToken}";
            var message = $"Bạn đã yêu cầu xác thực lại email. Nhấp vào link sau để xác nhận: <a href='{confirmationLink}'>Xác nhận</a>";

            await SendEmailAsync(user.Email, "Xác thực lại email", message);
        }

        public async Task SendResetPasswordEmailAsync(ApplicationUser user, string token)
        {
            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"http://localhost:3000/reset-password?email={user.Email}&token={encodedToken}";
            var message = $"Nhấp vào link để đặt lại mật khẩu: <a href='{resetLink}'>Đặt lại mật khẩu</a>";

            await SendEmailAsync(user.Email, "Đặt lại mật khẩu", message);
        }



    }




}

