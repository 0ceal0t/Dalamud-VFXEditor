using Dalamud.Utility.Signatures;
using System;

namespace VfxEditor.Interop {
    public partial class ResourceLoader {
        public delegate IntPtr GetMatrixSingletonDelegate();

        [Signature( Constants.GetMatrixSig )]
        public readonly GetMatrixSingletonDelegate GetMatrixSingleton = null;
    }
}