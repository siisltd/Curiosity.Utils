namespace Curiosity.Tools.Web.ReCaptcha
{
    internal class ReCaptchaResponse
    {
        public bool success { get; set; }

        public long challange_ts  { get; set; }

        public string hostname  { get; set; } = null!;
    }
}