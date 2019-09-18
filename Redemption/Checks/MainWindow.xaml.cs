using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Checks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();



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

            Users.ItemsSource = allUsers;
        }
    }
}
