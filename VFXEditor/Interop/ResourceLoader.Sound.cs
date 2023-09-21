using Dalamud.Hooking;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        private IntPtr OverridenSound = IntPtr.Zero;
        private int OverridenSoundIdx = -1;

        // ====== PLAY SOUND =======

        public delegate IntPtr PlaySoundDelegate( IntPtr path, byte play );

        private PlaySoundDelegate PlaySoundPath;

        public void PlaySound( string path, int idx ) {
            if( string.IsNullOrEmpty( path ) || idx < 0 || !Dalamud.DataManager.FileExists( path ) ) return;

            var bytes = Encoding.ASCII.GetBytes( path );
            var ptr = Marshal.AllocHGlobal( bytes.Length + 1 );
            Marshal.Copy( bytes, 0, ptr, bytes.Length );
            Marshal.WriteByte( ptr + bytes.Length, 0 );

            OverridenSound = ptr;
            OverridenSoundIdx = idx;

            PlaySoundPath( ptr, 1 );

            OverridenSound = IntPtr.Zero;
            OverridenSoundIdx = -1;
        }

        // ====== INIT SOUND =========

        public delegate IntPtr InitSoundPrototype( IntPtr a1, IntPtr path, float volume, int idx, int a5, uint a6, uint a7 );

        public Hook<InitSoundPrototype> InitSoundHook { get; private set; }

        private IntPtr InitSoundDetour( IntPtr a1, IntPtr path, float volume, int idx, int a5, uint a6, uint a7 ) {
            if( path != IntPtr.Zero && path == OverridenSound ) {
                return InitSoundHook.Original( a1, path, volume, OverridenSoundIdx, a5, a6, a7 );
            }

            return InitSoundHook.Original( a1, path, volume, idx, a5, a6, a7 );
        }
    }
}
