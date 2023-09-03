using ImGuiNET;
using OtterGui.Raii;
using SharpGLTF.Schema2;
using System.Collections.Generic;
using VfxEditor.PapFormat.Skeleton;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.PapFormat {
    public unsafe class PapGltfModal : Modal {
        private readonly PapAnimatedSkeleton Skeleton;
        private readonly int HavokIndex;
        private readonly string ImportPath;

        private bool Compress = false;
        private int AnimationIndex = 0;
        private readonly List<string> AnimationNames = new();
        private readonly List<string> NodeNames = new();

        public PapGltfModal( PapAnimatedSkeleton skeleton, int index, string importPath ) : base( "Animation Import" ) {
            Skeleton = skeleton;
            HavokIndex = index;
            ImportPath = importPath;

            var model = ModelRoot.Load( importPath );

            var boneNames = new List<string>();
            for( var i = 0; i < skeleton.Skeleton->Skeleton->Bones.Length; i++ ) {
                boneNames.Add( skeleton.Skeleton->Skeleton->Bones[i].Name.String );
            }

            foreach( var node in model.LogicalNodes ) {
                if( string.IsNullOrEmpty( node.Name ) || node.Name.ToLower().Contains( "mesh" ) || node.Name.ToLower().Contains( "armature" ) ) continue;
                if( !boneNames.Contains( node.Name ) || !node.IsTransformAnimated ) continue;
                NodeNames.Add( node.Name );
            }

            foreach( var animation in model.LogicalAnimations ) {
                AnimationNames.Add( animation.Name );
            }
        }

        protected override void DrawBody() {
            ImGui.Checkbox( "Compress Animation", ref Compress );

            var text = AnimationNames.Count == 0 ? "[NONE]" : AnimationNames[AnimationIndex];
            var combo = ImRaii.Combo( "Animation to Import", text );
            if( combo ) {
                for( var i = 0; i < AnimationNames.Count; i++ ) {
                    using var _ = ImRaii.PushId( i );
                    if( ImGui.Selectable( $"{AnimationNames[i]}##Name", i == AnimationIndex ) ) {
                        AnimationIndex = i;
                    }
                }
            }

            var nodes = ImRaii.TreeNode( "Nodes Being Imported" );
            if( nodes ) {
                using var child = ImRaii.Child( "Child", new( ImGui.GetContentRegionAvail().X, 300 ) );
                using var _ = ImRaii.PushIndent();
                foreach( var nodeName in NodeNames ) {
                    ImGui.Text( nodeName );
                }
            }
        }

        protected override void OnCancel() { }

        protected override void OnOk() {
            CommandManager.Pap.Add( new PapHavokCommand( Skeleton.File, () => {
                GltfAnimation.ImportAnimation(
                    Skeleton.File.AnimationData.Bones.AnimationContainer->Skeletons[0].ptr,
                    Skeleton,
                    HavokIndex,
                    AnimationIndex,
                    Compress,
                    ImportPath );
            } ) );
            UiUtils.OkNotification( "Havok data imported" );
        }
    }
}
