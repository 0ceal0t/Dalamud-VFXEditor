using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        public ResourceLoader() {
            Dalamud.Hooks.InitializeFromAttributes( this );

            var staticVfxCreateAddress = Dalamud.SigScanner.ScanText( VfxObject.Addresses.Create.String );
            var actorVfxRemoveAddressTemp = Dalamud.SigScanner.ScanText( Constants.ActorVfxRemoveSig ) + 7;
            var actorVfxRemoveAddress = Marshal.ReadIntPtr( actorVfxRemoveAddressTemp + Marshal.ReadInt32( actorVfxRemoveAddressTemp ) + 4 );

            ActorVfxRemove = Marshal.GetDelegateForFunctionPointer<ActorVfxRemoveDelegate>( actorVfxRemoveAddress );
            StaticVfxCreate = Marshal.GetDelegateForFunctionPointer<VfxObject.Delegates.Create>( staticVfxCreateAddress );

            StaticVfxCreateHook = Dalamud.Hooks.HookFromAddress<VfxObject.Delegates.Create>( staticVfxCreateAddress, StaticVfxNewDetour );
            ActorVfxRemoveHook = Dalamud.Hooks.HookFromAddress<ActorVfxRemoveDelegate>( actorVfxRemoveAddress, ActorVfxRemoveDetour );

            var luaManagerStart = Dalamud.SigScanner.ScanText( Constants.LuaManagerSig ) + 3;
            var luaManagerOffset = Marshal.ReadInt32( luaManagerStart );
            LuaManager = luaManagerStart + 4 + luaManagerOffset;

            var luaActorVariableStart = Dalamud.SigScanner.ScanText( Constants.LuaActorVariableSig ) + 3;
            var luaActorVariableOffset = Marshal.ReadInt32( luaActorVariableStart );
            LuaActorVariables = luaActorVariableStart + 4 + luaActorVariableOffset;

            var interleavedVtbl = Dalamud.SigScanner.ScanText( Constants.HavokInterleavedVtblSig ) - 4;
            var interleavedVtblOffset = Marshal.ReadInt32( interleavedVtbl );
            HavokInterleavedAnimationVtbl = interleavedVtbl + 4 + interleavedVtblOffset;

            var mappedVtbl = Dalamud.SigScanner.ScanText( Constants.HavokMapperVtblSig ) + 18;
            var mappedVtblOffset = Marshal.ReadInt32( mappedVtbl );
            HavokMapperVtbl = mappedVtbl + 4 + mappedVtblOffset;

            HumanSetupScalingHook = Dalamud.Hooks.HookFromAddress<CharacterBaseSetupScalingDelegate>( HumanVTable[58], SetupScaling );
            HumanCreateDeformerHook = Dalamud.Hooks.HookFromAddress<CharacterBaseCreateDeformerDelegate>( HumanVTable[101], CreateDeformer );

            //ReadSqpackHook.Enable();
            //GetResourceSyncHook.Enable();
            //GetResourceAsyncHook.Enable();

            StaticVfxCreateHook.Enable();
            StaticVfxRemoveHook.Enable();
            ActorVfxCreateHook.Enable();
            ActorVfxRemoveHook.Enable();

            CheckFileStateHook.Enable();
            LoadMdlFileExternHook.Enable();
            TextureOnLoadHook.Enable();
            SoundOnLoadHook.Enable();

            PlayActionHook.Enable();
            VfxUseTriggerHook.Enable();
            InitSoundHook.Enable();

            HumanSetupScalingHook.Enable();
            HumanCreateDeformerHook.Enable();

            PathResolved += AddCrc;
        }

        public void Dispose() {
            PathResolved -= AddCrc;

            ReadSqpackHook.Dispose();
            GetResourceSyncHook.Dispose();
            GetResourceAsyncHook.Dispose();

            StaticVfxCreateHook.Dispose();
            StaticVfxRemoveHook.Dispose();
            ActorVfxCreateHook.Dispose();
            ActorVfxRemoveHook.Dispose();

            CheckFileStateHook.Dispose();
            LoadMdlFileExternHook.Dispose();
            TextureOnLoadHook.Dispose();
            SoundOnLoadHook.Dispose();

            PlayActionHook.Dispose();
            VfxUseTriggerHook.Dispose();
            InitSoundHook.Dispose();

            HumanSetupScalingHook.Dispose();
            HumanCreateDeformerHook.Dispose();
        }
    }
}