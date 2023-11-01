using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Shader {
    public class MtrlConstant : IUiItem {
        private readonly ParsedCrc Id = new( "Id" );
        private readonly List<ParsedFloat> Values = new();

        private readonly ushort TempOffset;
        private readonly ushort TempSize;

        private readonly ListView<ParsedFloat> ValueView;

        public MtrlConstant() {
            ValueView = new( Values, () => new( "##Value" ), true );
        }

        public MtrlConstant( BinaryReader reader ) : this() {
            Id.Read( reader );
            TempOffset = reader.ReadUInt16();
            TempSize = reader.ReadUInt16();
        }

        public void PickValues( List<float> values ) {
            foreach( var item in values.GetRange( TempOffset / 4, TempSize / 4 ) ) {
                Values.Add( new ParsedFloat( "##Value", item ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            // TODO
        }

        public void Draw() {
            Id.Draw( CrcMaps.MaterialParams );
            ValueView.Draw();
        }
    }
}
