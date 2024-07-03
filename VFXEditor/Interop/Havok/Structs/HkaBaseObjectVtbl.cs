using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok.Structs {
    [StructLayout( LayoutKind.Explicit, Size = 0x08 )]
    public unsafe struct HkBaseObject {
        [FieldOffset( 0x00 )] public HkBaseObjectVtbl* vfptr;

        [StructLayout( LayoutKind.Explicit, Size = 0x10 )]
        public struct HkBaseObjectVtbl {
            [FieldOffset( 0x00 )] public void* dtor;
            [FieldOffset( 0x08 )] public void* __first_virtual_table_function__;
        }
    }
}
