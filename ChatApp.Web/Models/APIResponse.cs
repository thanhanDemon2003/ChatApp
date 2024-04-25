using System.Net;

namespace ChatApp.Web.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            Notification = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> Notification { get; set; }
        public object Result { get; set; }
    }
}
