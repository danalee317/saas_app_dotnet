using System.Net;
using Azure;

namespace MultiFamilyPortal.Extensions
{
    internal static class StorageExceptionExtensions
    {
        public static bool IsAlreadyExistsException(this RequestFailedException e)
        {
            return e?.Status == (int?)HttpStatusCode.Conflict;
        }
    }
}
