using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Common
{
    public enum ProtocolType { CPP, CS, CSWEB };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _PACKET_STRUCT
    {
        [MarshalAs(UnmanagedType.I4)]
        public Int32 size;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
        public string buffer;
        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(_PACKET_STRUCT))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }

        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (_PACKET_STRUCT)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(_PACKET_STRUCT));
            gch.Free();
        }

    };
}
