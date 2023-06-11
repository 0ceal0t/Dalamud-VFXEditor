using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        // ====== PLAY ACTION =======

        public delegate IntPtr PlayActionPrototype( IntPtr a1, IntPtr a2, char a3, IntPtr a4 );

        public Hook<PlayActionPrototype> PlayActionHook { get; private set; }

        private IntPtr PlayActionDetour( IntPtr timeline, IntPtr a2, char a3, IntPtr a4 ) {
            var ret = PlayActionHook.Original( timeline, a2, a3, a4 );
            ProcessTimeline( timeline );
            return ret;
        }

        private void ProcessTimeline( IntPtr timeline ) {
            try {
                if( timeline != IntPtr.Zero ) {
                    var getGameObjectIdx = ( ( delegate* unmanaged< IntPtr, int >** )timeline )[0][Constants.GetGameObjectIdxVfunc];
                    var idx = getGameObjectIdx( timeline );
                    if( idx >= 0 && idx < Plugin.Objects.Length ) {
                        var obj = ( GameObject* )Plugin.Objects.GetObjectAddress( idx );
                        if( obj == null ) return;

                        var action = Marshal.ReadIntPtr( timeline + Constants.TimelineToActionOffset );
                        if( action == IntPtr.Zero ) return;

                        var actionString = Marshal.PtrToStringAnsi( action + Constants.ActionToNameOffset );
                        var objectId = obj->ObjectID;

                        Plugin.Tracker?.Action.AddAction( action, ( int )objectId, actionString );
                    }
                }
            }
            catch( Exception e ) {
                PluginLog.Error( "Error reading timeline", e );
            }
        }
    }
}
