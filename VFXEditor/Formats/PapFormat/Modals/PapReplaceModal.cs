using ImGuiNET;
using VfxEditor.Interop.Havok;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public unsafe class PapReplaceModal : Modal {
        private readonly PapMotion Motion;
        private readonly int HavokIndex;
        private readonly string ImportPath;

        private int Index;

        public PapReplaceModal( PapMotion motion, int index, string importPath ) : base( "Animation Index", true ) {
            Motion = motion;
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
            Command.AddAndExecute( new PapHavokCommand( Motion.File, () => {
                var newAnimation = new HavokData( ImportPath, true );
                var container = Motion.File.MotionData.AnimationContainer;

                // Do this so we can undo the change later if necessary
                var anims = HavokData.ToList( container->Animations );
                var bindings = HavokData.ToList( container->Bindings );
                anims[HavokIndex] = newAnimation.AnimationContainer->Animations[Index];
                bindings[HavokIndex] = newAnimation.AnimationContainer->Bindings[Index];

                container->Animations = HavokData.CreateArray( Motion.File.Handles, ( uint )container->Animations.Flags, anims, sizeof( nint ) );
                container->Bindings = HavokData.CreateArray( Motion.File.Handles, ( uint )container->Bindings.Flags, bindings, sizeof( nint ) );
            } ) );
            UiUtils.OkNotification( "Havok data replaced" );
        }
    }
}
