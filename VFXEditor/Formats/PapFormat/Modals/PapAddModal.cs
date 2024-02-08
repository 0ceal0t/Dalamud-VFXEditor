using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Interop.Havok;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public unsafe class PapAddModal : Modal {
        private readonly PapFile File;
        private readonly string ImportPath;
        private int Index;

        public PapAddModal( PapFile file, string importPath ) : base( "Animation Import Index", true ) {
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
            animation.ReadTmb( Path.Combine( Plugin.RootLocation, "Files", "default_pap_tmb.tmb" ) );

            var commands = new List<ICommand> {
                new ListAddCommand<PapAnimation>( File.Animations, animation, ( PapAnimation item, bool add ) => item.File.RefreshHavokIndexes()  ),
                new PapHavokCommand( File, () => {
                    var newAnimation = new HavokData( ImportPath, true );
                    var container = File.MotionData.AnimationContainer;

                    var anims = HavokData.ToList( container->Animations );
                    var bindings = HavokData.ToList( container->Bindings );
                    anims.Add( newAnimation.AnimationContainer->Animations[Index] );
                    bindings.Add( newAnimation.AnimationContainer->Bindings[Index] );

                    container->Animations = HavokData.CreateArray( File.Handles, ( uint )container->Animations.Flags, anims, sizeof( nint ) );
                    container->Bindings = HavokData.CreateArray( File.Handles, ( uint )container->Bindings.Flags, bindings, sizeof( nint ) );
                } )
            };
            Command.AddAndExecute( new CompoundCommand( commands ) );

            UiUtils.OkNotification( "Havok data added" );
        }
    }
}
