using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public class Tmpp : TmbItem {
        public override string Magic => "TMPP";
        public override int Size => 0x0C;
        public override int ExtraSize => 0;

        public bool IsAssigned => Assigned.Value == true;
        private readonly ParsedByteBool Assigned = new( "Use Face Library", value: false );
        private readonly TmbOffsetString Path = new( "Face Library Path" );

        public Tmpp( TmbFile file, TmbReader reader ) : base( file, reader ) {
            reader.Reader.BaseStream.Position = reader.Reader.BaseStream.Position - 8; // go back before magic and size

            var savePos = reader.Reader.BaseStream.Position;
            var magic = reader.ReadString( 4 );// TMAL or TMPP

            if( magic == "TMPP" ) { // TMPP
                Assigned.Value = true;
                reader.ReadInt32(); // 0x0C
                Path.Read( reader );
            }
            else { // TMAL, ignore
                reader.Reader.BaseStream.Position = savePos;
            }
        }

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            Path.Write( writer );
        }

        public void Draw() {
            Assigned.Draw();
            ImGui.SameLine();
            UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Facial-Expressions" );

            if( IsAssigned ) Path.Draw();
        }
    }
}
