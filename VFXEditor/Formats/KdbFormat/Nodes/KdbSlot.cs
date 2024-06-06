using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbSlot : Slot {
        public readonly ConnectionType Type;
        public readonly Dictionary<Slot, KdbSlotData> Data = [];

        public KdbSlot( ConnectionType type, bool acceptMultiple = false ) : base( $"{type}", acceptMultiple ) {
            Type = type;
        }

        public override void DrawPopup( Slot target ) {
            if( Data.TryGetValue( target, out var data ) ) {
                data.Draw();
                return;
            }

            var newData = new KdbSlotData();
            Data[target] = newData;
            newData.Draw();
        }

        public void Connect( Slot target, ParsedFnvHash name, double coeff, uint unknown ) {
            Connect( target );
            Data[target] = new( name, coeff, unknown );
        }
    }

    public class KdbSlotData {
        public readonly ParsedFnvHash Name = new( "Name" );
        public readonly ParsedDouble Coeff = new( "Coefficient", 1 );
        public readonly ParsedUInt Unknown = new( "Unknown", 0 );

        public KdbSlotData() { }

        public KdbSlotData( ParsedFnvHash name, double coeff, uint unknown ) {
            Name = name;
            Coeff.Value = coeff;
            Unknown.Value = unknown;
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "SlotData" );
            Name.Draw();
            Coeff.Draw();
            Unknown.Draw();
        }
    }
}
