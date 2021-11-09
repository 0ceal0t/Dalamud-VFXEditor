using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VFXEditor.Tmb {
    public class TmbTmac {
        private short Time;
        private int Unk_2;
        private int Unk_3;
        private short Slot;
        private int Tmtr_Count;
        private List<TmbTmtr> TMTR = new();

        public int EntrySize => TMTR.Select( x => x.EntrySize ).Sum();
        public int ExtraSize => TMTR.Select( x => x.ExtraSize ).Sum();
        public int StringSize => TMTR.Select( x => x.StringSize ).Sum();
        public int EntryCount => TMTR.Select( x => x.EntryCount ).Sum();

        public TmbTmac(BinaryReader reader) {
            var startPos = reader.BaseStream.Position; // [TMAC]

            reader.ReadInt32(); // TMAC
            reader.ReadInt32(); // 0x1C
            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // some count ?
            Unk_3 = reader.ReadInt32(); // some count ?
            var offset = reader.ReadInt32(); // before [TMAC] + offset + 8 = spot on timeline
            Tmtr_Count = reader.ReadInt32(); // number of TMTR

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset + 8 + 2 * (Tmtr_Count - 1), SeekOrigin.Begin );
            Slot = reader.ReadInt16();
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public void ReadTmtr(BinaryReader reader) {
            for( var i = 0; i < Tmtr_Count; i++ ) {
                TMTR.Add( new TmbTmtr( reader ) );
            }
        }

        public void ReadEntries(BinaryReader reader) {
            foreach( var tmtr in TMTR ) tmtr.ReadEntries( reader );
        }

        public void Write( BinaryWriter headerWriter, int timelinePos, ref short id ) {
            var startPos = ( int )headerWriter.BaseStream.Position;
            var endPos = timelinePos + ( Slot - 2 ) * 2;
            var offset = endPos - startPos - 8 - 2 * (TMTR.Count - 1);

            TmbFile.WriteString( headerWriter, "TMAC" );
            headerWriter.Write( 0x1C );
            headerWriter.Write( id++ );
            headerWriter.Write( Time);
            headerWriter.Write( Unk_2);
            headerWriter.Write( Unk_3);
            headerWriter.Write( offset );
            headerWriter.Write( TMTR.Count );
        }

        public void WriteTmtr( BinaryWriter entryWriter, int entryPos, int timelinePos, ref short id ) {
            foreach( var tmtr in TMTR ) tmtr.Write( entryWriter, entryPos, timelinePos, ref id );
        }

        public void WriteEntries( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos, ref short id ) {
            foreach( var tmtr in TMTR ) tmtr.WriteEntries( entryWriter, entryPos, extraWriter, extraPos, stringWriter, stringPos, timelinePos, ref id );
        }

        public void Draw(string id) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_3 );
            TmbFile.ShortInput( $"Slot{id}", ref Slot );

            var i = 0;
            foreach( var tmtr in TMTR ) {
                if( ImGui.CollapsingHeader( $"TMTR {i}{id}{i}" ) ) {
                    ImGui.Indent();
                    tmtr.Draw( $"{id}{i}" );
                    ImGui.Unindent();
                }
                i++;
            }
        }
    }
}
