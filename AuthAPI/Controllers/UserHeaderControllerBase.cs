using AuthAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    public abstract class UserHeaderControllerBase : ControllerBase
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;

        protected UserHeaderControllerBase(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        protected int? GetUserId() => UserHeaderContext.GetUserId(HttpContextAccessor);

        protected string? GetHeader(string headerName) => UserHeaderContext.GetHeader(HttpContextAccessor, headerName);

        protected decimal? GetDecimal(string headerName) => UserHeaderContext.GetDecimal(HttpContextAccessor, headerName);
    }
}