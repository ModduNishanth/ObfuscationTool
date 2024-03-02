namespace DataObfuscationApp.Model
{
    public class ResponseModel<T>
    {
       public bool IsError { get; set; }
        public string? Message { get; set; }
        public string[]? Data { get; set; }
    }
}
