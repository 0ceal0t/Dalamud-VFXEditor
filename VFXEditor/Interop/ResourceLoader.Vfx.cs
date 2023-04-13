using Dalamud.Hooking;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Structs.Vfx;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        //====== STATIC ===========
        public delegate IntPtr StaticVfxCreateDelegate( string path, string pool );

        public StaticVfxCreateDelegate StaticVfxCreate;

        public delegate IntPtr StaticVfxRunDelegate( IntPtr vfx, float a1, uint a2 );

        public StaticVfxRunDelegate StaticVfxRun;

        public delegate IntPtr StaticVfxRemoveDelegate( IntPtr vfx );

        public StaticVfxRemoveDelegate StaticVfxRemove;

        // ======= STATIC HOOKS ========
        public delegate IntPtr StaticVfxCreateHookDelegate( char* path, char* pool );

        public Hook<StaticVfxCreateHookDelegate> StaticVfxCreateHook { get; private set; }

        public delegate IntPtr StaticVfxRemoveHookDelegate( IntPtr vfx );

        public Hook<StaticVfxRemoveHookDelegate> StaticVfxRemoveHook { get; private set; }

        // ======== ACTOR =============
        public delegate IntPtr ActorVfxCreateDelegate( string a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );

        public ActorVfxCreateDelegate ActorVfxCreate;

        public delegate IntPtr ActorVfxRemoveDelegate( IntPtr vfx, char a2 );

        public ActorVfxRemoveDelegate ActorVfxRemove;

        // ======== ACTOR HOOKS =============
        public delegate IntPtr ActorVfxCreateHookDelegate( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );

        public Hook<ActorVfxCreateHookDelegate> ActorVfxCreateHook { get; private set; }

        public delegate IntPtr ActorVfxRemoveHookDelegate( IntPtr vfx, char a2 );

        public Hook<ActorVfxRemoveHookDelegate> ActorVfxRemoveHook { get; private set; }

        private IntPtr StaticVfxNewHandler( char* path, char* pool ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( path ), Encoding.ASCII, 256 );
            var vfx = StaticVfxCreateHook.Original( path, pool );
            Plugin.VfxTracker?.AddStatic( ( VfxStruct* )vfx, vfxPath );

            if( Plugin.Configuration?.LogVfxDebug == true ) PluginLog.Log( $"New Static: {vfxPath} {vfx:X8}" );

            return vfx;
        }

        private IntPtr StaticVfxRemoveHandler( IntPtr vfx ) {
            if( Plugin.SpawnedVfx != null && vfx == ( IntPtr )Plugin.SpawnedVfx.Vfx ) Plugin.ClearSpawn();
            Plugin.VfxTracker?.RemoveStatic( ( VfxStruct* )vfx );
            return StaticVfxRemoveHook.Original( vfx );
        }

        private IntPtr ActorVfxNewHandler( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( a1 ), Encoding.ASCII, 256 );
            var vfx = ActorVfxCreateHook.Original( a1, a2, a3, a4, a5, a6, a7 );
            Plugin.VfxTracker?.AddActor( ( VfxStruct* )vfx, vfxPath );

            if( Plugin.Configuration?.LogVfxDebug == true ) PluginLog.Log( $"New Actor: {vfxPath} {vfx:X8}" );

            return vfx;
        }

        private IntPtr ActorVfxRemoveHandler( IntPtr vfx, char a2 ) {
            if( Plugin.SpawnedVfx != null && vfx == ( IntPtr )Plugin.SpawnedVfx.Vfx ) Plugin.ClearSpawn();
            Plugin.VfxTracker?.RemoveActor( ( VfxStruct* )vfx );
            return ActorVfxRemoveHook.Original( vfx, a2 );
        }
    }
}
