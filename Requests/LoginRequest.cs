namespace TarefasKanBan.Requests
{
    public class LoginRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
    }
}