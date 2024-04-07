namespace VfxEditor.AvfxFormat {
    public abstract class AvfxDataWithParameters : AvfxData {
        public readonly UiDisplayList ParameterTab = new( "Parameters" );

        public AvfxDataWithParameters( bool optional = false ) : base( optional ) {
            DisplayTabs.Add( ParameterTab );
        }
    }
}
