using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Testing
{
    [XmlRoot("MatchingList")]
    [XmlInclude(typeof(ContactEntry))]
    public class MatchingList
    {
        [XmlArray("MatchingArray")]
        [XmlArrayItem("MatchingObjekt")]
        public List<ContactEntry> ContactEntry = new List<ContactEntry>();

        // Konstruktoren 
        public MatchingList() { }

        public void AddEntry(ContactEntry contactEntry)
        {
            ContactEntry.Add(contactEntry);
        }
    }


    [XmlType("ContactEntry")]
    public class ContactEntry
    {
        [XmlAttribute("Subjekt", DataType = "string")]
        public string Subjekt { get; set; }

        [XmlElement("PublicId")]
        public string PublicId { get; set; }

        [XmlElement("MailboxId")]
        public string MailboxId { get; set; }

        // Konstruktoren 
        public ContactEntry() { }

        public ContactEntry(string subject, string publicId, string mailboxId)
        {
            this.Subjekt = subject;
            this.PublicId = publicId;
            this.MailboxId = mailboxId;
        }
    }
}
