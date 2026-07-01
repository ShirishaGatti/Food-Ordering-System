using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemListDal.dal;
using ItemListModel.Exceptions;
using ItemListModel.Model;
using ItemListModel.ViewModel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ItemListDal
{
    public class AuthRepository : IAuthRepository
    {
        private readonly Database _db;

        public AuthRepository()
        {
            _db = DatabaseFactory.CreateDatabase();
        }

        public void Register(RegisterViewModel vm, string passwordHash)
        {
            try
            {
                DbCommand com = _db.GetStoredProcCommand("Auth_Register");
                _db.AddInParameter(com, "@RoleId", DbType.Int32, vm.RoleId);
                _db.AddInParameter(com, "@Name", DbType.String, vm.Name);
                _db.AddInParameter(com, "@Email", DbType.String, vm.Email);
                _db.AddInParameter(com, "@PasswordHash", DbType.String, passwordHash);
                _db.AddInParameter(com, "@Contact", DbType.String,
                    string.IsNullOrEmpty(vm.Contact) ? (object)DBNull.Value : vm.Contact);
                _db.AddInParameter(com, "@Address", DbType.String,
                    string.IsNullOrEmpty(vm.Address) ? (object)DBNull.Value : vm.Address);
                _db.AddInParameter(com, "@CityId", DbType.Int32,
                    vm.CityId > 0 ? (object)vm.CityId : DBNull.Value);
          
                _db.AddInParameter(com, "@DateOfBirth", DbType.DateTime,
                   vm.DateOfBirth.HasValue ? 
                   (object)vm.DateOfBirth.Value : DBNull.Value);

                _db.ExecuteNonQuery(com);
            }
            catch (SqlException ex) when (ex.Message.Contains("EMAIL_EXISTS"))
            {
                // SP raised EMAIL_EXISTS — treat as business-level duplicate
                throw new BusinessException("An account with this email already exists.", "EMAIL_EXISTS");
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Registration failed.", "Auth_Register", ex);
            }
        }

        public UserCredentials GetByEmail(string email)
        {
            try
            {
                DbCommand com = _db.GetStoredProcCommand("Auth_Login");
                _db.AddInParameter(com, "@Email", DbType.String, email);
                DataSet ds = _db.ExecuteDataSet(com);

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return null;

                var row = ds.Tables[0].Rows[0];
                return new UserCredentials
                {
                    CredentialId = Convert.ToInt32(row["CredentialId"]),
                    RoleId = Convert.ToInt32(row["RoleId"]),
                    Email = row["Email"].ToString(),
                    PasswordHash = row["PasswordHash"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    IsLocked = Convert.ToBoolean(row["IsLocked"]),
                    FailedLoginCount = Convert.ToInt32(row["FailedLoginCount"]),
                    LockedUntil = row["LockedUntil"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["LockedUntil"]) : null,
                    UserId = row["UserId"] != DBNull.Value ? (int?)Convert.ToInt32(row["UserId"]) : null,
                    RestaurantId = row["RestaurantId"] != DBNull.Value ? (int?)Convert.ToInt32(row["RestaurantId"]) : null,
                    AdminId = row["AdminId"] != DBNull.Value ? (int?)Convert.ToInt32(row["AdminId"]) : null
                };
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to fetch credentials.", "Auth_Login", ex);
            }
        }

        public void UpdateLoginState(int credentialId, int failedCount, bool isLocked,
                                     DateTime? lockedUntil = null, DateTime? lastLoginAt = null)
        {
            try
            {
                DbCommand com = _db.GetStoredProcCommand("Auth_UpdateLoginState");
                _db.AddInParameter(com, "@CredentialId", DbType.Int32, credentialId);
                _db.AddInParameter(com, "@FailedLoginCount", DbType.Int32, failedCount);
                _db.AddInParameter(com, "@IsLocked", DbType.Boolean, isLocked);
                _db.AddInParameter(com, "@LockedUntil", DbType.DateTime,
                    lockedUntil.HasValue ? (object)lockedUntil.Value : DBNull.Value);
                _db.AddInParameter(com, "@LastLoginAt", DbType.DateTime,
                    lastLoginAt.HasValue ? (object)lastLoginAt.Value : DBNull.Value);

                _db.ExecuteNonQuery(com);
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to update login state.", "Auth_UpdateLoginState", ex);
            }
        }
    }
}