namespace SchoolWebApp.API.Utils
{
    public static class HandleExceptions
    {
        public static string GetMessageForInnerExceptions(Exception ex)
        {
            var messages = new List<string>();
            do
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            while (ex != null);
            var message = string.Join(" - ", messages);
            return message;
        }

    }
}
