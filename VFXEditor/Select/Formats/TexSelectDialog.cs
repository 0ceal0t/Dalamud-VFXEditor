using System;
using VfxEditor.Formats.TextureFormat;
using VfxEditor.Select.Tabs.Actions;
using VfxEditor.Select.Tabs.Statuses;

namespace VfxEditor.Select.Formats {
    public class TexSelectDialog : SelectDialog {
        public TexSelectDialog( string id, TextureManager manager, bool showLocal, Action<SelectResult> action ) : base( id, "atex", manager, showLocal, action ) {
            GameTabs.AddRange( [
                new ActionTabTex( this, "Action" ),
                new StatusTabTex( this, "Status" ),
            ] );
        }
    }
}