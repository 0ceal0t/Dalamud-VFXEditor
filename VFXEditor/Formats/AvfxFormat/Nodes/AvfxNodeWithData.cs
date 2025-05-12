using System;
using VfxEditor.AvfxFormat;
using VfxEditor.Formats.AvfxFormat.Components.Enums;
using VfxEditor.Parsing.Data;

namespace VfxEditor.Formats.AvfxFormat.Nodes {
    public abstract class AvfxNodeWithData<T> : AvfxNode, IItemWithData<AvfxData> where T : Enum {
        protected AvfxData? Data;
        protected AvfxDataEnum<T> Type;

        public AvfxNodeWithData( string avfxName, uint graphColor, string typeAvfxName ) : base( avfxName, graphColor ) {
            Type = new( this, "Type", typeAvfxName );
        }

        public AvfxData GetData() => Data;

        public void SetData( AvfxData data ) { Data = data; }

        public abstract void UpdateData();
    }
}
