namespace UserAPI.Application.Common
{
    public static class DatabaseErrorCodes
    {
        // RAISE EXCEPTION sin ERRCODE
        public const string RaiseException = "P0001";
        // ERRCODE usado por las funciones de encriptacion; la API lo responde como 404
        public const string BusinessRule = "P0409";
    }
}
