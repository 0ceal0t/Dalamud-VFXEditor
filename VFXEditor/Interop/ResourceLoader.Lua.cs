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

        public LuaVariableDelegate GetLuaVariable { get; private set; }

        // ======= STRINGS ===========

        public delegate IntPtr LuaReadDelegate( IntPtr a1 );

        public LuaReadDelegate LuaRead { get; private set; }

        public void LuaReadTest( string data ) {
            var bytes = Encoding.ASCII.GetBytes( data );
            var ptr = Marshal.AllocHGlobal( bytes.Length + 1 );
            Marshal.Copy( bytes, 0, ptr, bytes.Length );
            Marshal.WriteByte( ptr + bytes.Length, 0 );

            var res = LuaRead( ptr );

            Dalamud.Log( $"{data} -> {res:X8}" );

            Marshal.FreeHGlobal( ptr );
        }
    }
}
