namespace Authentication_Authorization.Utilities
{
    public class ApiReturnObj<T> where T : class
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public T? Response { get; set; }

    }
}
