namespace VfxEditor.UldFormat.Component.Node {
    public class UldNodeDataCommand : ICommand {
        private readonly UldNode Item;
        private UldGenericData OldData;
        private UldGenericData NewData;

        private readonly bool DoSwitchComponent = false;
        private readonly bool SwitchComponentState;

        public UldNodeDataCommand( UldNode item, bool doSwitchComponent = false ) {
            Item = item;
            DoSwitchComponent = doSwitchComponent;
            if( DoSwitchComponent ) SwitchComponentState = item.IsComponentNode;
        }

        public void Execute() {
            OldData = Item.Data;
            // Component state already updated
            Item.UpdateData();
            NewData = Item.Data;
        }

        public void Redo() {
            if( DoSwitchComponent ) Item.IsComponentNode = SwitchComponentState;
            Item.Data = NewData;
        }

        public void Undo() {
            if( DoSwitchComponent ) Item.IsComponentNode = !SwitchComponentState;
            Item.Data = OldData;
        }
    }
}
