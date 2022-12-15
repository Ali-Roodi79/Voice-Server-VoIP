using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TVS
{
    class Email
    {
        public void EMAIL (string pass , string UserEmail)
        {
            MailMessage PasswordRequest = new MailMessage();
            PasswordRequest.From = new MailAddress("teleschitvs@gmail.com");
            PasswordRequest.Subject = "TVS Password";
            PasswordRequest.Body = pass;
            PasswordRequest.To.Add(UserEmail);


          
            SmtpClient MailSender = new SmtpClient("smtp.gmail.com",587);           
            MailSender.Credentials = new NetworkCredential("teleschitvs@gmail.com", "TVS85NAM");
            MailSender.EnableSsl = true;
            MailSender.Send(PasswordRequest);
        }

    }
}
