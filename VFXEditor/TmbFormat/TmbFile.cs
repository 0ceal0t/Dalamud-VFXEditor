using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.TmbFormat.Actor;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Tmfcs;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

// Rework based on https://github.com/AsgardXIV/XAT
namespace VfxEditor.TmbFormat {
    public class TmbFile : FileManagerFile {
        public readonly Tmdh HeaderTmdh;
        public readonly Tmpp HeaderTmpp;
        public readonly Tmal HeaderTmal;

        public readonly List<Tmfc> Tmfcs = new();
        public readonly List<Tmac> Actors = new();
        public readonly List<Tmtr> Tracks = new();
        public readonly List<TmbEntry> AllEntries = new();

        public readonly TmbActorDropdown ActorsDropdown;
        public readonly TmfcDropdown TmfcDropdown;

        private TmbEntry DraggingEntry = null;

        public TmbFile( BinaryReader binaryReader, bool checkOriginal = true ) : this( binaryReader, new( Plugin.TmbManager ), checkOriginal ) { }

        public TmbFile( BinaryReader binaryReader, CommandManager manager, bool checkOriginal = true ) : base( manager ) {
            ActorsDropdown = new( this );
            TmfcDropdown = new( this );

            var startPos = binaryReader.BaseStream.Position;
            var reader = new TmbReader( binaryReader );
            var original = checkOriginal ? FileUtils.GetOriginal( binaryReader ) : null;

            reader.ReadInt32(); // TMLB
            var size = reader.ReadInt32();
            var numEntries = reader.ReadInt32(); // entry count (not including TMLB)

            HeaderTmdh = new Tmdh( this, reader );
            HeaderTmpp = new Tmpp( this, reader );
            HeaderTmal = new Tmal( this, reader );

            for( var i = 0; i < numEntries - ( HeaderTmpp.IsAssigned ? 3 : 2 ); i++ ) {
                reader.ParseItem( this, Actors, Tracks, AllEntries, Tmfcs, ref Verified );
            }

            HeaderTmal.PickActors( reader );
            Actors.ForEach( x => x.PickTracks( reader ) );
            Tracks.ForEach( x => x.PickEntries( reader ) );

            RefreshIds();

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );

            binaryReader.BaseStream.Seek( startPos + size, SeekOrigin.Begin );
        }

        public override void Write( BinaryWriter writer ) {
            var startPos = writer.BaseStream.Position;
            FileUtils.WriteString( writer, "TMLB" );
            writer.Write( 0 ); // placeholder for size

            RefreshIds();

            var timelineCount = Actors.Count + Actors.Select( x => x.Tracks.Count ).Sum() + Tracks.Select( x => x.Entries.Count ).Sum();

            List<TmbItem> items = new() { HeaderTmdh };
            if( HeaderTmpp.IsAssigned ) items.Add( HeaderTmpp );
            items.Add( HeaderTmal );
            items.AddRange( Actors );
            items.AddRange( Tracks );
            items.AddRange( AllEntries );

            var itemLength = items.Sum( x => x.Size );
            var extraLength = items.Sum( x => x.ExtraSize );
            var timelineLength = timelineCount * sizeof( short );
            var tmbWriter = new TmbWriter( itemLength, extraLength, timelineLength );

            writer.Write( items.Count );
            foreach( var item in items ) {
                tmbWriter.StartPosition = tmbWriter.Position;
                item.Write( tmbWriter );
            }

            tmbWriter.WriteTo( writer );
            tmbWriter.Dispose();

            // Fill in size placeholder
            var endPos = writer.BaseStream.Position;
            writer.BaseStream.Seek( startPos + 4, SeekOrigin.Begin );
            writer.Write( ( int )( endPos - startPos ) );
            writer.BaseStream.Seek( endPos, SeekOrigin.Begin );
        }

        public override void Draw() {
            var maxDanger = AllEntries.Count == 0 ? DangerLevel.None : AllEntries.Select( x => x.Danger ).Max();
            if( maxDanger == DangerLevel.DontAddRemove ) DontAddRemoveWarning();
            else if( maxDanger == DangerLevel.Detectable || Tmfcs.Count > 0 ) DetectableWarning();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();

            using( var tab = ImRaii.TabItem( "Actors" ) ) {
                if( tab ) ActorsDropdown.Draw();
            }

            using( var tab = ImRaii.TabItem( "TMFC" ) ) {
                if( tab ) TmfcDropdown.Draw();
            }
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            HeaderTmdh.Draw();
            HeaderTmpp.Draw();
        }

        public void RefreshIds() {
            short id = 2;
            foreach( var actor in Actors ) actor.Id = id++;
            foreach( var track in Tracks ) track.Id = id++;
            foreach( var entry in AllEntries ) entry.Id = id++;
        }

        public void StartDragging( TmbEntry entry ) {
            ImGui.SetDragDropPayload( "TMB_ENTRY", IntPtr.Zero, 0 );
            DraggingEntry = entry;
        }

        public unsafe void StopDragging( Tmtr destination ) {
            if( DraggingEntry == null ) return;
            var payload = ImGui.AcceptDragDropPayload( "TMB_ENTRY" );
            if( payload.NativePtr == null ) return;

            TmbRefreshIdsCommand command = new( this, false, true );
            foreach( var track in Tracks ) {
                track.DeleteEntry( command, DraggingEntry ); // will add to command
            }
            destination.AddEntry( command, DraggingEntry );
            Command.Add( command );

            DraggingEntry = null;
        }

        // ===============

        public static TmbFile FromPapEmbedded( string path, CommandManager manager ) {
            if( !File.Exists( path ) ) return null;
            using BinaryReader br = new( File.Open( path, FileMode.Open ) );
            return new TmbFile( br, manager, true );
        }

        public static void DetectableWarning() {
            ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.RED_COLOR );
            ImGui.TextWrapped( "Changes to this file are potentially detectable" );
            ImGui.PopStyleColor();
            ImGui.SameLine();
            if( ImGui.SmallButton( "Guide" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Notes-on-TMFC" );
        }

        public static void GenericWarning() {
            ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.RED_COLOR );
            ImGui.TextWrapped( "Please don't do anything stupid with this" );
            ImGui.PopStyleColor();
        }

        public static void DontAddRemoveWarning() {
            ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.RED_COLOR );
            ImGui.TextWrapped( "Don't add or remove entries in this file" );
            ImGui.PopStyleColor();
        }
    }
}
