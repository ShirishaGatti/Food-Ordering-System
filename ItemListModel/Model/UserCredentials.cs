using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemListModel.Model;
namespace ItemListModel.Model
{
    public class UserCredentials
        {
            public int CredentialId { get; set; }
            public int RoleId { get; set; }          // 1=User, 2=Restaurant, 3=Admin
            public int? UserId { get; set; }
            public int? RestaurantId { get; set; }
            public int? AdminId { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public bool IsActive { get; set; }
            public bool IsLocked { get; set; }
            public int FailedLoginCount { get; set; }
            public DateTime? LockedUntil { get; set; }
            public DateTime? LastLoginAt { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
