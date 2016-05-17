using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    class NetBuffer
    {
        private byte[] mBuffer;
        UInt32 mAddOffset = 0;
        UInt32 mGetOffset = 0;

        public NetBuffer(byte[] buffer)
        {
            mBuffer = buffer;
        }
        public NetBuffer()
        {
            mBuffer = new byte[256];
        }

        public byte[] GetBuffer() { return mBuffer; }

        public void GetString(ref string sStr)
        {
            UInt32 strSize = mBuffer[mGetOffset++];
            sStr = Encoding.Default.GetString(mBuffer, Convert.ToInt32(mGetOffset), Convert.ToInt32(strSize));
            mGetOffset += Convert.ToUInt32(sStr.Length);
            return;
        }
        public void AddString(string sStr)
        {
            mBuffer[mAddOffset++] = Convert.ToByte(sStr.Length);
            Array.Copy(Encoding.Default.GetBytes(sStr), 0, mBuffer, mAddOffset, sStr.Length);
            mAddOffset += Convert.ToUInt32(sStr.Length);
            return;
        }

        public void GetFloat(ref float fFloat)
        {
            fFloat = BitConverter.ToSingle(mBuffer, Convert.ToInt32(mGetOffset));
            mGetOffset += sizeof(float);
            return;
        }
        public void AddFloat(float fFloat)
        {
            byte[] bFloat = BitConverter.GetBytes(fFloat);
            Array.Copy(bFloat, 0, mBuffer, mAddOffset, sizeof(float));
            mAddOffset += sizeof(float);
            return;
        }

        public void GetDouble(ref double dDouble)
        {
            dDouble = BitConverter.ToDouble(mBuffer, Convert.ToInt32(mGetOffset));
            mGetOffset += sizeof(double);
            return;
        }
        public void AddDouble(double dDouble)
        {
            byte[] bFloat = BitConverter.GetBytes(dDouble);
            Array.Copy(bFloat, 0, mBuffer, mAddOffset, sizeof(double));
            mAddOffset += sizeof(double);
            return;
        }

        public void GetUInt8(ref byte nUint)
        {
            nUint = BitConverter.GetBytes(mBuffer[mGetOffset])[0];
            mGetOffset += sizeof(byte);
            return;
        }
        public void GetUInt16(ref UInt16 nUint)
        {
            nUint = BitConverter.ToUInt16(mBuffer, Convert.ToInt32(mGetOffset));
            mGetOffset += sizeof(UInt16);
            return;
        }
        public void GetUInt32(ref UInt32 nUint)
        {
            nUint = BitConverter.ToUInt32(mBuffer, Convert.ToInt32(mGetOffset));
            mGetOffset += sizeof(UInt32);
            return;
        }
        public void GetUInt64(ref UInt64 nUint)
        {
            nUint = BitConverter.ToUInt64(mBuffer, Convert.ToInt32(mGetOffset));
            mGetOffset += sizeof(UInt64);
            return;
        }

        public void AddUInt8(byte nUint)
        {
            byte[] bByte = BitConverter.GetBytes(nUint);
            Array.Copy(bByte, 0, mBuffer, mAddOffset, sizeof(byte));
            mAddOffset += sizeof(byte);
            return;
        }
        public void AddUInt16(UInt16 nUint)
        {
            byte[] bByte = BitConverter.GetBytes(nUint);
            Array.Copy(bByte, 0, mBuffer, mAddOffset, sizeof(UInt16));
            mAddOffset += sizeof(UInt16);
            return;
        }
        public void AddUInt32(UInt32 nUint)
        {
            byte[] bByte = BitConverter.GetBytes(nUint);
            Array.Copy(bByte, 0, mBuffer, mAddOffset, sizeof(UInt32));
            mAddOffset += sizeof(UInt32);
            return;
        }
        public void AddUInt64(UInt64 nUint)
        {
            byte[] bByte = BitConverter.GetBytes(nUint);
            Array.Copy(bByte, 0, mBuffer, mAddOffset, sizeof(UInt64));
            mAddOffset += sizeof(UInt64);
            return;
        }
    }
}
