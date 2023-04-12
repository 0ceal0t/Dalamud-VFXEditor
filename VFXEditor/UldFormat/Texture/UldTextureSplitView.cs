using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.UldFormat.PartList;

namespace VfxEditor.UldFormat.Texture {
    public class UldTextureSplitView : SimpleSplitView<UldTexture> {
        public UldTextureSplitView( List<UldTexture> items ) : base( "Texture", items, true ) { }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldTexture>( Items, new UldTexture() ) );
        }

        protected override void OnDelete( UldTexture item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldTexture>( Items, item ) );
        }

        protected override string GetText( UldTexture item, int idx ) => item.GetText();
    }
}
