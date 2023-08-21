using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.PapFormat {
    public class PapAnimationRemoveCommand : GenericRemoveCommand<PapAnimation> {
        public readonly PapFile File;

        public PapAnimationRemoveCommand( PapFile file, List<PapAnimation> group, PapAnimation item ) : base( group, item ) {
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
