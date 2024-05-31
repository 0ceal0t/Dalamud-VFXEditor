using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbSlot : Slot {
        public readonly ConnectionType Type;
        private readonly Dictionary<Slot, KdbSlotData> Data = [];

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

        public void Connect( Slot target, double coeff, uint unknown ) {
            Connect( target );
            Data[target] = new( coeff, unknown );
        }
    }

    public class KdbSlotData {
        public readonly ParsedDouble Coeff = new( "Coefficient", 1 );
        public readonly ParsedUInt Unknown = new( "Unknown", 0 );

        public KdbSlotData() { }

        public KdbSlotData( double coeff, uint unknown ) {
            Coeff.Value = coeff;
            Unknown.Value = unknown;
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "SlotData" );
            Coeff.Draw();
            Unknown.Draw();
        }
    }
}
