using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserService.Utils
{
    public static class ControllerUtils
    {
        public static string GetModelErrors(this ModelStateDictionary modelState)
        {
            string allErrors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)));
            return allErrors;
        }
    }
}
