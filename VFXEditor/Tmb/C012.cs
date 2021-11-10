using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VFXEditor.Tmb {
    public class C012 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 30;
        private int Unk_3 = 0;
        private string Path = "";
        private short BindPoint_1 = 1;
        private short BindPoint_2 = 0xFF;
        private short BindPoint_3 = 2;
        private short bindPoint_4 = 0xFF;
        private List<List<float>> Unk_8 = GetDefault();

        public static readonly string Name = "VFX (C012)";

        private static List<List<float>> GetDefault() {
            var ret = new List<List<float>>();
            var a = new[] { 1f, 1f, 1f };
            var b = new[] { 0f, 0f, 0f };
            var c = new[] { 0f, 0f, 0f };
            var d = new[] { 1f, 1f, 1f, 1f };

            ret.Add( new List<float>( a ) );
            ret.Add( new List<float>( b ) );
            ret.Add( new List<float>( c ) );
            ret.Add( new List<float>( d ) );
            return ret;
        }

        public C012() { }
        public C012( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C012] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // 30?
            Unk_3 = reader.ReadInt32(); // 0?

            var offset = reader.ReadInt32(); // path offset: [C012] + offset + 8 = path
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = TmbFile.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            BindPoint_1 = reader.ReadInt16(); // 1?
            BindPoint_2 = reader.ReadInt16(); // FF?
            BindPoint_3 = reader.ReadInt16(); // 2?
            bindPoint_4 = reader.ReadInt16(); // FF?

            Unk_8 = new();

            for(var i = 0; i < 5; i++) {
                var Unk_8_2 = new List<float>();

                var offset2 = reader.ReadInt32(); // offset [C012] + offset + 8
                var count = reader.ReadInt32(); // count

                var savePos2 = reader.BaseStream.Position;
                reader.BaseStream.Seek( startPos + offset2, SeekOrigin.Begin );
                for (var j = 0; j < count; j++) {
                    var item = reader.ReadSingle();
                    Unk_8_2.Add( item );
                }
                reader.BaseStream.Seek( savePos2, SeekOrigin.Begin );

                Unk_8.Add( Unk_8_2 );
            }
        }

        public override int GetSize() => 0x48;
        public override int GetStringSize() => Path.Length + 1;
        public override int GetExtraSize() => 4 * Unk_8.Select(x => x.Count).Sum();

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = ( int )stringWriter.BaseStream.Position + stringPos;
            var offset = endPos - startPos - 8;

            TmbFile.WriteString( entryWriter, "C012" );
            entryWriter.Write( 0x48 );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );

            entryWriter.Write( offset );
            TmbFile.WriteString( stringWriter, Path, true );

            entryWriter.Write( BindPoint_1 );
            entryWriter.Write( BindPoint_2 );
            entryWriter.Write( BindPoint_3 );
            entryWriter.Write( bindPoint_4 );

            foreach (var entry in Unk_8) {
                var extraEndPos = ( int )extraWriter.BaseStream.Position + extraPos;
                var extraOffset = extraEndPos - startPos - 8;

                var count = entry.Count;
                entryWriter.Write( count == 0 ? 0 : extraOffset );
                entryWriter.Write( count );

                foreach (var item in entry) {
                    extraWriter.Write( item );
                }
            }
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            TmbFile.ShortInput( $"Bind Point 1{id}", ref BindPoint_1 );
            TmbFile.ShortInput( $"Bind Point 2{id}", ref BindPoint_2 );
            TmbFile.ShortInput( $"Bind Point 3{id}", ref BindPoint_3 );
            TmbFile.ShortInput( $"Bind Point 4{id}", ref bindPoint_4 );

            for (var i = 0; i < Unk_8.Count; i++) {
                var entries = Unk_8[i];
                for (var j = 0; j < entries.Count; j++) {
                    var item = entries[j];
                    if (ImGui.InputFloat($"Unknown 8 ({i},{j}){id}", ref item)) {
                        Unk_8[i][j] = item;
                    }
                }
            }
        }
    }
}