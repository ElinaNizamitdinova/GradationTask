namespace UserService.Utils
{
    public static class ExceptionUtils
    {
        public static Exception GetMostInnerException(Exception ex)
        {
            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
            }

            return currentEx;
        }
    }
}
