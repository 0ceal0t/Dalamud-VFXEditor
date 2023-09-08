using ImGuiNET;
using System.IO;
using VfxEditor.Interop.Havok;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public unsafe class PapAddModal : Modal {
        private readonly PapFile File;
        private readonly string ImportPath;
        private int Index;

        public PapAddModal( PapFile file, string importPath ) : base( "Animation Import Index" ) {
            File = file;
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
            var animation = new PapAnimation( File, File.HkxTempLocation );
            animation.ReadTmb( Path.Combine( Plugin.RootLocation, "Files", "default_pap_tmb.tmb" ), File.Command );

            var command = new CompoundCommand();
            command.Add( new PapAnimationAddCommand( File, File.Animations, animation ) );
            command.Add( new PapHavokCommand( File, () => {
                var newAnimation = new HavokData( ImportPath );
                var container = File.MotionData.AnimationContainer;

                var anims = HavokData.ToList( container->Animations );
                var bindings = HavokData.ToList( container->Bindings );
                anims.Add( newAnimation.AnimationContainer->Animations[Index] );
                bindings.Add( newAnimation.AnimationContainer->Bindings[Index] );

                container->Animations = HavokData.CreateArray( container->Animations.Flags, anims, sizeof( nint ), out var _ );
                container->Bindings = HavokData.CreateArray( container->Bindings.Flags, bindings, sizeof( nint ), out var _ );
            } ) );
            File.Command.Add( command );

            UiUtils.OkNotification( "Havok data added" );
        }
    }
}
