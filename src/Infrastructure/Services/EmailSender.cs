using System.Threading.Tasks;
using NoteIt.Core.Services;

namespace NoteIt.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //TODO: Implemend email sending.
            return Task.CompletedTask;
        }
    }
}
