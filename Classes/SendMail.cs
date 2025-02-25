using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace regin.Classes
{
    internal class SendMail
    {
        public static void SendMessage(string message, string to)
        {
            var smptClient = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                Credentials = new NetworkCredential("ron454kar@yandex.ru", "xgzqclbqmeuaautf"),
                EnableSsl = true,
            };
            smptClient.Send("ron454kar@yandex.ru", to, "Проект RegIn", message);
        }
    }
}
