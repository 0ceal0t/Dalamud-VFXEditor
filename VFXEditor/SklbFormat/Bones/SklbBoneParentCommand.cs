namespace VfxEditor.SklbFormat.Bones {
    public class SklbBoneParentCommand : ICommand {
        private readonly SklbBone Item;
        private readonly SklbBone State;
        private readonly SklbBone PrevState;

        public SklbBoneParentCommand( SklbBone item, SklbBone parent ) {
            Item = item;
            State = parent;
            PrevState = item.Parent;
        }

        public void Execute() {
            Item.MakeChildOf( State );
        }

        public void Redo() {
            Item.MakeChildOf( State );
        }

        public void Undo() {
            Item.MakeChildOf( PrevState );
        }
    }
}