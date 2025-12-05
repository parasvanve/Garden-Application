using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private const string TestUsername = "testuser";
    private const string TestPassword = "password123";

    // GET: Login Page
    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View("LoginPage");
    }

    // POST: Login
    [HttpPost]
    public IActionResult Login(string username, string password, string returnUrl)
    {
        if (username == TestUsername && password == TestPassword)
        {
            // CRITICAL FIX: Use "Username" not "UserSession"
            HttpContext.Session.SetString("Username", username);

            // Optional: Add additional session data
            HttpContext.Session.SetInt32("IsAuthenticated", 1);

            // Redirect to return URL or Calendar
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Calendar", "Home");
        }

        ViewBag.Error = "Invalid username or password.";
        return View("LoginPage");
    }

    // POST: Logout (should be POST for security)
    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();  // Clear all session data
        return RedirectToAction("Calendar", "Home");  // Go back to Calendar, not Login
    }

    // Optional: Add Register action
    public IActionResult Register()
    {
        return View();
    }
}