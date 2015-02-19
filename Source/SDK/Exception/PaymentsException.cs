using System.Text;
using Newtonsoft.Json;
using PayPal.Api;

namespace PayPal
{
    public class PaymentsException : HttpException
    {
        /// <summary>
        /// Gets a <see cref="PayPal.Api.Error"/> JSON object containing the parsed details of the Payments error.
        /// </summary>
        public Error Details { get; private set; }

        /// <summary>
        /// Copy constructor that attempts to deserialize the response from the specified <paramref name="ex"/>.
        /// </summary>
        /// <param name="ex">Originating <see cref="PayPal.HttpException"/> object that contains the details of the exception.</param>
        public PaymentsException(HttpException ex) : base(ex)
        {
            if (!string.IsNullOrEmpty(this.Response))
            {
                this.Details = JsonFormatter.ConvertFromJson<Error>(this.Response);

                // Set base System.Exception information.
                this.HelpLink = this.Details.information_link;
                this.Data["ErrorName"] = this.Details.name;
                this.Data["ErrorMessage"] = this.Details.message;
                this.Data["HelpLink"] = this.Details.information_link;
                this.Data["DebugId"] = this.Details.debug_id;

                // Log the error details
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine("   Error:    " + this.Details.name);
                sb.AppendLine("   Message:  " + this.Details.message);
                sb.AppendLine("   URI:      " + this.Details.information_link);
                sb.AppendLine("   Debug ID: " + this.Details.debug_id);

                if (this.Details.details != null)
                {
                    int count = 0;
                    foreach (ErrorDetails errorDetails in this.Details.details)
                    {
                        var message = string.Format("{0} -> {1}", errorDetails.field, errorDetails.issue);
                        sb.AppendLine("   Details:  " + message);
                        this.Data["ErrorDetails_" + count++] = message;
                    }
                }
                this.LogMessage(sb.ToString());
            }
        }

        /// <summary>
        /// Gets the prefix to use when logging the exception information.
        /// </summary>
        protected override string ExceptionMessagePrefix { get { return "Payments Exception"; } }
    }
}
