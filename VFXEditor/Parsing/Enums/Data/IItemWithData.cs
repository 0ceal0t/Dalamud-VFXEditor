namespace VfxEditor.Parsing.Data {
    public interface IItemWithData<S> where S : class, IData {
        public void UpdateData();

        public void SetData( S data );

        public S GetData();
    }
}
