// =============================================
// FILE: ItemListModel\Exceptions\AppExceptions.cs
// PROJECT: FoodOrderingSystemModel  ← PUT IT HERE
//
// WHY HERE? The Model project is referenced by BOTH
// the DAL project AND the MVC project.
// So all layers can throw/catch these without circular references.
//
// Reference chain:
//   MVC  → references Model ✓
//   DAL  → references Model ✓
//   Both can use these exceptions ✓
// =============================================

using System;

namespace ItemListModel.Exceptions
{
    // Thrown by DAL when SQL Server / stored proc fails
    // → GlobalMvcExceptionFilter logs to FILE
    public class DataAccessException : Exception
    {
        public string StoredProcedure { get; }

        public DataAccessException(string message, string storedProcedure, Exception inner)
            : base(message, inner)
        {
            StoredProcedure = storedProcedure;
        }
    }

    // Thrown by Service when a business rule is broken
    // → GlobalMvcExceptionFilter logs to DB (ErrorLogs table)
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }

        public BusinessException(string message, string errorCode = "BUSINESS_ERROR")
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    // Thrown when a requested record doesn't exist
    // → No log, just show "not found" to user
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}