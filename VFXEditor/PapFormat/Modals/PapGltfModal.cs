using ImGuiNET;
using VfxEditor.PapFormat.Skeleton;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.PapFormat {
    public unsafe class PapGltfModal : Modal {
        private readonly PapAnimatedSkeleton Skeleton;
        private readonly int Index;
        private readonly string ImportPath;

        private bool Compress = false;

        public PapGltfModal( PapAnimatedSkeleton skeleton, int index, string importPath ) : base( "Animation Import" ) {
            Skeleton = skeleton;
            Index = index;
            ImportPath = importPath;
        }

        protected override void DrawBody() {
            ImGui.Checkbox( "Compress Animation", ref Compress );
        }

        protected override void OnCancel() { }

        protected override void OnOk() {
            CommandManager.Pap.Add( new PapHavokCommand( Skeleton.File, () => {
                GltfAnimation.ImportAnimation(
                    Skeleton.File.AnimationData.Bones.AnimationContainer->Skeletons[0].ptr,
                    Skeleton,
                    Index,
                    Compress,
                    ImportPath );
            } ) );
            UiUtils.OkNotification( "Havok data imported" );
        }
    }
}
