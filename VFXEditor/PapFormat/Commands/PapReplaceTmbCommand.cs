using VfxEditor.TmbFormat;

namespace VfxEditor.PapFormat {
    public class PapReplaceTmbCommand : ICommand {
        private readonly PapAnimation Item;
        private readonly TmbFile State;
        private readonly TmbFile PrevState;

        public PapReplaceTmbCommand( PapAnimation item, TmbFile state ) {
            Item = item;
            State = state;
            PrevState = item.Tmb;
        }

        public void Execute() {
            Item.Tmb = State;
        }

        public void Redo() {
            Item.Tmb = State;
        }

        public void Undo() {
            Item.Tmb = PrevState;
        }
    }
}
