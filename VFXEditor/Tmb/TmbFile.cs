using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXEditor.Tmb.Tmb;

namespace VFXEditor.Tmb {
    public class TmbFile : FileDropdown<TmbActor> {
        public static TmbFile FromLocalFile( string path ) {
            if( !File.Exists( path ) ) return null;
            using BinaryReader br = new( File.Open( path, FileMode.Open ) );
            return new TmbFile( br );
        }

        // ==========================

        private readonly List<TmbActor> Actors = new();
        private short TMDH_Unk1 = 0;
        private short TMDH_Unk2 = 0;
        private short TMDH_Unk3 = 3;

        public bool Verified = true;

        public TmbFile() {
        }
        public TmbFile( BinaryReader reader, bool checkOriginal = true ) {
            var startPos = reader.BaseStream.Position;

            byte[] original = null;
            if (checkOriginal) {
                original = FileHelper.ReadAllBytes( reader );
                reader.BaseStream.Seek( startPos, SeekOrigin.Begin );
            }

            reader.ReadInt32(); // TMLB
            var size = reader.ReadInt32();
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

            if (checkOriginal) { // Check if output matches the original
                var output = ToBytes();
                for( var i = 0; i < Math.Min( output.Length, original.Length ); i++ ) {
                    if( output[i] != original[i] ) {
                        PluginLog.Log( $"Warning: files do not match at {i} {output[i]} {original[i]}" );
                        break;
                    }
                }
            }

            reader.BaseStream.Seek( startPos + size, SeekOrigin.Begin );
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

            FileHelper.WriteString( headerWriter, "TMLB" );
            headerWriter.Write( totalSize );
            headerWriter.Write( entryCount + 2 ); // + 2 for TMDH and TMAL

            FileHelper.WriteString( headerWriter, "TMDH" );
            headerWriter.Write( 0x10 );
            headerWriter.Write( ( short )1 );
            headerWriter.Write( TMDH_Unk1 );
            headerWriter.Write( TMDH_Unk2 );
            headerWriter.Write( TMDH_Unk3 );

            FileHelper.WriteString( headerWriter, "TMAL" );
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
                FileHelper.WriteString( stringWriter, item, true );
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

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Unknown 1{id}", ref TMDH_Unk1 );
            FileHelper.ShortInput( $"Unknown 2{id}", ref TMDH_Unk2 );
            FileHelper.ShortInput( $"Unknown 3{id}", ref TMDH_Unk3 );

            DrawDropDown( id );

            if( Selected != null ) {
                Selected.Draw( $"{id}{Actors.IndexOf( Selected )}" );
            }
            else {
                ImGui.Text( "Select a timeline actor..." );
            }
        }

        protected override List<TmbActor> GetOptions() => Actors;

        protected override string GetName( TmbActor item, int idx ) => $"Actor {idx}";

        protected override void OnNew() => Actors.Add( new TmbActor() );

        protected override void OnDelete( TmbActor item ) => Actors.Remove( item );
    }
}
