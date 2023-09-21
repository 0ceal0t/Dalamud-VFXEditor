using System;
using VfxEditor.Interop.Havok.Structs;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        public static IntPtr HavokInterleavedAnimationVtbl { get; private set; }

        public delegate hkaSplineCompressedAnimation* HavokSplineCtorDelegate(
            hkaSplineCompressedAnimation* spline,
            hkaInterleavedUncompressedAnimation* interleaved );

        public HavokSplineCtorDelegate HavokSplineCtor { get; private set; }
    }
}
