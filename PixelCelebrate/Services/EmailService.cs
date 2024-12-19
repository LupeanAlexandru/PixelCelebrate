using Microsoft.AspNetCore.Http.HttpResults;
using MimeKit;
using PixelCelebrate.Models;

namespace PixelCelebrate.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "sandbox.smtp.mailtrap.io";
        private readonly string _smtpUser = "f638edf4a813eb";
        private readonly string _smtpPassword = "b7f146b6b4bd35";

        public async Task<bool> SendBulkEmailAsync(List<User> users, int daysBeforeBirthday)
        {
            var comingBirthday = false;
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Pixel Celebrate", _smtpUser));
            var recipients = GetRecipients(users, daysBeforeBirthday);
            var birthdayUsers = GetBirthdayUser(users, daysBeforeBirthday);

            if (birthdayUsers.Count < 1)
            {
                return comingBirthday;
            } else
            {
                comingBirthday = true;
            }

            foreach (var recipient in recipients)
            {
                emailMessage.To.Add(new MailboxAddress("", recipient));
            }

            emailMessage.Body = BuildEmail(emailMessage, birthdayUsers, daysBeforeBirthday);

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, 587, false);
                await client.AuthenticateAsync(_smtpUser, _smtpPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            return comingBirthday;
        }

        private List<User> GetBirthdayUser(List<User> users, int daysBeforeBirthday)
        {
            var today = DateTime.Now;
            var targetDate = today.AddDays(daysBeforeBirthday);

            return users.Where(u => u.BirthDay.Day == targetDate.Day && u.BirthDay.Month == targetDate.Month).ToList();
        }
        private List<string> GetRecipients(List<User> users, int daysBeforeBirthday)
        {
            var today = DateTime.Now;
            var targetDate = today.AddDays(daysBeforeBirthday);

            return users.Where(u => u.BirthDay.Day != targetDate.Day || u.BirthDay.Month != targetDate.Month ).Select(u => u.Email).ToList();
        }

        private MimeEntity BuildEmail(MimeMessage emailMessage, List<User> users, int daysBeforeBirthday)
        {
            var emailBody = "";
                if(users.Count > 1)
                {
                    emailBody = $"Hey Team,\n\nWe have {users.Count} birthdays coming in {daysBeforeBirthday} days! Let's get ready to celebrate our friends: ";
                    foreach (var user in users)
                    {
                        emailBody += user.FirstName + "\n";
                    }
                    emailBody += "\n\nBest regards,\nPixel Celebrate";
                } 
                else
                {
                    emailBody = $"Hey Team,\n\nIt's almost {users.SingleOrDefault().FirstName}'s birthday in {daysBeforeBirthday} days! Let's get ready to celebrate!\n\nBest regards,\nPixel Celebrate";
                }

            var emailSubject = "Celebrate a Special Day!";
            emailMessage.Subject = emailSubject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = emailBody
            };

           return emailMessage.Body = bodyBuilder.ToMessageBody();
        }
    }
}
