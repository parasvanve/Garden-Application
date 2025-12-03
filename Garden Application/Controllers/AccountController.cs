using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    // ----------------------------------------------------------------------
    // HARDCODED CREDENTIALS - FOR TESTING ONLY
    // ----------------------------------------------------------------------
    private const string TestUsername = "testuser";
    private const string TestPassword = "password123";
    // ----------------------------------------------------------------------

    // GET: /Account/Login (Displays the login form)
    public IActionResult Login()
    {
        // No check for User.Identity.IsAuthenticated needed, 
        // as we are not using cookies for session management.
        return View("LoginPage");
    }

    // POST: /Account/Login (Processes the login form data)
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        // 1. SIMPLE LOGIC CHECK
        if (username == TestUsername && password == TestPassword)
        {
            // --- CHECK SUCCESS ---
            // If the check passes, we redirect to the "secured" page. 
            // NOTE: This page is NOT actually secured without cookies!
            return RedirectToAction("Calendar", "Home");
        }

        // --- CHECK FAILURE ---
        ViewBag.Error = "Invalid username or password.";

        // Return to the login page to display the error
        return View("LoginPage");
    }

    // GET: /Account/Logout is no longer needed since there is no session to destroy.
    // However, if the user navigates to it, we redirect them back to the login page.
    public IActionResult Logout()
    {
        // Since no cookie is set, simply redirecting to login is sufficient.
        return RedirectToAction("Login");
    }

    // GET: /Account/Register (Simple placeholder)
    public IActionResult Register()
    {
        // Assuming your register view file is named RegisterPage.cshtml
        return View("RegisterPage");
    }
}