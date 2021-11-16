using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Helper;
using VFXEditor.Tmb;

namespace VFXEditor.Pap {
    public class PapAnimation {
        private string Name = ""; // padded to 32 bytes (INCLUDING NULL)
        private short Unknown1 = 0;
        private short HavokIndex = 0;
        private int Unknown2 = 0;

        private TmbFile Tmb;

        public PapAnimation() {
            Tmb = new TmbFile();
        }

        public PapAnimation( BinaryReader reader ) {
            Name = FileHelper.ReadString( reader );
            reader.ReadBytes( 32 - Name.Length - 1 );
            Unknown1 = reader.ReadInt16();
            HavokIndex = reader.ReadInt16();
            Unknown2 = reader.ReadInt32();
        }

        public void Write( BinaryWriter writer ) {
            FileHelper.WriteString( writer, Name, true );
            for (var i = 0; i < (32 - Name.Length - 1); i++) {
                writer.Write( ( byte )0 );
            }
            writer.Write( Unknown1 );
            writer.Write( HavokIndex );
            writer.Write( Unknown2 );
        }

        public void ReadTmb( BinaryReader reader ) {
            Tmb = new TmbFile( reader, false );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name;

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Havok Index{id}", ref HavokIndex );
            ImGui.InputText( $"Name{id}", ref Name, 31 );
            FileHelper.ShortInput( $"Unknown 1{id}", ref Unknown1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unknown2 );

            Tmb.Draw( id + "/Tmb" );
        }
    }
}
