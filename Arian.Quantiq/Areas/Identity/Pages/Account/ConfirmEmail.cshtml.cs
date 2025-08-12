// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Arian.Quantiq.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Arian.Quantiq.Areas.Identity.Pages.Account;

public class ConfirmEmailModel(UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [TempData]
    public string StatusMessage { get; set; }

    [BindProperty]
    public string UserId { get; set; }

    [BindProperty]
    public string Code { get; set; }

    public IActionResult OnGet(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToPage("/Index");
        }

        UserId = userId;
        Code = code;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (UserId == null || Code == null)
        {
            return RedirectToPage("/Index");
        }

        var user = await _userManager.FindByIdAsync(UserId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserId}'.");
        }

        string decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
        StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

        return Page();
    }
}