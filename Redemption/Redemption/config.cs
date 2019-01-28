using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class Config
    {
        public string password, username, domain, exUri;
        public bool delOldAppo;
        public List<configMailboxes> mailboxes = new List<configMailboxes>();

        public void AddMailbox(string smtpAdresse, bool birthday, bool anniversary, List<string> folder)
        {
            mailboxes.Add(new configMailboxes(smtpAdresse, birthday, anniversary, folder));
        }
    }

    public class configMailboxes
    {
        public string smtpAdresse;
        public bool birthday, anniversary;
        public List<string> folder = new List<string>();

        public configMailboxes(string smtpAdresse, bool birthday, bool anniversary, List<string> folder)
        {
            this.smtpAdresse = smtpAdresse;
            this.birthday = birthday;
            this.anniversary = anniversary;
            this.folder = folder;
        }
    }
}
