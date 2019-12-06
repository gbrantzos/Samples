namespace Sandbox
{
    public enum EmailType
    {
        Personal,
        Work,
        Other
    }

    public class ContactInfo
    {
        public int ID { get; set; }
        public EmailType EmailType { get; set; }
        public string Email { get; set; }
    }
}