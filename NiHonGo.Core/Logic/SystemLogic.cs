using NiHonGo.Core.DTO.System;
using NiHonGo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace NiHonGo.Core.Logic
{
    public class SystemLogic : _BaseLogic
    {
        /// <summary>
        /// SystemLogic Logic
        /// </summary>
        public SystemLogic() : base() { }

        public void DBDataInit()
        {
            NiHonGoContext.Levels.Add(new Level { Display = "無經驗" });
            NiHonGoContext.Levels.Add(new Level { Display = "初學者" });
            NiHonGoContext.Levels.Add(new Level { Display = "日檢5級" });
            NiHonGoContext.Levels.Add(new Level { Display = "日檢4級" });
            NiHonGoContext.Levels.Add(new Level { Display = "日檢3級" });
            NiHonGoContext.Levels.Add(new Level { Display = "日檢2級" });
            NiHonGoContext.Levels.Add(new Level { Display = "日檢1級" });
            NiHonGoContext.Levels.Add(new Level { Display = "商用程度" });

            NiHonGoContext.SaveChanges();
        }

        public List<ListItem> GetLevels()
        {
            return NiHonGoContext.Levels.Select(r => new ListItem
            {
                Display = r.Display,
                Value = r.Id.ToString()
            }).ToList();
        }

        public bool SendMail(string smtpServer, string mailFrom, string mailTo, string mailSub, string mailBody)
        {
            try
            {
                if (string.IsNullOrEmpty(mailFrom))
                    return false;

                if (string.IsNullOrEmpty(smtpServer))
                    return false;

                if (string.IsNullOrEmpty(mailTo))
                    return false;

                if (string.IsNullOrEmpty(mailSub))
                    return false;

                if (string.IsNullOrEmpty(mailBody))
                    return false;

                //建立MailMessage物件
                var mms = new MailMessage();

                //指定一位寄信人MailAddress
                mms.From = new MailAddress(mailFrom.Trim());

                //信件主旨
                mms.Subject = mailSub;

                //信件內容
                mms.Body = mailBody;

                //信件內容 是否採用Html格式
                mms.IsBodyHtml = true;

                //加入信件的收信人address
                mms.To.Add(new MailAddress(mailTo.Trim()));

                using (SmtpClient client = new SmtpClient(smtpServer.Trim(), 25))//或公司、客戶的smtp_server
                {
                    client.Credentials = new NetworkCredential(mailFrom, "");//寄信帳密
                    client.Send(mms);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}