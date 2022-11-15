using ImGuiNET;
using System.IO;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class Tmpp : TmbItem {
        public override string Magic => "TMPP";
        public override int Size => 0x0C;
        public override int ExtraSize => 0;

        public bool IsAssigned => Assigned;
        private bool Assigned = false;

        private string Path = "";

        public Tmpp( TmbReader reader ) : base( reader ) {
            var savePos = reader.Reader.BaseStream.Position;
            var magic = reader.ReadString( 4 );// TMAL or TMPP

            if( magic == "TMPP" ) { // TMPP
                Assigned = true;
                reader.ReadInt32(); // 0x0C
                Path = reader.ReadOffsetString();
            }
            else { // TMAL, reset
                reader.Reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            }
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.WriteOffsetString( Path );
        }

        public void Draw( string id ) {
            ImGui.Checkbox( $"Use face library{id}", ref Assigned );
            ImGui.SameLine();
            UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Facial-Expressions" );

            if( Assigned ) {
                ImGui.InputText( $"Face library path{id}", ref Path, 256 );
            }
        }
    }
}
