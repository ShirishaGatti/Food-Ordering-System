using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel.ViewModel
{
    public class RegisterViewModel
    {

        public int CityId { get; set; } = 1;
        public int RoleId { get; set; } = 1;
        public string Role { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
    }
}
