using System.Collections.Generic;
using Newtonsoft.Json;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Response of sending <see cref="UnisenderGoSendEmailRequest"/>.
    /// </summary>
    public class UnisenderGoSendEmailResponse
    {
        /// <summary>
        /// Status of sending email.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = null!;

        /// <summary>
        /// Is sending was successful.
        /// </summary>
        [JsonIgnore]
        public bool IsSuccessful => Status == "success";

        /// <summary>
        /// The ID of the sending task, may be useful in finding out the causes of failures.
        /// </summary>
        [JsonProperty("job_id")]
        public string JobId { get; set; } = null!;

        /// <summary>
        /// Array of email addresses successfully accepted for sending.
        /// </summary>
        [JsonProperty("emails")]
        public IReadOnlyList<string>? Emails { get; set; }

        /// <summary>
        /// An object with email addresses that were not accepted for sending.
        /// Email is represented by the property name, and its status is represented by the property value, for example: {“email1@gmail.com ”: “temporary_unavailable"}.
        /// </summary>
        /// <remarks>
        /// Possible statuses of addresses that could not be sent to:
        /// - unsubscribed - the specified address has been unsubscribed;
        /// - invalid - address does not exist, or entered incorrectly;
        /// - duplicate - address is repeated in the request, re-sending to the same email is prevented;
        /// - temporary_unavailable - the address is temporarily unavailable. This means that within the next three days, sending to this address will return an error. The email may be temporarily unavailable for various reasons, for example:
        ///   - 1. the previous sending was rejected by the recipient's server as spam;
        ///   - 2. the recipient's mailbox is full or not in use;
        ///   - 3. the domain does not accept mail due to incorrect settings on the recipient's side or for other reasons;
        ///   - 4. the sender's server was rejected due to blacklisting;
        /// - permanent_unavailable - the address is permanently unavailable due to multiple non-deliveries;
        /// - complained - in one of the previous emails , the addressee clicked “This is spam”;
        /// - blocked - sending to this address is prohibited by the administration of Unisender Go.
        /// - Other statuses may appear in the future.
        /// </remarks>
        [JsonProperty("failed_emails")]
        public IReadOnlyList<string>? FailedEmails { get; set; }

        /// <summary>
        /// Error message in English.Required if <see cref="Status"/> is "error".
        /// </summary>
        [JsonProperty("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Error code. Required if <see cref="Status"/> is "error".
        /// </summary>
        [JsonProperty("code")]
        public int? Code { get; set; }
    }
}
