namespace UserAPI.Application.Common
{
    public static class ErrorMessages
    {
        public static class Questions
        {
            public const string Required = "La pregunta es requerida";
            public const string MustHaveFourOptions = "La pregunta debe contar con 4 respuestas";
            public const string MustHaveSubject = "La pregunta debe pertenecer a una materia";
            public const string CannotHaveMoreThanFourOptions = "La pregunta no puede contar con más de 4 respuestas";

            public const string Create = "Unexpected error during create question with options";
            public const string Update = "Unexpected error during updating question with options";
            public const string Delete = "Unexpected error during deleting question with options";
            public const string GetById = "Unexpected error during get question with options by question id";
            public const string GetPage = "Unexpected error during get question with options page";
            public const string GetRandom = "Unexpected error during get random questions";
        }

        public static class Scores
        {
            // Mismos mensajes que encriptacion.insert_score
            public const string NotMixedWithoutSubject = "No puede guardarse el score si no hay materia asignada y el modo de juego no es mixed";
            public const string MixedWithSubject = "No puede guardarse el score del modo mixed con una materia asignada";

            public const string Create = "Unexpected error during create score";
            public const string GetGeneralProgress = "Unexpected error during getting general progress";
            public const string GetLatestScoresPage = "Unexpected error during getting scores paged";
            public const string GetUserGlobalProgress = "Unexpected error during getting user global progress";
        }

        public static class Clients
        {
            public const string Create = "Unexpected error during insertion";
            public const string Update = "Unexpected error during update";
            public const string Delete = "Unexpected error during delete";
            public const string GetByClave = "Unexpected error during get";
            public const string GetAll = "Unexpected error during get clients";
            public const string GetPage = "Unexpected error during get clients";
        }

        public static class Subjects
        {
            public const string Create = "Unexpected error during create subject";
            public const string Update = "Unexpected error during update subject";
            public const string Delete = "Unexpected error during deleting subject";
            public const string GetForOptions = "Unexpected error during getting subject questions for options";
            public const string GetWithQuestionsCount = "Unexpected error during getting subject questions count";
        }

        public static class Users
        {
            public const string InvalidFormat = "Formato inválido para la creación de usuario";
            public const string InvalidCredentials = "Las credenciales son incorrectas";

            public const string Create = "An error occurs during register user";
            public const string Update = "An error occurs during updating user";
            public const string Delete = "An error occurs during delete user";
            public const string GetById = "An error occurs during getting user";
            public const string GetAll = "An error occurs during getting users";
            public const string Login = "An error occurs during login";
        }
    }
}
