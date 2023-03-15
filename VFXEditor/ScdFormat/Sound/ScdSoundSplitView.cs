using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.ScdFormat.Sound {
    public class ScdSoundSplitView : ScdSimpleSplitView<ScdSoundEntry> {
        public ScdSoundSplitView( string itemName, List<ScdSoundEntry> items ) : base( itemName, items, true ) {
        }

        protected override void OnNew() {
            CommandManager.Scd.Add( new GenericAddCommand<ScdSoundEntry>( Items, new ScdSoundEntry() ) );
        }

        protected override void OnDelete( ScdSoundEntry item ) {
            CommandManager.Scd.Add( new GenericRemoveCommand<ScdSoundEntry>( Items, item ) );
        }
    }
}
