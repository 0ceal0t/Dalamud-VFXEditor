using System;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Select.Tex.Action;
using VfxEditor.Select.Tex.Status;

namespace VfxEditor.Select.Tex {
    public class TexSelectDialog : SelectDialog {
        public TexSelectDialog( string id, TextureManager manager, bool showLocal, Action<SelectResult> action ) : base( id, "atex", manager, showLocal, action ) {
            GameTabs.AddRange( new SelectTab[]{
                new ActionTab( this, "Action" ),
                new StatusTab( this, "Status" ),
            } );
        }
    }
}
