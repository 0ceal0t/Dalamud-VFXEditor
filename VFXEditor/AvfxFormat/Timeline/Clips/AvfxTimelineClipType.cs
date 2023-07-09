using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineClipType : IUiItem {
        private static readonly Dictionary<string, string> IdOptions = new() {
            { "LLIK", "Kill" },
            { "TSER", "Reset" },
            { " DNE", "End" },
            { "IDAF", "Fade In" },
            { "PLLU", "Unlock Loop Point" },
            { " GRT", "Trigger" },
            { "GRTR", "Random Trigger" }
        };

        public string Value = "LLIK";

        public string Text => IdOptions[Value];

        public AvfxTimelineClipType() { }

        public void Read( BinaryReader reader ) {
            Value = Encoding.ASCII.GetString( reader.ReadBytes( 4 ) );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Encoding.ASCII.GetBytes( Value ) );
        }

        public void Draw() {
            using var combo = ImRaii.Combo( "Type", Text );
            if( !combo ) return;

            foreach( var key in IdOptions.Keys ) {
                if( ImGui.Selectable( IdOptions[key], Value == key ) ) {
                    CommandManager.Avfx.Add( new AvfxTimelineClipTypeCommand( this, key ) );
                }
            }
        }
    }
}
