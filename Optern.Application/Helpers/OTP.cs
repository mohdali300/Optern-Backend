using Microsoft.AspNetCore.Http;
using Optern.Application.DTOs.Mail;
using Optern.Application.Response;
using Optern.Infrastructure.ExternalServices.MailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Helpers
{
     public class OTP
      {
        private  readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        public OTP(IHttpContextAccessor httpContextAccessor, IMailService _mailService) 
        {
            _httpContextAccessor = httpContextAccessor; 
            this._mailService = _mailService;
        
        }

        public bool IsOtpValid(string key)
        {
            var combinedValue = _httpContextAccessor.HttpContext.Request.Cookies[key];

            if (string.IsNullOrEmpty(combinedValue))
            {
                return false;
            }

            var parts = combinedValue.Split('|');
            if (parts.Length != 2)
            {
                return false;
            }

            var otp = parts[0];
            var expirationTime = parts[1];

            if (!DateTime.TryParse(expirationTime, out var expiryDateTime))
            {
                return false;
            }

            if (DateTime.UtcNow > expiryDateTime)
            {
                return false;
            }

            return true;
        }

        public async Task<Response<bool>> SendRegisterationOTPAsync(string email)
        {
            var otp = GenerateOTP.Generateotp();
            var expirationTime = DateTime.UtcNow.AddMinutes(10).ToString("o");
            var combinedValue = $"{otp}|{expirationTime}";

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(10)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("OtpRegister", combinedValue, cookieOptions);

            MailRequestDTO mail = new()
            {
                Email = email,
                Subject = "Verification OTP for Account Registration",
                Body = $@"
                       <html>
                       <body>
                       <p>Dear User,</p>

                       <p>Thank you for choosing to register with us. To complete your registration process, please use the one-time password (OTP) below:</p>

                       <p><strong>OTP: {otp}</strong></p>

                       <p><strong>Please note:</strong></p>
                       <ul>
                           <li>This OTP will expire in <strong>10 minutes</strong> from the time of receiving it.</li>
                           <li>Please ensure to use it before it expires.</li>
                       </ul>

                       <p>If you did not request this OTP, please ignore this email.</p>

                       <p>Best regards,</p>
                       <p>The Team</p>
                       </body>
                       </html>
                       "
            };

            var response = await _mailService.SendEmailAsync(mail);
            return response;
        }



    }
}
