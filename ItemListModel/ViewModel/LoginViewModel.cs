using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel.ViewModel
{
    public class LoginViewModel
    {
        public string Username { get; set; }      // matches m.Username in Razor
        public string Password { get; set; }
        public string Role { get; set; }          // matches m.Role in Razor ("1","2","3")
        public bool RememberMe { get; set; }      // matches m.RememberMe in Razor

        // returned after successful login (not bound from form)
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int RoleId { get; set; }
        public int ProfileId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
