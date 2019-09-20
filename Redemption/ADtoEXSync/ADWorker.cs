using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADtoEXSync
{
    public static class ADWorker
    {
        public static List<UserPrincipal> GetAllRelevantADUsers(string domain, string ou)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain, ou);
            UserPrincipal qbeUser = new UserPrincipal(ctx);
            PrincipalSearcher srch = new PrincipalSearcher(qbeUser);



            List<UserPrincipal> allUsers = new List<UserPrincipal>();
            // find all matches
            foreach (var found in srch.FindAll())
            {
                UserPrincipal user = found as UserPrincipal;

                var countOU = 0;
                foreach (var item in user.DistinguishedName.Split(','))
                    countOU += (item.StartsWith("OU=")) ? 1 : 0;

                if (countOU == 1 && user.Surname != null && user.Enabled == true) allUsers.Add(user);
            }

            return allUsers;
        }
    }
}
