using System.Xml.Serialization;

namespace ContactManager.Application.ViewModels
{
    [XmlRoot("Contact")]
    public class ContactViewModel
    {
        [XmlElement("ID_Number")]
        public long ID { get; set; }
        public string FullName { get; set; }
    }
}
