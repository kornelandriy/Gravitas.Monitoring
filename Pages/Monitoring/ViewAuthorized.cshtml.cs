using Gravitas.Monitoring.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gravitas.Monitoring.Pages.Monitoring
{
    [Authorize(Roles = UserRoles.AdminUser)]
    public class ViewAuthorized : PageModel
    {
        public void OnGet()
        {
            
        }
    }
}