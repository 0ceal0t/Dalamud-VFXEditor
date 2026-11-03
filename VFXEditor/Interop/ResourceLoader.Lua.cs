using Dalamud.Utility.Signatures;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        public delegate uint LuaVariableDelegate( IntPtr a1, uint a2 );

        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        public delegate uint LuaActorVariableDelegate( IntPtr a1 );

        public IntPtr LuaManager { get; private set; }

        public IntPtr LuaActorVariables { get; private set; }

        public int LuaActorVariableCount { get; private set; }

        public const int LuaActorVariableStride = 16;

        [Signature( Constants.LuaGetVariableSig )]
        public readonly LuaVariableDelegate GetLuaVariable = null;

        // ======= STRINGS ===========

        public delegate IntPtr LuaReadDelegate( IntPtr a1 );

        [Signature( Constants.LuaReadSig )]
        public readonly LuaReadDelegate LuaRead = null;
    }
}