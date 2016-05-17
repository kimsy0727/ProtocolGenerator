using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Web.Mvc;

namespace Common
{
    class NetPeer
    {
        // 웹 위한 임시 버퍼
        string ackBuffer = "";
        object mSocket;

        public NetPeer()
        {
        }

        public NetPeer(object socket)
        {
            mSocket = socket;
        }

        public string GetAckPacket()
        {
            return ackBuffer;
        }

        public Controller GetController()
        {
            return (Controller)mSocket;
        }

        public Socket GetSocket()
        {
            return (Socket)mSocket;
        }

        private string Base64Encoding(string EncodingText, System.Text.Encoding oEncoding = null)
        {
            if (oEncoding == null)
                oEncoding = System.Text.Encoding.UTF8;

            byte[] arr = oEncoding.GetBytes(EncodingText);
            return System.Convert.ToBase64String(arr);
        }

        private string Base64Decoding(string DecodingText, System.Text.Encoding oEncoding = null)
        {
            if (oEncoding == null)
                oEncoding = System.Text.Encoding.UTF8;

            byte[] arr = System.Convert.FromBase64String(DecodingText);
            return oEncoding.GetString(arr);
        }

        private string ByteArrayToBinary(byte[] byteBuf)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in byteBuf)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }

            return sb.ToString();
        }

        private byte[] BinaryToByteArray(string strBinary)
        {
            List<Byte> byteList = new List<Byte>();
            
            for (int i = 0; i < strBinary.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(strBinary.Substring(i, 8), 2));
            }

            return byteList.ToArray();
        }

        public String AckWebData(NetBuffer buffer)
        {
            // 바이너리로 변경하자
            byte[] byteBuf = buffer.GetBuffer();
            string strBinary = ByteArrayToBinary(byteBuf);

            // 암호화 시키자
            return Base64Encoding(strBinary);
        }

        public bool SendData(NetBuffer buffer)
        {
            // 바이너리로 변경하자
            byte[] byteBuf = buffer.GetBuffer();
            string strBinary = ByteArrayToBinary(byteBuf);

            // 암호화 시키자
            string encryptedPacket = Base64Encoding(strBinary);
            _PACKET_STRUCT sendPacket = new _PACKET_STRUCT();
            sendPacket.size = 4 + 4096;
            sendPacket.buffer = encryptedPacket;
            byte[] sendBuffer = sendPacket.Serialize();
            // 보내자
            ((Socket)mSocket).Send(sendBuffer);

            return true;
        }

        public bool SendWebData(NetBuffer buffer)
        {
            // 바이너리로 변경하자
            byte[] byteBuf = buffer.GetBuffer();
            string strBinary = ByteArrayToBinary(byteBuf);

            // 암호화 시키자
            string encryptedPacket = "packet=" + Base64Encoding(strBinary);

            // POST 방식으로 보내자
            byte[] bytePacket = UTF8Encoding.UTF8.GetBytes(encryptedPacket);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:13569/Process");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytePacket.Length;

            // 바이트 형태를 스트림으로 만들자
            Stream stDataParams = request.GetRequestStream();
            stDataParams.Write(bytePacket, 0, bytePacket.Length);
            stDataParams.Close();

            // 요청
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // 응답 스트림 가져오기
            Stream stReadData = response.GetResponseStream();
            StreamReader srReadData = new StreamReader(stReadData, Encoding.Default);

            // 응답 스트림을 문자열로 변환
            ackBuffer = srReadData.ReadToEnd();

            // 성공실패 여부
            return true;
        }

        public byte[] RecvData(String packet)
        {
            // 복호화 하자
            string decryptedPacket = Base64Decoding(packet);
            // 바이너리 풀자
            byte[] buffer = BinaryToByteArray(decryptedPacket);

            return buffer;
        }
    }
}
