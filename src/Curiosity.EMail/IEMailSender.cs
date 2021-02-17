using System.Threading.Tasks;

namespace Curiosity.EMail
{
    /// <summary>
    /// EMail sender
    /// </summary>
    public interface IEMailSender
    {
        /// <summary>
        /// Sends EMail.
        /// </summary>
        /// <param name="toAddress">Recipient address.</param>
        /// <param name="subject">EMail subject.</param>
        /// <param name="body">EMail body.</param>
        /// <param name="isBodyHtml">Is body html.</param>
        /// <returns></returns>
        Task<bool> SendAsync(string toAddress, string subject, string body, bool isBodyHtml = false);
    }
}