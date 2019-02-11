using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    /// <summary>
    /// Klasse um die eingelesene Configdatei darzustellen und zu verwenden.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Variablen um die aus der Datei eingelesenen Daten zu sichern.
        /// </summary>
        public string password, username, domain, exUri;
        /// <summary>
        /// true => es werden alle alten Termine gelöscht.
        /// </summary>
        public bool delOldAppo;
        /// <summary>
        /// Liste von configMailboxes die synchronisiert werden sollen
        /// </summary>
        public List<configMailboxes> mailboxes = new List<configMailboxes>();

        /// <summary>
        /// Fügt ein neues Eintrag zur Liste der configMailboxes hinzu. Für jeden Datensatz aus der Configdatei wird ein Eintrag in dieser Liste erstellt.<br/>
        /// <b>Beispieleintrag in Configdatei</b><br/>
        /// <i>beispiel@arges.de; true; true; Arges Intern; Arges Kontakte</i>
        /// </summary>
        /// <param name="smtpAdresse">Die SMTP Adresse des Postfaches</param>
        /// <param name="birthday">Ob die Geburtstage erstellt werden sollen.</param>
        /// <param name="anniversary">Ob die Jahrestage erstellt werden sollen.</param>
        /// <param name="folder">Die Ordner die synchronisiert werden sollen</param>
        public void AddMailbox(string smtpAdresse, bool birthday, bool anniversary, List<string> folder)
        {
            mailboxes.Add(new configMailboxes(smtpAdresse, birthday, anniversary, folder));
        }
    }


    /// <summary>
    /// Klasse um die einzelnen Datensätze der eingelesene Configdatei darzustellen und zu verwenden.
    /// </summary>
    public class configMailboxes
    {
        /// <summary>
        /// Die SMTP Adresse des Postfaches
        /// </summary>
        public string smtpAdresse;
        /// <summary>
        /// Ob die Geburtstage und/oder Jahrestage erstellt werden sollen.
        /// </summary>
        public bool birthday, anniversary;
        /// <summary>
        /// Die Ordner die synchronisiert werden sollen
        /// </summary>
        public List<string> folder = new List<string>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="smtpAdresse">Die SMTP Adresse des Postfaches</param>
        /// <param name="birthday">Ob die Geburtstage erstellt werden sollen.</param>
        /// <param name="anniversary">Ob die Jahrestage erstellt werden sollen.</param>
        /// <param name="folder">Die Ordner die synchronisiert werden sollen</param>
        public configMailboxes(string smtpAdresse, bool birthday, bool anniversary, List<string> folder)
        {
            this.smtpAdresse = smtpAdresse;
            this.birthday = birthday;
            this.anniversary = anniversary;
            this.folder = folder;
        }
    }
}
