using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Testing
{
    class Program
    {
        public static List<Customer> customerList;

        static void Main(string[] args)
        {

            string path = "Liste.xml";

            if (File.Exists(path))
            {
                try
                {
                    // READ XML
                    customerList = (
                        from e in XDocument.Load("test2.xml").Root.Elements("MatchingEntry")
                        select new Customer
                        {
                            MailboxId = (string)e.Element("MailboxId"),
                            PublicId = (string)e.Element("PublicId"),
                            Subject = (string)e.Attribute("Subject")
                        }
                    ).ToList();

                    foreach (var item in customerList)
                    {
                        Console.WriteLine(item.Subject + " - " + item.PublicId);
                    }

                    //WRITE XML

                    //var users = new List<Customer>();
                    //users.Add(new Customer("Sascha", "public", "mailbox"));
                    //users.Add(new Customer("Peter", "public2", "mailbox2"));
                    //users.Add(new Customer("Klaus", "public3", "mailbox3"));
                    //users.Add(new Customer("Seppl", "public4", "mailbox4"));

                    //var doc = new XDocument();

                    //doc.Add(new XElement("MatchingList", users.Select(x => new XElement("MatchingEntry",
                    //        new XAttribute("Subject", x.Subject),
                    //        new XElement("PublicId", x.PublicId),
                    //        new XElement("MailboxId", x.MailboxId)))));

                    //doc.Save("test2.xml");
                    //Console.Write("XML File created");
                    //Console.ReadKey();


                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }

            

            

            Console.ReadKey();
        }
    }

    class Customer
    {
        public string MailboxId { get; set; }
        public string PublicId { get; set; }
        public string Subject { get; set; }

        // Konstruktoren 
        public Customer() { }

        public Customer(string subject, string publicId, string mailboxId)
        {
            this.Subject = subject;
            this.PublicId = publicId;
            this.MailboxId = mailboxId;
        }
    }



    //class Entry : IEquatable<Entry>
    //{
    //    public string PublicId { get; set; }

    //    public string MailboxId { get; set; }

    //    public override string ToString()
    //    {
    //        return "PublicId: " + PublicId + "   MailboxId: " + MailboxId;
    //    }
    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null) return false;
    //        Entry objAsPart = obj as Entry;
    //        if (objAsPart == null) return false;
    //        else return Equals(objAsPart);
    //    }

    //    public bool Equals(Entry other)
    //    {
    //        if (other == null) return false;
    //        return (this.PublicId.Equals(other.PublicId) && this.MailboxId.Equals(other.MailboxId));
    //    }
    //}
}
