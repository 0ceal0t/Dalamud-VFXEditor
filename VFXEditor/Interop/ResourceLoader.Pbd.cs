using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using VfxEditor.Interop.Structs;
using VfxEditor.Structs;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        [Signature( Constants.CharacterUtilitySig, ScanType = ScanType.StaticAddress )]
        private readonly CharacterUtilityData** CharacterUtilityAddress = null;

        [Signature( Constants.HumanVTable, ScanType = ScanType.StaticAddress )]
        public readonly nint* HumanVTable = null!;

        private delegate void CharacterBaseSetupScalingDelegate( CharacterBase* drawObject, uint slotIndex );
        private delegate void* CharacterBaseCreateDeformerDelegate( CharacterBase* drawObject, uint slotIndex );

        private readonly Hook<CharacterBaseSetupScalingDelegate> HumanSetupScalingHook;
        private readonly Hook<CharacterBaseCreateDeformerDelegate> HumanCreateDeformerHook;

        private ResourceHandle* CurrentPbd;

        public void ReplacePbd( ResourceHandle* newPbd ) {
            CurrentPbd = newPbd;
        }

        private void SetupScaling( CharacterBase* drawObject, uint slotIndex ) {
            if( CurrentPbd != null ) ( *CharacterUtilityAddress )->HumanPbdResource = CurrentPbd;
            HumanSetupScalingHook.Original( drawObject, slotIndex );
        }

        private void* CreateDeformer( CharacterBase* drawObject, uint slotIndex ) {
            if( CurrentPbd != null ) ( *CharacterUtilityAddress )->HumanPbdResource = CurrentPbd;
            return HumanCreateDeformerHook.Original( drawObject, slotIndex );
        }
    }
}
