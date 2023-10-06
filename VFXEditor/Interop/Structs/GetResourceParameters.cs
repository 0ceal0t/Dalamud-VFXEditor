using System.Runtime.InteropServices;

namespace VfxEditor.Structs {
    [StructLayout( LayoutKind.Explicit )]
    public struct GetResourceParameters {
        [FieldOffset( 16 )]
        public uint SegmentOffset;

        [FieldOffset( 20 )]
        public uint SegmentLength;

        public readonly bool IsPartialRead
            => SegmentLength != 0;
    }
}