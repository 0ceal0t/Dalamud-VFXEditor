namespace VfxEditor.Parsing.Data {
    public class ParsedDataEnumCommand<T, S> : ICommand where S : class, IData {
        private readonly ParsedSimpleCommand<T> Command;
        private readonly IItemWithData<S> Item;
        private S OldData;
        private S NewData;

        public ParsedDataEnumCommand( ParsedSimpleCommand<T> command, IItemWithData<S> item ) {
            Command = command;
            Item = item;
        }

        public void Execute() {
            OldData = Item.GetData();
            OldData?.Disable();

            Command.Execute();

            Item.UpdateData();
            NewData = Item.GetData();
        }

        public void Redo() {
            OldData?.Disable();

            Command.Redo();

            Item.SetData( NewData );
            NewData?.Enable();
        }

        public void Undo() {
            NewData?.Disable();

            Command.Undo();

            Item.SetData( OldData );
            OldData?.Enable();
        }
    }
}
