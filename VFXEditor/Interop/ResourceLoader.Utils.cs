using Dalamud.Utility.Signatures;
using System;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        public delegate IntPtr GetMatrixSingletonDelegate();

        [Signature( Constants.GetMatrixSig )]
        public readonly GetMatrixSingletonDelegate GetMatrixSingleton = null;
    }
}
