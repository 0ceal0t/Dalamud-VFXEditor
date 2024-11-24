using System.Runtime.InteropServices;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        public ResourceLoader() {
            Dalamud.Hooks.InitializeFromAttributes( this );

            ReadSqpackHook = Dalamud.Hooks.HookFromSignature<ReadSqpackPrototype>( Constants.ReadSqpackSig, ReadSqpackDetour );
            GetResourceSyncHook = Dalamud.Hooks.HookFromSignature<GetResourceSyncPrototype>( Constants.GetResourceSyncSig, GetResourceSyncDetour );
            GetResourceAsyncHook = Dalamud.Hooks.HookFromSignature<GetResourceAsyncPrototype>( Constants.GetResourceAsyncSig, GetResourceAsyncDetour );
            ReadFile = Marshal.GetDelegateForFunctionPointer<ReadFilePrototype>( Dalamud.SigScanner.ScanText( Constants.ReadFileSig ) );

            var staticVfxCreateAddress = Dalamud.SigScanner.ScanText( Constants.StaticVfxCreateSig );
            var staticVfxRemoveAddress = Dalamud.SigScanner.ScanText( Constants.StaticVfxRemoveSig );
            var actorVfxCreateAddress = Dalamud.SigScanner.ScanText( Constants.ActorVfxCreateSig );
            var actorVfxRemoveAddresTemp = Dalamud.SigScanner.ScanText( Constants.ActorVfxRemoveSig ) + 7;
            var actorVfxRemoveAddress = Marshal.ReadIntPtr( actorVfxRemoveAddresTemp + Marshal.ReadInt32( actorVfxRemoveAddresTemp ) + 4 );

            ActorVfxCreate = Marshal.GetDelegateForFunctionPointer<ActorVfxCreateDelegate>( actorVfxCreateAddress );
            ActorVfxRemove = Marshal.GetDelegateForFunctionPointer<ActorVfxRemoveDelegate>( actorVfxRemoveAddress );
            StaticVfxRemove = Marshal.GetDelegateForFunctionPointer<StaticVfxRemoveDelegate>( staticVfxRemoveAddress );
            StaticVfxRun = Marshal.GetDelegateForFunctionPointer<StaticVfxRunDelegate>( Dalamud.SigScanner.ScanText( Constants.StaticVfxRunSig ) );
            StaticVfxCreate = Marshal.GetDelegateForFunctionPointer<StaticVfxCreateDelegate>( staticVfxCreateAddress );

            StaticVfxCreateHook = Dalamud.Hooks.HookFromAddress<StaticVfxCreateDelegate>( staticVfxCreateAddress, StaticVfxNewDetour );
            StaticVfxRemoveHook = Dalamud.Hooks.HookFromAddress<StaticVfxRemoveDelegate>( staticVfxRemoveAddress, StaticVfxRemoveDetour );
            ActorVfxCreateHook = Dalamud.Hooks.HookFromAddress<ActorVfxCreateDelegate>( actorVfxCreateAddress, ActorVfxNewDetour );
            ActorVfxRemoveHook = Dalamud.Hooks.HookFromAddress<ActorVfxRemoveDelegate>( actorVfxRemoveAddress, ActorVfxRemoveDetour );

            GetMatrixSingleton = Marshal.GetDelegateForFunctionPointer<GetMatrixSingletonDelegate>( Dalamud.SigScanner.ScanText( Constants.GetMatrixSig ) );
            GetFileManager = Marshal.GetDelegateForFunctionPointer<GetFileManagerDelegate>( Dalamud.SigScanner.ScanText( Constants.GetFileManagerSig ) );
            GetFileManager2 = Marshal.GetDelegateForFunctionPointer<GetFileManagerDelegate>( Dalamud.SigScanner.ScanText( Constants.GetFileManager2Sig ) );
            DecRef = Marshal.GetDelegateForFunctionPointer<DecRefDelegate>( Dalamud.SigScanner.ScanText( Constants.DecRefSig ) );
            RequestFile = Marshal.GetDelegateForFunctionPointer<RequestFileDelegate>( Dalamud.SigScanner.ScanText( Constants.RequestFileSig ) );

            CheckFileStateHook = Dalamud.Hooks.HookFromSignature<CheckFileStatePrototype>( Constants.CheckFileStateSig, CheckFileStateDetour );
            LoadTexFileLocal = Marshal.GetDelegateForFunctionPointer<LoadTexFileLocalDelegate>( Dalamud.SigScanner.ScanText( Constants.LoadTexFileLocalSig ) );
            LoadMdlFileLocal = Marshal.GetDelegateForFunctionPointer<LoadMdlFileLocalDelegate>( Dalamud.SigScanner.ScanText( Constants.LoadMdlFileLocalSig ) );
            LoadMdlFileExternHook = Dalamud.Hooks.HookFromSignature<LoadMdlFileExternDelegate>( Constants.LoadMdlFileExternSig, LoadMdlFileExternDetour );

            PlayActionHook = Dalamud.Hooks.HookFromSignature<PlayActionPrototype>( Constants.PlayActionSig, PlayActionDetour );

            GetLuaVariable = Marshal.GetDelegateForFunctionPointer<LuaVariableDelegate>( Dalamud.SigScanner.ScanText( Constants.LuaGetVariableSig ) );

            var luaManagerStart = Dalamud.SigScanner.ScanText( Constants.LuaManagerSig ) + 3;
            var luaManagerOffset = Marshal.ReadInt32( luaManagerStart );
            LuaManager = luaManagerStart + 4 + luaManagerOffset;

            var luaActorVariableStart = Dalamud.SigScanner.ScanText( Constants.LuaActorVariableSig ) + 3;
            var luaActorVariableOffset = Marshal.ReadInt32( luaActorVariableStart );
            LuaActorVariables = luaActorVariableStart + 4 + luaActorVariableOffset;

            VfxUseTriggerHook = Dalamud.Hooks.HookFromSignature<VfxUseTriggerDelete>( Constants.CallTriggerSig, VfxUseTriggerDetour );

            var interleavedVtbl = Dalamud.SigScanner.ScanText( Constants.HavokInterleavedVtblSig ) - 4;
            var interleavedVtblOffset = Marshal.ReadInt32( interleavedVtbl );
            HavokInterleavedAnimationVtbl = interleavedVtbl + 4 + interleavedVtblOffset;

            var mappedVtbl = Dalamud.SigScanner.ScanText( Constants.HavokMapperVtblSig ) + 18;
            var mappedVtblOffset = Marshal.ReadInt32( mappedVtbl );
            HavokMapperVtbl = mappedVtbl + 4 + mappedVtblOffset;

            HavokSplineCtor = Marshal.GetDelegateForFunctionPointer<HavokSplineCtorDelegate>( Dalamud.SigScanner.ScanText( Constants.HavokSplineCtorSig ) );

            PlaySoundPath = Marshal.GetDelegateForFunctionPointer<PlaySoundDelegate>( Dalamud.SigScanner.ScanText( Constants.PlaySoundSig ) );
            InitSoundHook = Dalamud.Hooks.HookFromSignature<InitSoundPrototype>( Constants.InitSoundSig, InitSoundDetour );

            LuaRead = Marshal.GetDelegateForFunctionPointer<LuaReadDelegate>( Dalamud.SigScanner.ScanText( Constants.LuaReadSig ) );

            HumanSetupScalingHook = Dalamud.Hooks.HookFromAddress<CharacterBaseSetupScalingDelegate>( HumanVTable[58], SetupScaling );
            HumanCreateDeformerHook = Dalamud.Hooks.HookFromAddress<CharacterBaseCreateDeformerDelegate>( HumanVTable[101], CreateDeformer );

            ReadSqpackHook.Enable();
            GetResourceSyncHook.Enable();
            GetResourceAsyncHook.Enable();

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