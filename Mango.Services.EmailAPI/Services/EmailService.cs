using Mango.Email.Models;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
             _dbOptions = dbOptions;   
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message= new StringBuilder();
            message.AppendLine("<br>Cart Email Requested");
            message.AppendLine("<br>Total "+cartDto.CartHeader.CartTotal);
            message.Append("<br>");
            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.AppendLine(item.Product.Name + " x " + item.Count);
                message.Append("<li>");

            }
            message.Append("<ul>");
            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }
        public async Task<bool> LogAndEmail(string message, string email)
        {
            try {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message,

                };
                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User Registeration Succesful. <br/> Email: " + email;
            await LogAndEmail(message, "");
        }
    }
}
