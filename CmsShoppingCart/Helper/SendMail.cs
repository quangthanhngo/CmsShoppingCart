using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CmsShoppingCart.Helper
{
    public class SendMail
    {
        public static bool SendEmail(string to, string subject, string body, string attachFile)
        {
            try
            {
                MailMessage msg = new MailMessage("ksquangthanh69@gmail.com", to, subject, body);

                using(var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;

                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        Attachment attachment = new Attachment(attachFile);
                        msg.Attachments.Add(attachment);
                    }

                    NetworkCredential credential = new NetworkCredential("ksquangthanh69@gmail.com", "cebdbvctnngdlc");
                    client.UseDefaultCredentials = false;
                    client.Credentials = credential;
                    client.Send(msg);
                }

                
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
