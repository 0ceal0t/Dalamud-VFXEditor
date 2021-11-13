using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VFXEditor.Helper;
using VFXEditor.Tmb.Item;

namespace VFXEditor.Tmb {
    public class TmbFile {
        private readonly List<TmbActor> Actors = new();
        private short TMDH_Unk1 = 0;
        private short TMDH_Unk2 = 0;
        private short TMDH_Unk3 = 3;
        private TmbActor SelectedActor = null;

        public bool Verified = true;

        public TmbFile( BinaryReader reader ) {
            var original = ReadAllBytes( reader );
            reader.BaseStream.Seek( 0, SeekOrigin.Begin );

            reader.ReadInt32(); // TMLB
            reader.ReadInt32(); // 0x0C
            var numEntries = reader.ReadInt32(); // entry count (not including TMLB)

            reader.ReadInt32(); // TMDH
            reader.ReadInt32(); // 0x10
            reader.ReadInt16(); // id
            TMDH_Unk1 = reader.ReadInt16();
            TMDH_Unk2 = reader.ReadInt16(); // ?
            TMDH_Unk3 = reader.ReadInt16(); // 3

            reader.ReadInt32(); // TMAL
            reader.ReadInt32(); // 0x10
            reader.ReadInt32(); // offset from [TMAL] + 8 to timeline
            var numActors = reader.ReadInt32(); // Number of TMAC

            // ============ PARSE ================
            for( var i = 0; i < numActors; i++ ) { // parse actors
                Actors.Add( new TmbActor( reader ) );
            }
            var tracks = new List<TmbTrack>();
            var entries = new List<TmbItem>();
            TmbTrack.ParseEntries( reader, entries, tracks, numEntries - 2 - Actors.Count, ref Verified );
            // ===================================

            foreach( var track in tracks ) track.PickEntries( entries, 2 + Actors.Count + tracks.Count ); // if 1 actor, 1 track => 1 = header, 2 = actor, 3 = track, 4 = entry...
            foreach( var actor in Actors ) actor.PickTracks( tracks, 2 + Actors.Count );

            // Check if output matches the original
            var output = ToBytes();
            for( var i = 0; i < Math.Min( output.Length, original.Length ); i++ ) {
                if( output[i] != original[i] ) {
                    PluginLog.Log( $"Warning: files do not match at {i} {output[i]} {original[i]}" );
                    break;
                }
            }
        }

