namespace ChatApp.Web.ViewModels
{
    public class ResVM<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T? data { get; set; }

    }
}
