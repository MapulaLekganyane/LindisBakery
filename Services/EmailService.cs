using System.Net;
using System.Net.Mail;

namespace LindisBakery.Data
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendOrderConfirmationAsync(
            string toEmail,
            string customerName,
            string paymentMethod,
            decimal total)
        {
            try
            {
                var smtp = _config.GetSection("SmtpSettings");

                if (!int.TryParse(smtp["Port"], out int port))
                    throw new InvalidOperationException("Invalid SMTP port");

                using var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                    Subject = $"Order Confirmation - Lindi's Home Bakery",
                    IsBodyHtml = true,
                    Body = $@"
                        <h2>Thank you for your order, {customerName}!</h2>
                        <p>Your order has been received successfully.</p>
                        <p><strong>Payment Method:</strong> {paymentMethod}</p>
                        <p><strong>Total Amount:</strong> R {total:F2}</p>
                        <p>We'll contact you shortly regarding delivery.</p>
                        <br />
                        <p>❤️ Lindi's Home Bakery</p>"
                };

                message.To.Add(toEmail);
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order confirmation email");
            }
        }
    }
}
