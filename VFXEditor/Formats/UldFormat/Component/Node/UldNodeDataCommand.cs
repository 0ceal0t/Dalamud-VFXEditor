namespace VfxEditor.UldFormat.Component.Node {
    public class UldNodeDataCommand : ICommand {
        private readonly UldNode Item;
        private readonly UldGenericData State;
        private readonly UldGenericData PrevState;
        private readonly bool DoSwitchComponent = false;
        private readonly bool ComponentState;

        public UldNodeDataCommand( UldNode item, bool doSwitchComponent = false ) {
            Item = item;
            DoSwitchComponent = doSwitchComponent;
            ComponentState = item.IsComponentNode;

            PrevState = Item.Data;
            // Component state already updated
            Item.UpdateData();
            State = Item.Data;
        }
        public void Redo() {
            if( DoSwitchComponent ) Item.IsComponentNode = ComponentState;
            Item.Data = State;
        }

        public void Undo() {
            if( DoSwitchComponent ) Item.IsComponentNode = !ComponentState;
            Item.Data = PrevState;
        }
    }
}
