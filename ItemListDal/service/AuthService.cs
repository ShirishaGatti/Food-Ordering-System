using ItemListDal;
using ItemListModel.Exceptions;
using ItemListModel.ViewModel;
using System;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace ItemListTask1.Service
{
    public interface IAuthService
    {
        void Register(RegisterViewModel vm);
        LoginViewModel Login(LoginViewModel vm);
    }

    public class AuthService : IAuthService
    {
        private readonly AuthRepository _repo;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;

        public AuthService()
        {
            _repo = new AuthRepository();
        }

        // ===========================
        // Register
        // ===========================

        public void Register(RegisterViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Name))
                throw new BusinessException("Name is required.", "NAME_REQUIRED");

            if (string.IsNullOrWhiteSpace(vm.Email) || !IsValidEmail(vm.Email))
                throw new BusinessException("A valid email is required.", "INVALID_EMAIL");

            if (string.IsNullOrWhiteSpace(vm.Password) || vm.Password.Length < 8)
                throw new BusinessException("Password must be at least 8 characters.", "WEAK_PASSWORD");

            if (vm.RoleId < 1 || vm.RoleId > 3)
                throw new BusinessException("Invalid role selected.", "INVALID_ROLE");

            // Hash password
            string hash = BCrypt.Net.BCrypt.HashPassword(vm.Password, 12);

            _repo.Register(vm, hash);
        }

        // ===========================
        // Login
        // ===========================

        public LoginViewModel Login(LoginViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Email) ||
                string.IsNullOrWhiteSpace(vm.Password))
            {
                throw new BusinessException(
                    "Email and password are required.",
                    "MISSING_CREDENTIALS");
            }

            var cred = _repo.GetByEmail(vm.Email);

            if (cred == null)
            {
                throw new BusinessException(
                    "Invalid email or password.",
                    "INVALID_CREDENTIALS");
            }

            if (!cred.IsActive)
            {
                throw new BusinessException(
                    "Your account has been deactivated.",
                    "ACCOUNT_INACTIVE");
            }

            // Check account lock
            if (cred.IsLocked)
            {
                if (cred.LockedUntil.HasValue &&
                    cred.LockedUntil.Value > DateTime.UtcNow)
                {
                    throw new BusinessException(
                        string.Format(
                            "Account locked. Try again after {0:HH:mm}.",
                            cred.LockedUntil.Value),
                        "ACCOUNT_LOCKED");
                }

                // Lock expired
                _repo.UpdateLoginState(
                    cred.CredentialId,
                    0,
                    false);

                cred.IsLocked = false;
                cred.FailedLoginCount = 0;
            }
            string hash = cred.PasswordHash;
            // Verify password
            bool passwordOk = BCrypt.Net.BCrypt.Verify(
                vm.Password,
                cred.PasswordHash);

            if (!passwordOk)
            {
                int newFails = cred.FailedLoginCount + 1;
                bool lockNow = newFails >= MaxFailedAttempts;

                _repo.UpdateLoginState(
                    cred.CredentialId,
                    newFails,
                    lockNow,
                    lockNow
                        ? DateTime.UtcNow.AddMinutes(LockoutMinutes)
                        : (DateTime?)null);

                string msg;

                if (lockNow)
                {
                    msg = string.Format(
                        "Too many failed attempts. Account locked for {0} minutes.",
                        LockoutMinutes);
                }
                else
                {
                    msg = "Invalid email or password.";
                }

                throw new BusinessException(
                    msg,
                    "INVALID_CREDENTIALS");
            }

            // Reset failed attempts
            _repo.UpdateLoginState(
                cred.CredentialId,
                0,
                false,
                null,
                DateTime.UtcNow);

            // Get profile id based on role
            int profileId = 0;

            switch (cred.RoleId)
            {
                case 1:
                    profileId = cred.UserId.HasValue
                        ? cred.UserId.Value
                        : 0;
                    break;

                case 2:
                    profileId = cred.RestaurantId.HasValue
                        ? cred.RestaurantId.Value
                        : 0;
                    break;

                case 3:
                    profileId = cred.AdminId.HasValue
                        ? cred.AdminId.Value
                        : 0;
                    break;

                default:
                    profileId = 0;
                    break;
            }

            LoginViewModel result = new LoginViewModel();

            result.Success = true;
            result.RoleId = cred.RoleId;
            result.ProfileId = profileId;
            result.Email = cred.Email;

            return result;
        }

        // ===========================
        // Helpers
        // ===========================

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
    }
}