using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADtoEXSync
{
    class Program
    {
        static void Main(string[] args)
        {
        }


        List<UserPrincipal> GetAllRelevantADUsers()
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, "ARGES", "OU=Arges_Intern,DC=arges,DC=local");
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

                if (countOU == 1 && user.Surname != null) allUsers.Add(user);
            }

            return allUsers;
        }
    }
}
