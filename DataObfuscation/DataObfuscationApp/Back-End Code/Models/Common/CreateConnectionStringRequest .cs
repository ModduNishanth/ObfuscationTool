namespace DataObfuscationApp.Model
{
    public class CreateConnectionStringRequest
    {
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? ConnectionString { get; set; }
        public string Datatype { get; set; }
        public int? TestConnection { get; set; }

    }
}
