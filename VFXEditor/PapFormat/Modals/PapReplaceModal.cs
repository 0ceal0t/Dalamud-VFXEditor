using ImGuiNET;
using VfxEditor.Interop.Havok;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public unsafe class PapReplaceModal : Modal {
        private readonly PapAnimation Animation;
        private readonly string ImportPath;
        private int Index;

        public PapReplaceModal( PapAnimation animation, string importPath ) : base( "Animation Index" ) {
            Animation = animation;
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
            CommandManager.Pap.Add( new PapHavokCommand( Animation.File, () => {
                var newAnimation = new HavokData( ImportPath );
                var container = Animation.File.AnimationData.AnimationContainer;

                // Do this so we can undo the change later if necessary
                var anims = HavokData.ToList( container->Animations );
                var bindings = HavokData.ToList( container->Bindings );
                anims[Animation.HavokIndex] = newAnimation.AnimationContainer->Animations[Index];
                bindings[Animation.HavokIndex] = newAnimation.AnimationContainer->Bindings[Index];

                container->Animations = HavokData.CreateArray( container->Animations, anims, sizeof( nint ), out var _ );
                container->Bindings = HavokData.CreateArray( container->Bindings, bindings, sizeof( nint ), out var _ );
            } ) );
            UiUtils.OkNotification( "Havok data replaced" );
        }
    }
}
