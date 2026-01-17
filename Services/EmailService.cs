using LindisBakery.Models;
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


        public async Task SendAdminOrderNotificationAsync(Order order)
        {
            try
            {
                // Get SMTP settings
                var smtp = _config.GetSection("SmtpSettings");

                if (!int.TryParse(smtp["Port"], out int port))
                    throw new InvalidOperationException("Invalid SMTP port");

                using var client = new SmtpClient(smtp["Host"], port)
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = smtp.GetValue<bool>("EnableSsl")
                };

                // Get admin emails
                var adminSettings = _config.GetSection("AdminSettings");
                var adminEmails = new List<string> { adminSettings["AdminEmail"] };

                if (!string.IsNullOrEmpty(adminSettings["AdditionalAdminEmails"]))
                {
                    adminEmails.AddRange(adminSettings["AdditionalAdminEmails"]
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => e.Trim()));
                }

                // Compose email
                using var message = new MailMessage
                {
                    From = new MailAddress(smtp["SenderEmail"], smtp["SenderName"]),
                    Subject = $"New Order Received - {order.OrderNumber}",
                    IsBodyHtml = true,
                    Body = $@"
                <h2>New Order Received!</h2>
                <p><strong>Order Number:</strong> {order.OrderNumber}</p>
                <p><strong>Customer:</strong> {order.CustomerName} ({order.CustomerEmail})</p>
                <p><strong>Phone:</strong> {order.CustomerPhone}</p>
                <p><strong>Payment Method:</strong> {order.PaymentMethod}</p>
                <p><strong>Total:</strong> R {order.Total:F2}</p>
                <h3>Order Items:</h3>
                <ul>
                    {string.Join("", order.Items.Select(i => $"<li>{i.ProductName} x {i.Quantity} - R {i.TotalPrice:F2}</li>"))}
                </ul>
                <p><a href='{adminSettings["AdminUrl"]}'>View in Admin Dashboard</a></p>"
                };

                // Add all admin emails
                foreach (var email in adminEmails)
                    message.To.Add(email);

                // Send email
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin notification email");
            }
        }


    }
}
