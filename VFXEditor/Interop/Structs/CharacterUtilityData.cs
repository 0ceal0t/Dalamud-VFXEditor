using System.Runtime.InteropServices;
using VfxEditor.Structs;

namespace VfxEditor.Interop.Structs {
    [StructLayout( LayoutKind.Explicit )]
    public unsafe struct CharacterUtilityData {
        public const int IndexHumanPbd = 63;

        [FieldOffset( 8 + IndexHumanPbd * 8 )]
        public ResourceHandle* HumanPbdResource;
    }
}
