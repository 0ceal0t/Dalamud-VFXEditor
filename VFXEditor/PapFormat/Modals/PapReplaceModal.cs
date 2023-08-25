using ImGuiNET;
using VfxEditor.Interop.Havok;
using VfxEditor.PapFormat.Skeleton;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public unsafe class PapReplaceModal : Modal {
        private readonly PapAnimatedSkeleton Skeleton;
        private readonly int HavokIndex;
        private readonly string ImportPath;

        private int Index;

        public PapReplaceModal( PapAnimatedSkeleton skeleton, int index, string importPath ) : base( "Animation Index" ) {
            Skeleton = skeleton;
            HavokIndex = index;
            ImportPath = importPath;
        }

        protected override void DrawBody() {
            ImGui.PushTextWrapPos( 240 );
            ImGui.TextWrapped( "Select the index of the animation being imported" );
            ImGui.PopTextWrapPos();

            ImGui.InputInt( "##Index", ref Index );
        }

        protected override void OnCancel() { }

        protected override void OnOk() {
            CommandManager.Pap.Add( new PapHavokCommand( Skeleton.File, () => {
                var newAnimation = new HavokData( ImportPath );
                var container = Skeleton.File.AnimationData.AnimationContainer;

                // Do this so we can undo the change later if necessary
                var anims = HavokData.ToList( container->Animations );
                var bindings = HavokData.ToList( container->Bindings );
                anims[HavokIndex] = newAnimation.AnimationContainer->Animations[Index];
                bindings[HavokIndex] = newAnimation.AnimationContainer->Bindings[Index];

                container->Animations = HavokData.CreateArray( container->Animations.Flags, anims, sizeof( nint ), out var _ );
                container->Bindings = HavokData.CreateArray( container->Bindings.Flags, bindings, sizeof( nint ), out var _ );
            } ) );
            UiUtils.OkNotification( "Havok data replaced" );
        }
    }
}
