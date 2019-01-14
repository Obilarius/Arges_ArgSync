using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public class Config
    {
        public string password, username, domain, exUri;
        public List<configMailboxes> mailboxes = new List<configMailboxes>();

        public void AddMailbox(string v1, string v2)
        {
            mailboxes.Add(new configMailboxes(v1, v2));
        }

        public void AddMailbox(string v1, string v2, bool disable)
        {
            mailboxes.Add(new configMailboxes(v1, v2, disable));
        }
    }

    public class configMailboxes
    {
        public string smtpAdresse, folder;
        public bool disable;

        public configMailboxes(string smtpAdresse, string folder, bool disable = false)
        {
            this.smtpAdresse = smtpAdresse;
            this.folder = folder;
            this.disable = disable;
        }
    }
}
