using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VFXEditor.Tmb {
    public class TmbActor {
        public short Id { get; private set; } // temp
        private int TrackCount;

        private short Time = 0;
        private int Unk_2 = 0;
        private int Unk_3 = 0;

        private List<TmbTrack> Tracks = new();

        public int EntrySize => Tracks.Select( x => x.EntrySize ).Sum();
        public int ExtraSize => Tracks.Select( x => x.ExtraSize ).Sum();
        public int StringSize => Tracks.Select( x => x.StringSize ).Sum();
        public int EntryCount => Tracks.Select( x => x.EntryCount ).Sum();

        public TmbActor() { }
        public TmbActor(BinaryReader reader) {
            var startPos = reader.BaseStream.Position;

            reader.ReadInt32(); // TMAC
            reader.ReadInt32(); // 0x1C
            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // some count ?
            Unk_3 = reader.ReadInt32(); // some count ?
            var offset = reader.ReadInt32(); // before [TMAC] + offset + 8 = spot on timeline
            TrackCount = reader.ReadInt32(); // number of TMTR

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset + 8 + 2 * ( TrackCount - 1), SeekOrigin.Begin );
            var endId = reader.ReadInt16();
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public void ReadTracks(BinaryReader reader) {
            for( var i = 0; i < TrackCount; i++ ) {
                Tracks.Add( new TmbTrack( reader ) );
            }
        }

        public void ReadEntries(BinaryReader reader) {
            foreach( var track in Tracks ) track.ReadEntries( reader );
        }

        public void CalculateId( ref short id ) {
            Id = id++;
        }

        public void Write( BinaryWriter headerWriter, int timelinePos ) {
            var lastId = Tracks.Count > 0 ? Tracks.Last().Id : Id;

            var startPos = ( int )headerWriter.BaseStream.Position;
            var endPos = timelinePos + ( lastId - 2 ) * 2;
            var offset = endPos - startPos - 8 - 2 * (Tracks.Count - 1);

            TmbFile.WriteString( headerWriter, "TMAC" );
            headerWriter.Write( 0x1C );
            headerWriter.Write( Id );
            headerWriter.Write( Time);
            headerWriter.Write( Unk_2);
            headerWriter.Write( Unk_3);
            headerWriter.Write( offset );
            headerWriter.Write( Tracks.Count );
        }

        public void CalculateTracksId( ref short id ) {
            foreach( var track in Tracks ) track.CalculateId( ref id );
        }

        public void WriteTracks( BinaryWriter entryWriter, int entryPos, int timelinePos ) {
            foreach( var track in Tracks ) track.Write( entryWriter, entryPos, timelinePos );
        }

        public void CalculateEntriesId( ref short id ) {
            foreach( var track in Tracks ) track.CalculateEntriesId( ref id );
        }

        public void WriteEntries( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos ) {
            foreach( var track in Tracks ) track.WriteEntries( entryWriter, entryPos, extraWriter, extraPos, stringWriter, stringPos, timelinePos );
        }

        public void Draw(string id) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );

            var i = 0;
            foreach( var track in Tracks ) {
                if( ImGui.CollapsingHeader( $"Track {i}{id}{i}" ) ) {
                    ImGui.Indent();

                    if (ImGui.Button($"Delete{id}{i}")) {
                        Tracks.Remove( track );
                        ImGui.Unindent();
                        break;
                    }
                    track.Draw( $"{id}{i}" );

                    ImGui.Unindent();
                }
                i++;
            }

            if( ImGui.Button( $"+ Track{id}" ) ) {
                Tracks.Add( new TmbTrack() );
            }
        }
    }
}
