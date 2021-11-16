using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using VFXEditor.Helper;

namespace VFXEditor.Tmb.Tmb {
    public class TmbActor {
        public short Id { get; private set; }

        private readonly int TrackCount_Temp;
        private readonly short LastId_Temp;

        private readonly List<TmbTrack> Tracks = new();
        private short Time = 0;
        private int Unk_2 = 0;
        private int Unk_3 = 0;

        private TmbTrack SelectedTrack = null;

        public int EntrySize => Tracks.Select( x => x.EntrySize ).Sum();
        public int ExtraSize => Tracks.Select( x => x.ExtraSize ).Sum();
        public int EntryCount => Tracks.Select( x => x.EntryCount ).Sum();

        public TmbActor() { }
        public TmbActor( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position;

            reader.ReadInt32(); // TMAC
            reader.ReadInt32(); // 0x1C
            reader.ReadInt16(); // id
            Time = reader.ReadInt16();
            Unk_2 = reader.ReadInt32(); // some count ?
            Unk_3 = reader.ReadInt32(); // some count ?
            var offset = reader.ReadInt32(); // before [TMAC] + offset + 8 = spot on timeline
            TrackCount_Temp = reader.ReadInt32(); // number of TMTR

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset + 8 + 2 * ( TrackCount_Temp - 1 ), SeekOrigin.Begin );
            LastId_Temp = reader.ReadInt16();
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public void PickTracks( List<TmbTrack> tracks, int startId ) {
            Tracks.AddRange( tracks.GetRange( LastId_Temp - startId - TrackCount_Temp + 1, TrackCount_Temp ).Where( x => x != null ) );
        }

        public void CalculateId( ref short id ) {
            Id = id++;
        }

        public void CalculateTracksId( ref short id ) {
            foreach( var track in Tracks ) track.CalculateId( ref id );
        }

        public void CalculateEntriesId( ref short id ) {
            foreach( var track in Tracks ) track.CalculateEntriesId( ref id );
        }

        public void Write( BinaryWriter headerWriter, int timelinePos ) {
            var lastId = Tracks.Count > 0 ? Tracks.Last().Id : Id;

            var startPos = ( int )headerWriter.BaseStream.Position;
            var endPos = timelinePos + ( lastId - 2 ) * 2;
            var offset = endPos - startPos - 8 - 2 * ( Tracks.Count - 1 );

            FileHelper.WriteString( headerWriter, "TMAC" );
            headerWriter.Write( 0x1C );
            headerWriter.Write( Id );
            headerWriter.Write( Time );
            headerWriter.Write( Unk_2 );
            headerWriter.Write( Unk_3 );
            headerWriter.Write( offset );
            headerWriter.Write( Tracks.Count );
        }

        public void WriteEntries( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos, int timelinePos ) {
            foreach( var track in Tracks ) track.WriteEntries( entryWriter, entryPos, extraWriter, extraPos, stringPositions, stringPos, timelinePos );
        }

        public void WriteTracks( BinaryWriter entryWriter, int entryPos, int timelinePos ) {
            foreach( var track in Tracks ) track.Write( entryWriter, entryPos, timelinePos );
        }

        public void PopulateStringList( List<string> stringList) {
            foreach( var track in Tracks ) track.PopulateStringList( stringList );
        }

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            ImGui.BeginChild( $"{id}-ActorChild", new Vector2( -1, -1 ), true );
            ImGui.Columns( 2, $"{id}-ActorChild-Cols", true );

            ImGui.BeginChild( $"{id}-ActorChild-Left" );
            // ===========
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) {
                Tracks.Add( new TmbTrack() );
            }
            if( SelectedTrack != null ) {
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 3 );
                if( UiHelper.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    Tracks.Remove( SelectedTrack );
                    SelectedTrack = null;
                }
            }
            ImGui.PopFont();

            var selectedIndex = SelectedTrack == null ? -1 : Tracks.IndexOf( SelectedTrack );
            for( var i = 0; i < Tracks.Count; i++ ) {
                if( ImGui.Selectable( $"Track {i}{id}{i}", Tracks[i] == SelectedTrack ) ) {
                    SelectedTrack = Tracks[i];
                    selectedIndex = i;
                }
            }
            // ===========
            ImGui.EndChild();

            ImGui.SetColumnWidth( 0, 150 );

            ImGui.NextColumn();
            ImGui.BeginChild( $"{id}-ActorChild-Right" );
            // ===========
            if( SelectedTrack != null ) {
                SelectedTrack.Draw( $"{id}{selectedIndex}" );
            }
            else {
                ImGui.Text( "Select a timeline track..." );
            }
            // ===========
            ImGui.EndChild();

            ImGui.Columns( 1 );
            ImGui.EndChild();
        }
    }
}
