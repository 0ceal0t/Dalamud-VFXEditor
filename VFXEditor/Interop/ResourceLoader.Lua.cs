using System;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        public delegate uint LuaVariableDelegate( IntPtr a1, uint a2 );

        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        public delegate uint LuaActorVariableDelegate( IntPtr a1 );

        public IntPtr LuaManager { get; private set; }

        public IntPtr LuaActorVariables { get; private set; }

        public LuaVariableDelegate GetLuaVariable { get; private set; }
    }
}
