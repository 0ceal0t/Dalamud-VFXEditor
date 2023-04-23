namespace VfxEditor.Select.Scd.Voice {
    public class VoiceRow {
        public readonly string Name;
        public readonly string Id;
        public readonly int RowId;

        public VoiceRow( int rowId, string path, string name ) {
            RowId = rowId;
            Id = path;
            Name = name;
        }
    }
}