        public void Draw( string id ) {
            ShortInput( $"Unknown 1{id}", ref TMDH_Unk1 );
            ShortInput( $"Unknown 2{id}", ref TMDH_Unk2 );
            ShortInput( $"Unknown 3{id}", ref TMDH_Unk3 );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            var selectedIndex = SelectedActor == null ? -1 : Actors.IndexOf( SelectedActor );
            if( ImGui.BeginCombo( $"{id}-ActorSelect", SelectedActor == null ? "[NONE]" : $"Actor {selectedIndex}" ) ) {
                for( var i = 0; i < Actors.Count; i++ ) {
                    var actor = Actors[i];
                    if( ImGui.Selectable( $"Actor {i}{id}{i}", actor == SelectedActor ) ) {
                        SelectedActor = actor;
                        selectedIndex = i;
                    }
                }
                ImGui.EndCombo();
            }

            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SameLine();
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) {
                Actors.Add( new TmbActor() );
            }
            if( SelectedActor != null ) {
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 3 );
                if( UiHelper.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    Actors.Remove( SelectedActor );
                    SelectedActor = null;
                }
            }
            ImGui.PopFont();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            if( SelectedActor != null ) {
                SelectedActor.Draw( $"{id}{selectedIndex}" );
            }
            else {
                ImGui.Text( "Select a timeline actor..." );
            }
        }

        public byte[] ToBytes() {
            var headerSize = 0x0C + 0x10 + 0x10 + Actors.Count * 0x1C;
            var entriesSize = Actors.Select( x => x.EntrySize ).Sum();
            var extraSize = Actors.Select( x => x.ExtraSize ).Sum();

            var stringList = new List<string>();
            Actors.ForEach( x => x.PopulateStringList( stringList ) );
            var stringSize = stringList.Select( x => x.Length + 1 ).Sum();

            var entryCount = Actors.Count + Actors.Select( x => x.EntryCount ).Sum(); // include TMTR + entries
            var timelineSize = 2 * entryCount;

            // calculate starting positions
            var entriesPos = headerSize;
            var extraPos = entriesPos + entriesSize;
            var timelinePos = extraPos + extraSize;
            var stringPos = timelinePos + timelineSize;
            var totalSize = headerSize + entriesSize + extraSize + timelineSize + stringSize;

            // write header
            var headerData = new byte[headerSize];
            using MemoryStream headerMs = new( headerData );
            using BinaryWriter headerWriter = new( headerMs );
            headerWriter.BaseStream.Seek( 0, SeekOrigin.Begin );

            WriteString( headerWriter, "TMLB" );
            headerWriter.Write( totalSize );
            headerWriter.Write( entryCount + 2 ); // + 2 for TMDH and TMAL

            WriteString( headerWriter, "TMDH" );
            headerWriter.Write( 0x10 );
            headerWriter.Write( ( short )1 );
            headerWriter.Write( TMDH_Unk1 );
            headerWriter.Write( TMDH_Unk2 );
            headerWriter.Write( TMDH_Unk3 );

            WriteString( headerWriter, "TMAL" );
            headerWriter.Write( 0x10 );
            headerWriter.Write( timelinePos - ( int )headerWriter.BaseStream.Position );
            headerWriter.Write( Actors.Count );

            // write timeline
            var timelineData = new byte[timelineSize];
            using MemoryStream timelineMs = new( timelineData );
            using BinaryWriter timelineWriter = new( timelineMs );
            timelineWriter.BaseStream.Seek( 0, SeekOrigin.Begin );

            for( var i = 0; i < entryCount; i++ ) {
                timelineWriter.Write( ( short )( 2 + i ) );
            }

            var stringData = new byte[stringSize];
            using MemoryStream stringMs = new( stringData );
            using BinaryWriter stringWriter = new( stringMs );
            stringWriter.BaseStream.Seek( 0, SeekOrigin.Begin );
            Dictionary<string, int> stringPositions = new();
            var currentStringPos = 0;
            foreach(var item in stringList) {
                stringPositions[item] = currentStringPos;
                WriteString( stringWriter, item, true );
                currentStringPos += item.Length + 1;
            }

            var entriesData = new byte[entriesSize];
            using MemoryStream entriesMs = new( entriesData );
            using BinaryWriter entriesWriter = new( entriesMs );
            entriesWriter.BaseStream.Seek( 0, SeekOrigin.Begin );

            var extraData = new byte[extraSize];
            using MemoryStream extraMs = new( extraData );
            using BinaryWriter extraWriter = new( extraMs );
            extraWriter.BaseStream.Seek( 0, SeekOrigin.Begin );

            short id = 2;
            foreach( var tmac in Actors ) tmac.CalculateId( ref id );
            foreach( var tmac in Actors ) tmac.CalculateTracksId( ref id );
            foreach( var tmac in Actors ) tmac.CalculateEntriesId( ref id );

            foreach( var tmac in Actors ) tmac.Write( headerWriter, timelinePos );
            foreach( var tmac in Actors ) tmac.WriteTracks( entriesWriter, entriesPos, timelinePos );
            foreach( var tmac in Actors ) tmac.WriteEntries( entriesWriter, entriesPos, extraWriter, extraPos, stringPositions, stringPos, timelinePos );

            var output = new byte[totalSize];
            Buffer.BlockCopy( headerData, 0, output, 0, headerData.Length );
            Buffer.BlockCopy( entriesData, 0, output, entriesPos, entriesData.Length );
            Buffer.BlockCopy( extraData, 0, output, extraPos, extraData.Length );
            Buffer.BlockCopy( timelineData, 0, output, timelinePos, timelineData.Length );
            Buffer.BlockCopy( stringData, 0, output, stringPos, stringData.Length );

            return output;
        }

        // ====================================

        public static string ReadString( BinaryReader input ) {
            var strBytes = new List<byte>();
            int b;
            while( ( b = input.ReadByte() ) != 0x00 )
                strBytes.Add( ( byte )b );
            return Encoding.ASCII.GetString( strBytes.ToArray() );
        }

        public static void WriteString( BinaryWriter writer, string str, bool writeNull = false ) {
            writer.Write( Encoding.ASCII.GetBytes( str.Trim().Trim( '\0' ) ) );
            if( writeNull ) writer.Write( ( byte )0 );
        }

        public static bool ShortInput( string id, ref short value ) {
            var val = ( int )value;
            if( ImGui.InputInt( id, ref val ) ) {
                value = ( short )val;
                return true;
            }
            return false;
        }

        public static byte[] ReadAllBytes( BinaryReader reader ) {
            const int bufferSize = 4096;
            using var ms = new MemoryStream();
            var buffer = new byte[bufferSize];
            int count;
            while( ( count = reader.Read( buffer, 0, buffer.Length ) ) != 0 )
                ms.Write( buffer, 0, count );
            return ms.ToArray();

        }
    }
}
