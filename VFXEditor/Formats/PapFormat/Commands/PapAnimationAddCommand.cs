using System.Collections.Generic;
using VfxEditor.Data.Command.ListCommands;

namespace VfxEditor.PapFormat {
    public class PapAnimationAddCommand : ListAddCommand<PapAnimation> {
        public readonly PapFile File;

        public PapAnimationAddCommand( PapFile file, List<PapAnimation> group, PapAnimation item ) : base( group, item ) {
            File = file;
        }

        public PapAnimationAddCommand( PapFile file, List<PapAnimation> group, PapAnimation item, int idx ) : base( group, item, idx ) {
            File = file;
        }

        public override void Execute() {
            base.Execute();
            File.RefreshHavokIndexes();
        }

        public override void Redo() {
            base.Redo();
            File.RefreshHavokIndexes();
        }

        public override void Undo() {
            base.Undo();
            File.RefreshHavokIndexes();
        }
    }
}
