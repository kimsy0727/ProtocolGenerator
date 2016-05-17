using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Common;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace Common
{
    public class PingChecker
    {
        private String mXmlPath;
        private XMLManager mXmlManager;
        private FileManager mFileManager;
        private String mPath;

        private String mMailServerAdress;
        private String mMailServerPassword;
        private String mMailServerHost;
        private int mMailServerPort;

        private Dictionary<String, String> mServerList;
        private Dictionary<String, String> mMailList;
        
        public PingChecker(String path)
        {
            mPath = path;
            mXmlPath = mPath + "\\config.xml";
            mServerList = new Dictionary<string, string>();
            mMailList = new Dictionary<string, string>();
        }

        public Boolean Init()
        {
            try
            {
                // 로그파일 기록을 위한 변수
                mFileManager = new FileManager(mPath+"\\PingChecker.log");
                // xml 데이터를 dataSet 형태로 가져온다.
                mXmlManager = new XMLManager(mXmlPath);
                DataSet dataSet = mXmlManager.Load();
                if (dataSet == null)
                {
                    mFileManager.AppendLog("XMLManager::Load() return false. " + mXmlPath);
                    return false;
                }

                DataTable serverTable = dataSet.Tables["server_list"];
                // 서버 목록을 파싱해서 컨테이너에 담자
                foreach (DataRelation relation in serverTable.ChildRelations)
                {
                    // relation 에 해당하는 row 들을 루프돌자
                    foreach (DataRow row in serverTable.Rows[0].GetChildRows(relation.ToString()))
                    {
                        mServerList.Add(row["name"].ToString(), row["address"].ToString());
                    }
                }

                DataTable mailTable = dataSet.Tables["mail_list"];
                //알림 메일 목록을 파싱해서 컨테이너에 담자
                foreach (DataRelation relation in mailTable.ChildRelations)
                {
                    // relation 에 해당하는 row 들을 루프돌자
                    foreach (DataRow row in mailTable.Rows[0].GetChildRows(relation.ToString()))
                    {
                        mMailList.Add(row["name"].ToString(), row["address"].ToString());
                    }
                }

                // 알림을 위한 관리자 계정을 저장하자
                DataTable adminTable = dataSet.Tables["admin_account"];
                mMailServerAdress = adminTable.Rows[0]["address"].ToString();
                mMailServerPassword = adminTable.Rows[0]["password"].ToString();
                mMailServerHost = adminTable.Rows[0]["host"].ToString();
                mMailServerPort = Convert.ToInt32(adminTable.Rows[0]["port"]);

                return true;
            }
            catch (Exception e)
            {
                mFileManager.AppendLog(e.ToString());
                return false;
            }
        }

        public void CheckAll()
        {
            Ping ping = new Ping();
            // 컨테이너 루프돌면서 핑체크하자
            foreach (KeyValuePair<String, String> row in mServerList)
            {
                PingReply reply = ping.Send(row.Value);
                if (reply.Status == IPStatus.Success)
                {
                    mFileManager.AppendLog("[SYSTEM] success ping check. => ServerName:"+row.Key+", IPAddress:"+row.Value+", RoundTripTime:"+reply.RoundtripTime);
                }
                else
                {
                    String msg = "[SYSTEM] failed ping check. => ServerName:" + row.Key + ", IPAddress:" + row.Value + ", Status:" + reply.Status.ToString() + " ";
                    mFileManager.AppendLog(msg);
                    // 실패했으면 등록된 메일주소로 알리자
                    SendAll(msg, "system message");
                }
            }

        }

        private void SendAll(String subject, String body)
        {
            try
            {
                foreach (KeyValuePair<String, String> row in mMailList)
                {
                    MailMessage mail = new MailMessage(mMailServerAdress, row.Value);
                    SmtpClient client = new SmtpClient();
                    client.Port = mMailServerPort;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = mMailServerHost;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(mMailServerAdress, mMailServerPassword);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.Priority = MailPriority.High;
                    client.Send(mail);
                }
            }
            catch (Exception e)
            {
                mFileManager.AppendLog(e.ToString());
            }
        }
    }
}
