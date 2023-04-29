using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Site.Models
{
    public class Mailing
    {
        //SiteContext _context;

        public Mailing(/*SiteContext context*/)
        {
            //_context = context;
        }

        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
        [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
        int cbSize, [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
        int dwMimeFlags, out IntPtr ppwzMimeOut, int dwReserved);
        public static string GetMimeFromFile(string file)
        {
            IntPtr mimeout;
            if (!System.IO.File.Exists(file))
                throw new FileNotFoundException(file + " not found");
            int MaxContent = (int)new FileInfo(file).Length;
            if (MaxContent > 4096) MaxContent = 4096;
            FileStream fs = File.OpenRead(file);

            byte[] buf = new byte[MaxContent];
            fs.Read(buf, 0, MaxContent);
            fs.Flush();
            int result = FindMimeFromData(IntPtr.Zero, file, buf, MaxContent, null, 0, out mimeout, 0);
            if (result != 0)
                throw Marshal.GetExceptionForHR(result);
            string mime = Marshal.PtrToStringUni(mimeout);
            Marshal.FreeCoTaskMem(mimeout);
            return mime;
        }

        public bool SmtpMail(string Name, string Email, string Message, string ToName, string ToEmail, string Subject, Dictionary<string, object> Attachments)
        {
            var MimeMessage = new MimeMessage();
            MimeMessage.From.Add(new MailboxAddress(Name, Email));
            MimeMessage.To.Add(new MailboxAddress(ToName, ToEmail));
            MimeMessage.Subject = Subject;

            var builder = new BodyBuilder { TextBody = Message };

            //considering one or more attachments
            if (Attachments != null)
            {
                foreach (var Attachment in Attachments)
                {
                    string _contentType = GetMimeFromFile(Attachment.Value.ToString());
                    builder.Attachments.Add(Attachment.Key, File.ReadAllBytes(Attachment.Value.ToString()), ContentType.Parse(_contentType));
                }
            }
            MimeMessage.Body = builder.ToMessageBody();

            //MimeMessage.Body = new TextPart("plain")
            //{
            //    Text = Message
            //};

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.strato.com", 465, SecureSocketOptions.SslOnConnect);

                    // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
                    //client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.QueryCapabilitiesAfterAuthenticating = false;
                    client.Authenticate("info@lilodesign.nl", "w8w8w8w8");

                    client.Send(MimeMessage);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public string MailChimpSubscription(string Email)
        //{

        //    try
        //    {
        //        string apikey = "5cf2513d5a5f522f3e25c550d53302c1-us15";
        //        string list_id = "218a045e78";
        //        // Dim resp As String = ""

        //        //Dim request As WebRequest = WebRequest.Create("https://us11.api.mailchimp.com/2.0/?method=listSubscribe&output=xml&apikey=" & apikey & "&id=" & list_id & "&email_address=" & HttpContext.Current.Server.UrlEncode(sEmail))
        //        //request.Method = "POST"
        //        //Dim response As WebResponse = request.GetResponse()


        //        //MailChimpManager mc = new MailChimpManager(apikey);

        //        ////  Create the email parameter
        //        //EmailParameter email = new EmailParameter { Email = sEmail };

        //        //EmailParameter results = mc.Subscribe(list_id, email);

        //        //Dim xmlhttp = HttpContext.Current.Server.CreateObject("MSXML2.ServerXMLHTTP")
        //        //xmlhttp.Open("GET", "https://us11.api.mailchimp.com/2.0/?method=listSubscribe&output=xml&apikey=" & apikey & "&id=" & list_id & "&email_address=" & HttpContext.Current.Server.UrlEncode(sEmail), False) '& "&merge_vars="
        //        //xmlhttp.send()
        //        //resp = xmlhttp.statusText
        //        //xmlhttp = Nothing

        //        //If resp = "OK" Then
        //        //    Return True
        //        //Else
        //        //    Return False
        //        //End If
        //        // Dim s As String = ""
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //=======================================================
        //Service provided by Telerik (www.telerik.com)
        //Conversion powered by NRefactory.
        //Twitter: @telerik
        //Facebook: facebook.com/telerik
        //=======================================================

        //    return "";
        //}
    }
}
