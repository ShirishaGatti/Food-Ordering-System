using ItemListModel.Model;
using ItemListModel.ViewModel;
using System;

namespace ItemListDal.dal
{
    public interface IAuthRepository
    {
        void Register(RegisterViewModel vm, string passwordHash);
        UserCredentials GetByEmail(string email);
        void UpdateLoginState(int credentialId, int failedCount, bool isLocked,
                              DateTime? lockedUntil = null, DateTime? lastLoginAt = null);
    }
}