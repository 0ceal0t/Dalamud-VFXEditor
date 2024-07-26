namespace VfxEditor.Formats.PbdFormat {
    public class PbdConnectionCommand : ICommand {
        public readonly PbdConnection Item;

        public readonly PbdConnection Parent;
        public readonly PbdConnection PrevParent;

        public readonly PbdConnection Child;
        public readonly PbdConnection PrevChild;

        public readonly PbdConnection Sibling;
        public readonly PbdConnection PrevSibling;

        public PbdConnectionCommand( PbdConnection item, PbdConnection parent, PbdConnection child, PbdConnection sibling ) {
            Item = item;
            Parent = parent;
            Child = child;
            Sibling = sibling;

            PrevParent = Item.Parent;
            PrevChild = Item.Child;
            PrevSibling = Item.Sibling;

            Redo();
        }

        public void Redo() {
            Item.Parent = Parent;
            Item.Child = Child;
            Item.Sibling = Sibling;
        }

        public void Undo() {
            Item.Parent = PrevParent;
            Item.Child = PrevChild;
            Item.Sibling = PrevSibling;
        }

        public static PbdConnectionCommand SetParent( PbdConnection item, PbdConnection parent ) => new( item, parent, item.Child, item.Sibling );
        public static PbdConnectionCommand SetChild( PbdConnection item, PbdConnection child ) => new( item, item.Parent, child, item.Sibling );
        public static PbdConnectionCommand SetSibling( PbdConnection item, PbdConnection sibling ) => new( item, item.Parent, item.Child, sibling );
    }
}
