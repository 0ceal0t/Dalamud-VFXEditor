using System.Runtime.InteropServices;

namespace VFXEditor.Structs
{
    [StructLayout( LayoutKind.Explicit )]
    public unsafe struct ResourceHandle
    {
        [FieldOffset( 0x48 )]
        public byte* FileName;
    }
}