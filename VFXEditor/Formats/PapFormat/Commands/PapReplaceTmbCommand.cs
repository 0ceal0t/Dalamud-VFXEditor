using VfxEditor.TmbFormat;

namespace VfxEditor.PapFormat {
    public class PapReplaceTmbCommand : ICommand {
        private readonly PapAnimation Item;
        private readonly TmbFile State;
        private TmbFile PrevState;

        public PapReplaceTmbCommand( PapAnimation item, TmbFile state ) {
            Item = item;
            State = state;
        }

        public void Execute() {
            PrevState = Item.Tmb;
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
