// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Project_1640.Models;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Project_1640.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return Page();
                // Don't reveal that the user does not exist or is not confirmed
                //return RedirectToPage("./ForgotPasswordConfirmation");
            }


            // For more information on how to enable account confirmation and password reset please
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            Email emailData = new Email()
            {
                //Input email details
                From = "luandtgcs200115@fpt.edu.vn",
                Password = "Conso123!",
                Body = "Please reset your password by <a href="+ $"{ callbackUrl }" + "> click here</a>",
            };

            MimeMessage email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailData.From));
                email.To.Add(MailboxAddress.Parse(Input.Email));
                email.Subject = "Recovery Passsword";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData.Body };

            using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(emailData.From, emailData.Password);
                smtp.Send(email);
                smtp.Disconnect(true);

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
