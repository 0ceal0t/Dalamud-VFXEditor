using System;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        public delegate uint LuaVariableDelegate( IntPtr a1, uint a2 );

        public IntPtr LuaManager { get; private set; }
        public LuaVariableDelegate GetLuaVariable { get; private set; }
    }
}
