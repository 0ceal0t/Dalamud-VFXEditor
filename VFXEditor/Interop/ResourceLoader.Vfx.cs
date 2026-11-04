using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.Graphics.Vfx;
using InteropGenerator.Runtime;
using System;
using System.Runtime.InteropServices;
using VfxEditor.Data;
using VfxEditor.Spawn;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        //====== STATIC ===========
        public delegate IntPtr StaticVfxRunDelegate( VfxObject* vfx, float a1, uint a2 );

        [Signature( Constants.StaticVfxRunSig )]
        public readonly StaticVfxRunDelegate StaticVfxRun = null;

        public delegate IntPtr StaticVfxRemoveDelegate( VfxObject* vfx );

        [Signature( Constants.StaticVfxRemoveSig )]
        public readonly StaticVfxRemoveDelegate StaticVfxRemove = null;

        // ======= STATIC HOOKS ========
        public Hook<VfxObject.Delegates.Create> StaticVfxCreateHook { get; private set; }

        [Signature( Constants.StaticVfxRemoveSig, DetourName = nameof( StaticVfxRemoveDetour ) )]
        public readonly Hook<StaticVfxRemoveDelegate> StaticVfxRemoveHook = null;

        // ======== ACTOR =============
        public delegate VfxData* ActorVfxCreateDelegate( byte* path, GameObject* caster, GameObject* target, float a4, char a5, ushort a6, char a7 );

        [Signature( Constants.ActorVfxCreateSig )]
        public readonly ActorVfxCreateDelegate ActorVfxCreate = null;

        public delegate IntPtr ActorVfxRemoveDelegate( VfxData* vfx, char a2 );

        public ActorVfxRemoveDelegate ActorVfxRemove;

        // ======== ACTOR HOOKS =============
        [Signature( Constants.ActorVfxCreateSig, DetourName = nameof( ActorVfxNewDetour ) )]
        public readonly Hook<ActorVfxCreateDelegate> ActorVfxCreateHook = null;

        public Hook<ActorVfxRemoveDelegate> ActorVfxRemoveHook { get; private set; }

        // ==============================

        private VfxObject* StaticVfxNewDetour( CStringPointer path, CStringPointer pool ) {
            var vfx = StaticVfxCreateHook.Original( path, pool );
            Plugin.TrackerManager?.Vfx.AddStatic( vfx, $"{path}" );

            if( Plugin.Configuration?.LogVfxDebug == true ) Dalamud.Log( $"New Static: {path} {( nint )vfx:X8}" );
            return vfx;
        }

        private IntPtr StaticVfxRemoveDetour( VfxObject* vfx ) {
            VfxSpawn.InteropRemoved( ( nint )vfx );
            Plugin.TrackerManager?.Vfx.RemoveStatic( vfx );
            return StaticVfxRemoveHook.Original( vfx );
        }

        private VfxData* ActorVfxNewDetour( byte* pathPtr, GameObject* caster, GameObject* target, float a4, char a5, ushort a6, char a7 ) {
            var vfx = ActorVfxCreateHook.Original( pathPtr, caster, target, a4, a5, a6, a7 );
            var path = Marshal.PtrToStringUTF8( ( nint )pathPtr );
            Plugin.TrackerManager?.Vfx.AddActor( vfx, path, caster == null ? 0 : caster->EntityId, target == null ? 0 : target->EntityId );

            if( Plugin.Configuration?.LogVfxDebug == true ) Dalamud.Log( $"New Actor: {path} {( nint )vfx:X8}" );
            return vfx;
        }

        private IntPtr ActorVfxRemoveDetour( VfxData* vfx, char a2 ) {
            VfxSpawn.InteropRemoved( ( nint )vfx );
            Plugin.TrackerManager?.Vfx.RemoveActor( vfx );
            return ActorVfxRemoveHook.Original( vfx, a2 );
        }
    }
}