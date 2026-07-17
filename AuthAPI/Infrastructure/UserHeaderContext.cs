using System.Globalization;

namespace AuthAPI.Infrastructure
{
    public static class UserHeaderContext
    {
        public const string UserIdHeader = "X-User-Id";
        public const string UserNameHeader = "X-User-Name";
        public const string UserEmailHeader = "X-User-Email";
        public const string UserWeightHeader = "X-User-Weight";
        public const string UserTrainingExperienceHeader = "X-User-TrainingExperience";
        public const string UserCaloricTargetHeader = "X-User-CaloricTarget";

        public static string? GetHeader(IHttpContextAccessor httpContextAccessor, string headerName)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            return httpContext.Request.Headers.TryGetValue(headerName, out var value)
                ? value.ToString()
                : null;
        }

        public static int? GetUserId(IHttpContextAccessor httpContextAccessor)
        {
            var headerValue = GetHeader(httpContextAccessor, UserIdHeader);
            return int.TryParse(headerValue, out var userId) ? userId : null;
        }

        public static decimal? GetDecimal(IHttpContextAccessor httpContextAccessor, string headerName)
        {
            var headerValue = GetHeader(httpContextAccessor, headerName);
            return decimal.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
                ? value
                : null;
        }
    }
}