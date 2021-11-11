using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VFXEditor.Helper;

namespace VFXEditor.Tmb {
    public class TmbFile {
        private readonly List<TmbActor> Actors = new();
        private short TMDH_Unk1 = 0;
        private short TMDH_Unk2 = 0;
        private short TMDH_Unk3 = 3;
        private TmbActor SelectedActor = null;

        public bool EntriesOk = true;

        public TmbFile( BinaryReader reader ) {
            reader.ReadInt32(); // TMLB
            reader.ReadInt32(); // 0x0C
            reader.ReadInt32(); // entry count (not including TMBL)

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

            for( var i = 0; i < numActors; i++ ) {
                Actors.Add( new TmbActor( reader ) );
            }
            foreach( var actor in Actors ) actor.ReadTracks( reader );
            foreach( var actor in Actors ) actor.ReadEntries( reader, ref EntriesOk );
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
            var stringSize = Actors.Select( x => x.StringSize ).Sum();
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

            // entries, extra, string
            var entriesData = new byte[entriesSize];
            using MemoryStream entriesMs = new( entriesData );
            using BinaryWriter entriesWriter = new( entriesMs );
            entriesWriter.BaseStream.Seek( 0, SeekOrigin.Begin );

            var extraData = new byte[extraSize];
            using MemoryStream extraMs = new( extraData );
            using BinaryWriter extraWriter = new( extraMs );
            extraWriter.BaseStream.Seek( 0, SeekOrigin.Begin );

            var stringData = new byte[stringSize];
            using MemoryStream stringMs = new( stringData );
            using BinaryWriter stringWriter = new( stringMs );
            stringWriter.BaseStream.Seek( 0, SeekOrigin.Begin );

            short id = 2;
            foreach( var tmac in Actors ) tmac.CalculateId( ref id );
            foreach( var tmac in Actors ) tmac.CalculateTracksId( ref id );
            foreach( var tmac in Actors ) tmac.CalculateEntriesId( ref id );

            foreach( var tmac in Actors ) tmac.Write( headerWriter, timelinePos );
            foreach( var tmac in Actors ) tmac.WriteTracks( entriesWriter, entriesPos, timelinePos );
            foreach( var tmac in Actors ) tmac.WriteEntries( entriesWriter, entriesPos, extraWriter, extraPos, stringWriter, stringPos, timelinePos );

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
    }
}
