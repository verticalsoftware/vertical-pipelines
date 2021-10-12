using System.Threading.Tasks;

namespace Vertical.Examples.Shared
{
    public interface INotificationService
    {
        Task SendEmailAsync(string from, string to, object email);
    }
}