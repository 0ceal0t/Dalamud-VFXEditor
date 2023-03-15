using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.ScdFormat.Sound {
    public class ScdSoundSplitView : ScdSimpleSplitView<ScdSoundEntry> {
        public ScdSoundSplitView( List<ScdSoundEntry> items ) : base( "Sound", items, true ) {
        }

        protected override void OnNew() {
            CommandManager.Scd.Add( new GenericAddCommand<ScdSoundEntry>( Items, new ScdSoundEntry() ) );
        }

        protected override void OnDelete( ScdSoundEntry item ) {
            CommandManager.Scd.Add( new GenericRemoveCommand<ScdSoundEntry>( Items, item ) );
        }
    }
}
