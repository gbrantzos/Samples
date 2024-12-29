using System.Collections.ObjectModel;

namespace EFCoreSample;

public class Person
{
    public int ID { get; private set; }
    public string Name { get; private set; }

    public EmailAddresses Addresses { get; set; } = new EmailAddresses();

    public Person(string name) => Name = name;
}

public class EMail
{
    public int ID { get; private set; }
    public string Address { get; private set; } = String.Empty;
    public EmailType Type { get; private set; }

    public EMail(string address, EmailType type = EmailType.Default)
    {
        Address = address;
        Type = type;
    }
}

public enum EmailType
{
    Default,
    Work,
    Other
}

public class EmailAddresses : Collection<EMail>
{
    
}