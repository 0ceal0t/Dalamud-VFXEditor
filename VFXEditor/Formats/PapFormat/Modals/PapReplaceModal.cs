using ImGuiNET;
using VfxEditor.Interop.Havok;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Ui.Components;

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
            var newAnimation = new HavokData( ImportPath, true );
            var newAnimationsLength = newAnimation.AnimationContainer->Animations.Length;
            var OkResult = "Havok data replaced";
            if( Index < 0 )
            {
                Index = 0;
                OkResult = "Index defaulted to 0. Havok data replaced";
            }
            else if( Index >= newAnimationsLength )
            {
                var animationsMax = newAnimationsLength - 1;
                Index = animationsMax;
                OkResult = "Index defaulted to " + animationsMax + ". Havok data replaced";
            }

            Command.AddAndExecute( new PapHavokCommand( Motion.File, () => {
                var container = Motion.File.MotionData.AnimationContainer;

                // Do this so we can undo the change later if necessary
                var anims = HavokData.ToList( container->Animations );
                var bindings = HavokData.ToList( container->Bindings );
                anims[HavokIndex] = newAnimation.AnimationContainer->Animations[Index];
                bindings[HavokIndex] = newAnimation.AnimationContainer->Bindings[Index];

                container->Animations = HavokData.CreateArray( Motion.File.Handles, ( uint )container->Animations.Flags, anims, sizeof( nint ) );
                container->Bindings = HavokData.CreateArray( Motion.File.Handles, ( uint )container->Bindings.Flags, bindings, sizeof( nint ) );
            } ) );
            Dalamud.OkNotification( OkResult );
        }
    }
}
