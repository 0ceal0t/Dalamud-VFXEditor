using Dalamud.Hooking;
using System;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        // ====== PLAY ACTION =======

        public delegate IntPtr PlayActionPrototype( IntPtr a1, IntPtr a2, char a3, IntPtr a4 );

        public Hook<PlayActionPrototype> PlayActionHook { get; private set; }

        private IntPtr PlayActionDetour( IntPtr timeline, IntPtr a2, char a3, IntPtr a4 ) {
            var ret = PlayActionHook.Original( timeline, a2, a3, a4 );
            Plugin.TrackerManager?.Tmb.AddAction( timeline );
            return ret;
        }
    }
}
