using System.Runtime.InteropServices;

namespace VFXEditor.Structs
{
    [StructLayout( LayoutKind.Explicit )]
    public unsafe struct ResourceHandle {
        [FieldOffset( 0x48 )]
        public StdString File;
    }
}

[StructLayout( LayoutKind.Explicit )]
public unsafe struct StdString {
    [FieldOffset( 0x0 )]
    public byte* BufferPtr;

    [FieldOffset( 0x0 )]
    public fixed byte Buffer[16];

    [FieldOffset( 0x10 )]
    public ulong Size;

    [FieldOffset( 0x18 )]
    public ulong Capacity;
}