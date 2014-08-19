﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace our.usercontrols
{
    public partial class Forgotpassword : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void sendPass(object sender, EventArgs e)
        {
            string em = tb_email.Text;

            umbraco.cms.businesslogic.member.Member m = umbraco.cms.businesslogic.member.Member.GetMemberFromEmail(em);

            if (m != null)
            {
                var pass = RandomString(8, true);
                m.Password = pass;
                m.Save();

                var email = "<p>Hi " + m.Text + "</p>";
                email = email + "<p>This is your new password for your account on http://our.umbraco.org:</p>";
                email = email + "<p><strong>" + pass + "</strong></p>";
                email = email + "<br/><br/><p>All the best<br/> <em>The email robot</em></p>";

                var mailMessage = new MailMessage
                {
                    Subject = string.Format("Your password to our.umbraco.org"),
                    Body = email,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(new MailAddress(m.Email));

                mailMessage.From = new MailAddress("robot@umbraco.org");

                var smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);

                msg.Visible = true;
                msg.Text = "<div class='notice'><p>A new password has been sent to the email address: <strong>" + em + "</strong></p><p>Go to the <a href='/member/login.aspx'>login page</a></div>";
            }

        }

        private string RandomString(int size, bool lowerCase) {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }

}