using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class config
    {
        public string password, username, domain, exUri;
        public List<configMailboxes> mailboxes = new List<configMailboxes>();

        public void AddMailbox(string v1, string v2)
        {
            mailboxes.Add(new configMailboxes(v1, v2));
        }
    }

    public class configMailboxes
    {
        public string smtpAdresse, folder;

        public configMailboxes(string smtpAdresse, string folder)
        {
            this.smtpAdresse = smtpAdresse;
            this.folder = folder;
        }
    }
}
