using System.Collections.Generic;

namespace VfxEditor.UldFormat.Timeline.Frames {
    public class UldKeyGroupDataCommand : ICommand {
        private readonly UldKeyGroup Item;
        private readonly List<UldKeyframe> OldKeyframes = new();

        public UldKeyGroupDataCommand( UldKeyGroup item ) {
            Item = item;
        }

        public void Execute() {
            OldKeyframes.AddRange( Item.Keyframes );
            Item.Keyframes.Clear();
        }

        public void Redo() {
            Item.Keyframes.Clear();
        }

        public void Undo() {
            Item.Keyframes.Clear();
            Item.Keyframes.AddRange( OldKeyframes );
        }
    }
}
