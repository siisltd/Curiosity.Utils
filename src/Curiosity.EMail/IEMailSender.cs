using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools;

namespace Curiosity.EMail
{
    /// <summary>
    /// Service for sending EMail messages.
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
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<Response> SendAsync(string toAddress, string subject, string body, bool isBodyHtml = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends EMail using specified extra params.
        /// </summary>
        /// <param name="toAddress">Recipient address.</param>
        /// <param name="subject">EMail subject.</param>
        /// <param name="body">EMail body.</param>
        /// <param name="isBodyHtml">Is body html.</param>
        /// <param name="emailExtraParams">Additional sending params.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<Response> SendAsync(string toAddress, string subject, string body, bool isBodyHtml, IEMailExtraParams emailExtraParams, CancellationToken cancellationToken = default);
    }
}
