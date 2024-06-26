using Dalamud.Hooking;
using System;
using VfxEditor.Spawn;
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
        public Hook<StaticVfxCreateDelegate> StaticVfxCreateHook { get; private set; }

        public Hook<StaticVfxRemoveDelegate> StaticVfxRemoveHook { get; private set; }

        // ======== ACTOR =============
        public delegate IntPtr ActorVfxCreateDelegate( string path, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );

        public ActorVfxCreateDelegate ActorVfxCreate;

        public delegate IntPtr ActorVfxRemoveDelegate( IntPtr vfx, char a2 );

        public ActorVfxRemoveDelegate ActorVfxRemove;

        // ======== ACTOR HOOKS =============
        public Hook<ActorVfxCreateDelegate> ActorVfxCreateHook { get; private set; }

        public Hook<ActorVfxRemoveDelegate> ActorVfxRemoveHook { get; private set; }

        // ======= TRIGGERS =============
        public delegate IntPtr VfxUseTriggerDelete( IntPtr vfx, uint triggerId );

        public Hook<VfxUseTriggerDelete> VfxUseTriggerHook { get; private set; }

        // ==============================

        private IntPtr StaticVfxNewDetour( string path, string pool ) {
            var vfx = StaticVfxCreateHook.Original( path, pool );
            Plugin.TrackerManager?.Vfx.AddStatic( ( VfxStruct* )vfx, path );

            if( Plugin.Configuration?.LogVfxDebug == true ) Dalamud.Log( $"New Static: {path} {vfx:X8}" );
            return vfx;
        }

        private IntPtr StaticVfxRemoveDetour( IntPtr vfx ) {
            VfxSpawn.InteropRemoved( vfx );
            Plugin.TrackerManager?.Vfx.RemoveStatic( ( VfxStruct* )vfx );
            return StaticVfxRemoveHook.Original( vfx );
        }

        private IntPtr ActorVfxNewDetour( string path, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 ) {
            var vfx = ActorVfxCreateHook.Original( path, a2, a3, a4, a5, a6, a7 );
            Plugin.TrackerManager?.Vfx.AddActor( ( VfxStruct* )vfx, path );

            if( Plugin.Configuration?.LogVfxDebug == true ) Dalamud.Log( $"New Actor: {path} {vfx:X8}" );
            return vfx;
        }

        private IntPtr ActorVfxRemoveDetour( IntPtr vfx, char a2 ) {
            VfxSpawn.InteropRemoved( vfx );
            Plugin.TrackerManager?.Vfx.RemoveActor( ( VfxStruct* )vfx );
            return ActorVfxRemoveHook.Original( vfx, a2 );
        }

        private IntPtr VfxUseTriggerDetour( IntPtr vfx, uint triggerId ) {
            var timeline = VfxUseTriggerHook.Original( vfx, triggerId );

            if( Plugin.Configuration?.LogVfxTriggers == true ) Dalamud.Log( $"Trigger {triggerId} on {vfx:X8}, timeline: {timeline:X8}" );
            return timeline;
        }
    }
}
