using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.FileBrowser;
using VfxEditor.Interop.Havok;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public unsafe class PapAnimationDropdown : Dropdown<PapAnimation> {
        private readonly PapFile File;

        public PapAnimationDropdown( PapFile file, List<PapAnimation> items ) : base( "Animations", items ) {
            File = file;
        }

        public override string GetText( PapAnimation item, int idx ) => item.GetName();

        protected override void DrawControls() => DrawNewDeleteControls( OnNew, OnDelete );

        private void OnNew() {
            FileBrowserManager.OpenFileDialog( "Select a File", "Animation{.hkx,.pap}", ( bool ok, string res ) => {
                if( ok ) Plugin.AddModal( new PapAddModal( File, res ) );
            } );
        }

        private void OnDelete( PapAnimation item ) {
            var index = Items.IndexOf( item );

            var command = new CompoundCommand( [
                new ListRemoveCommand<PapAnimation>( Items, item, ( PapAnimation item, bool remove ) => item.File.RefreshHavokIndexes() ),
                new PapHavokCommand( File, () => {
                    var container = File.MotionData.AnimationContainer;

                    var anims = HavokData.ToList( container->Animations );
                    var bindings = HavokData.ToList( container->Bindings );
                    anims.RemoveAt( index );
                    bindings.RemoveAt( index );

                    container->Animations = HavokData.CreateArray( File.Handles, ( uint )container->Animations.Flags, anims, sizeof( nint ) );
                    container->Bindings = HavokData.CreateArray( File.Handles, ( uint )container->Bindings.Flags, bindings, sizeof( nint ) );
                } )
            ] );
            CommandManager.Add( command );

            UiUtils.OkNotification( "Havok data removed" );
        }

        protected override void DrawSelected() => Selected.Draw();
    }
}
