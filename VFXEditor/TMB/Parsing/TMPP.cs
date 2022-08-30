using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Parsing {
    public class TMPP {
        public int Size => Assigned ? 0x0C : 0;
        public int PathSize => Assigned ? Path.Length + 1 : 0;

        public bool Assigned = false;
        public string Path = "";

        public TMPP( BinaryReader reader ) {
            var magic = reader.ReadInt32(); // TMAL or TMPP
            if( magic == 0x50504D54 ) { // TMPP
                Assigned = true;
                reader.ReadInt32(); // 0x0C
                var offset = reader.ReadInt32(); // offset from [TMPP] + 8 to strings
                Path = FileHelper.ReadStringRelativeOffset( reader, offset - 4 );
            }
        }

        public void Write( BinaryWriter writer ) {
            // TODO
        }

        public void Draw( string id ) {

            ImGui.Checkbox( $"Use face library{id}", ref Assigned );
            ImGui.SameLine();
            UIHelper.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Facial-Expressions" );

            if( Assigned ) {
                ImGui.InputText( $"Face library path{id}", ref Path, 256 );
            }
        }
    }
}
