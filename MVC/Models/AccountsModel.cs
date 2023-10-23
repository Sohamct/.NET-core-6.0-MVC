namespace MVC.Models
{
    public class AccountsModel
    {
        public int ID { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public byte[]? ProfilePhoto { get; set; }
    }
}
