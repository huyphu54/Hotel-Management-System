using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HotelManagement.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Hotel Admin", _configuration["EmailSettings:From"]));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("html") { Text = message };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]), SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_configuration["EmailSettings:SmtpUsername"], _configuration["EmailSettings:SmtpPassword"]);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Ghi lại thông tin lỗi
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                throw; // Ném lại ngoại lệ để xử lý ở nơi gọi nếu cần
            }
        }

    }
}
