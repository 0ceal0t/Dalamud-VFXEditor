using VfxEditor.Formats.AvfxFormat.Components.UiComponents;

namespace VfxEditor.Formats.AvfxFormat.Base {
    public abstract class AvfxDataWithParameters : AvfxData {
        public readonly UiDisplayList ParameterTab = new( "Parameters" );

        public AvfxDataWithParameters( bool optional = false ) : base( optional ) {
            Tabs.Add( ParameterTab );
        }
    }
}
