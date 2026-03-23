using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages;

public class ErrorPageModel : PageModel
{
    public string? RequestId { get; private set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet()
    {
        RequestId = HttpContext.TraceIdentifier;
    }
}
