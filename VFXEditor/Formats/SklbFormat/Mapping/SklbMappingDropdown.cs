using ImGuiFileDialog;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Interop.Havok;
using VfxEditor.Ui.Components;

namespace VfxEditor.SklbFormat.Mapping {
    public class SklbMappingDropdown : Dropdown<SklbMapping> {
        private readonly SklbFile File;

        public SklbMappingDropdown( SklbFile file, List<SklbMapping> items ) : base( "Mappings", items, false, false ) {
            File = file;
        }

        protected override string GetText( SklbMapping item, int idx ) => $"Mapping {idx}" + ( string.IsNullOrEmpty( item.Name.Value ) ? "" : $" ({item.Name.Value})" );

        protected override void OnNew() {
            FileDialogManager.OpenFileDialog( "Select a Skeleton", "Skeleton{.hkx,.sklb},.*", ( ok, res ) => {
                if( !ok ) return;

                var hkxPath = res;
                if( res.EndsWith( ".sklb" ) ) {
                    SimpleSklb.LoadFromLocal( res ).SaveHavokData( SklbMapping.TempMappingHkx );
                    hkxPath = SklbMapping.TempMappingHkx;
                }

                var havokData = new HavokBones( hkxPath, true );

                // var newMapping = new SklbMapping( File.Bones, mapper, "hkaSkeletonMapper" );

                // CommandAction.Invoke().Add( new GenericAddCommand<T>( Items, NewAction.Invoke(), OnChangeAction ) );
            } );
        }

        protected override void OnDelete( SklbMapping item ) => CommandManager.Sklb.Add( new GenericRemoveCommand<SklbMapping>( Items, item ) );

        protected override void DrawSelected() => Selected.Draw();
    }
}
