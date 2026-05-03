using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.Infrastructure.Utils
{
    public static class EmailTemplates
    {
        public static string GetOTPEmailTemplate(string otp, int expiryMinutes)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Password Reset OTP</title>
                </head>
                <body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
                    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <td style='padding: 40px 0; text-align: center; background-color: #f4f4f4;'>
                
                                <!-- Card -->
                                <table role='presentation' style='width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 20px rgba(0,0,0,0.1);'>
                    
                                    <!-- Header -->
                                    <tr>
                                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px; text-align: center;'>
                                            <h1 style='margin: 0; color: #ffffff; font-size: 28px; font-weight: 700; letter-spacing: 1px;'>LIMTIC</h1>
                                            <p style='margin: 8px 0 0; color: rgba(255,255,255,0.85); font-size: 14px;'>Secure Password Reset</p>
                                        </td>
                                    </tr>

                                    <!-- Body -->
                                    <tr>
                                        <td style='padding: 50px 40px;'>
                                            <!-- Title -->
                                            <h2 style='margin: 0 0 12px; color: #1a1a2e; font-size: 22px; font-weight: 700; text-align: center;'>
                                                Password Reset Request
                                            </h2>
                                            <p style='margin: 0 0 30px; color: #666; font-size: 15px; line-height: 1.6; text-align: center;'>
                                                We received a request to reset your password. Use the OTP code below to proceed. 
                                                Do <strong>not</strong> share this code with anyone.
                                            </p>

                                            <!-- OTP Box -->
                                            <div style='background: linear-gradient(135deg, #f5f7ff 0%, #e8ecff 100%); border: 2px dashed #667eea; border-radius: 12px; padding: 30px; text-align: center; margin-bottom: 30px;'>
                                                <p style='margin: 0 0 8px; color: #888; font-size: 12px; text-transform: uppercase; letter-spacing: 2px;'>Your OTP Code</p>
                                                <p style='margin: 0; color: #667eea; font-size: 48px; font-weight: 800; letter-spacing: 16px; font-family: monospace;'>{otp}</p>
                                            </div>

                                            <!-- Expiry Warning -->
                                            <div style='background-color: #fff8e1; border-left: 4px solid #ffc107; border-radius: 4px; padding: 14px 18px; margin-bottom: 30px;'>
                                                <p style='margin: 0; color: #856404; font-size: 14px;'>
                                                     This code expires in <strong>{expiryMinutes} minutes</strong>. Please use it as soon as possible.
                                                </p>
                                            </div>

                                            <!-- Security Notice -->
                                            <div style='background-color: #fef2f2; border-left: 4px solid #ef4444; border-radius: 4px; padding: 14px 18px;'>
                                                <p style='margin: 0; color: #991b1b; font-size: 14px;'>
                                                     If you did not request a password reset, please ignore this email or contact support immediately.
                                                </p>
                                            </div>

                                        </td>
                                    </tr>

                                    <!-- Footer -->
                                    <tr>
                                        <td style='background-color: #f8f9fa; padding: 24px 40px; text-align: center; border-top: 1px solid #eee;'>
                                            <p style='margin: 0 0 6px; color: #aaa; font-size: 12px;'>This is an automated message, please do not reply.</p>
                                            <p style='margin: 0; color: #aaa; font-size: 12px;'>© {DateTime.UtcNow.Year} LIMTIC. All rights reserved.</p>
                                        </td>
                                    </tr>

                                </table>
                                <!-- End Card -->

                            </td>
                        </tr>
                    </table>
                </body>
                </html>";
        }
    }
}
