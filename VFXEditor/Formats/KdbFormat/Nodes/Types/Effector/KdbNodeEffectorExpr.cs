using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Effector {
    public class KdbNodeEffectorExpr : KdbNode {
        public override KdbNodeType Type => KdbNodeType.EffectorExpr;

        public readonly ParsedString Expression = new( "Expression" );
        public readonly List<ParsedDouble4> Values = [];
        private readonly CommandListView<ParsedDouble4> ValueList;

        public KdbNodeEffectorExpr() : base() {
            ValueList = new( Values, () => new( "##Value" ), true );
        }

        public KdbNodeEffectorExpr( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            var valueCount = reader.ReadUInt32();
            var valuePosition = reader.BaseStream.Position + reader.ReadUInt32();
            var expressionLength = reader.ReadInt32();
            var expressionPosition = reader.BaseStream.Position + reader.ReadUInt32();

            reader.BaseStream.Position = valuePosition;
            for( var i = 0; i < valueCount; i++ ) {
                var newValue = new ParsedDouble4( "##Value" );
                newValue.Read( reader );
                Values.Add( newValue );
            }

            reader.BaseStream.Position = expressionPosition;
            Expression.Value = FileUtils.ReadString( reader, expressionLength );

            var ignoreStart = ( int )reader.BaseStream.Position;
            FileUtils.PadTo( reader, 8 );
            KdbFile.VerifyIgnore.Add( (ignoreStart, ( int )reader.BaseStream.Position) );
        }

        public override void WriteBody( BinaryWriter writer ) {
            var placeHolderPos = writer.BaseStream.Position;
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( 0 );

            var valuePosition = writer.BaseStream.Position;
            foreach( var value in Values ) value.Write( writer );

            var expressionPosition = writer.BaseStream.Position;
            var expression = Expression.Value.Trim().Trim( '\0' );
            FileUtils.WriteString( writer, expression, true );
            FileUtils.PadTo( writer, 8 );

            var savePos = writer.BaseStream.Position;
            writer.BaseStream.Position = placeHolderPos;
            writer.Write( Values.Count );
            writer.Write( ( uint )( valuePosition - writer.BaseStream.Position ) );
            writer.Write( expression.Length + 1 );
            writer.Write( ( uint )( expressionPosition - writer.BaseStream.Position ) );
            writer.BaseStream.Position = savePos;
        }

        protected override void DrawBody( List<string> bones ) {
            Expression.Draw();
            ImGui.Separator();
            ValueList.Draw();
        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input, true ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
