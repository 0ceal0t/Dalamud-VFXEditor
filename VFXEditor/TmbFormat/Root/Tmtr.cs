using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Actor;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Tmfcs;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public class Tmtr : TmbItemWithTime {
        public override string Magic => "TMTR";
        public override int Size => 0x18;
        public override int ExtraSize => !UseUnknownExtra ? 0 : 8 + ( 12 * UnknownData.Count );

        public readonly List<TmbEntry> Entries = new();
        private readonly List<int> TempIds;
        public DangerLevel MaxDanger => Entries.Count == 0 ? DangerLevel.None : Entries.Select( x => x.Danger ).Max();

        private bool UseUnknownExtra => UnknownExtraAssigned.Value == true;
        private readonly ParsedByteBool UnknownExtraAssigned = new( "Use Unknown Extra Data", defaultValue: false );
        private readonly List<TmtrUnknownData> UnknownData = new();

        private int AllEntriesIdx => Entries.Count == 0 ? 0 : File.AllEntries.IndexOf( Entries.Last() ) + 1;

        public Tmtr( TmbFile file ) : base( file ) { }

        public Tmtr( TmbFile file, TmbReader reader ) : base( file, reader ) {
            TempIds = reader.ReadOffsetTimeline();
            reader.ReadAtOffset( ( binaryReader ) => {
                UnknownExtraAssigned.Value = true;

                binaryReader.ReadInt32(); // 8
                var count = binaryReader.ReadInt32();
                for( var i = 0; i < count; i++ ) UnknownData.Add( new TmtrUnknownData( binaryReader, file.PapEmbedded ) );
            } );
        }

        public void PickEntries( TmbReader reader ) => Entries.AddRange( reader.Pick<TmbEntry>( TempIds ) );

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            writer.WriteOffsetTimeline( Entries );
            if( !UseUnknownExtra ) writer.Write( 0 );
            else {
                writer.WriteExtra( ( binaryWriter ) => {
                    binaryWriter.Write( 8 );
                    binaryWriter.Write( UnknownData.Count );
                    UnknownData.ForEach( x => x.Write( binaryWriter ) );
                } );
            }
        }

        public void Draw() {
            DrawHeader();
            DrawUnknownData();
            DrawEntries();
        }

        private void DrawUnknownData() {
            UnknownExtraAssigned.Draw( Command );
            if( UseUnknownExtra ) {
                using var _ = ImRaii.PushId( "Unk" );

                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.SameLine();
                    if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                        Command.Add( new GenericAddCommand<TmtrUnknownData>( UnknownData, new TmtrUnknownData( PapEmbedded ) ) );
                    }
                }

                for( var idx = 0; idx < UnknownData.Count; idx++ ) {
                    var unknownItem = UnknownData[idx];

                    if( ImGui.CollapsingHeader( $"Unknown Extra Item {idx}" ) ) {
                        using var __ = ImRaii.PushId( idx );
                        using var indent = ImRaii.PushIndent();

                        if( UiUtils.RemoveButton( "Delete", true ) ) {
                            Command.Add( new GenericRemoveCommand<TmtrUnknownData>( UnknownData, unknownItem ) );
                            break;
                        }
                        unknownItem.Draw();
                    }
                }
            }
        }

        private void DrawEntries() {
            for( var idx = 0; idx < Entries.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                if( Entries[idx].Draw( this ) ) break;
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            if( UiUtils.IconButton( FontAwesomeIcon.Plus, "New" ) ) ImGui.OpenPopup( "NewEntryPopup" );

            if( ImGui.BeginPopup( "NewEntryPopup" ) ) { // NEW
                using var _ = ImRaii.PushId( "NewEntry" );

                if( UiUtils.IconSelectable( FontAwesomeIcon.FileImport, "Import" ) ) ImportDialog();

                ImGui.Separator();

                foreach( var entryOption in TmbUtils.ItemTypes.Values ) {
                    if( ImGui.Selectable( entryOption.DisplayName ) ) {
                        var constructor = entryOption.Type.GetConstructor( new Type[] { typeof( TmbFile ) } );
                        var newEntry = ( TmbEntry )constructor.Invoke( new object[] { File } );

                        TmbRefreshIdsCommand command = new( File, false, true );
                        AddEntry( command, newEntry );
                        Command.Add( command );
                    }
                }
                ImGui.EndPopup();
            }
        }

        public void DuplicateEntry( TmbEntry entry ) => ImportEntry( entry.ToBytes() );

        private void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", "TMB Entry{.tmbentry},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    ImportEntry( System.IO.File.ReadAllBytes( res ) );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not import data" );
                }
            } );
        }

        private void ImportEntry( byte[] data ) {
            using var ms = new MemoryStream( data );
            using var reader = new BinaryReader( ms );
            var tmbReader = new TmbReader( reader );

            var verified = true;
            var actors = new List<Tmac>();
            var tracks = new List<Tmtr>();
            var entries = new List<TmbEntry>();
            var tmfcs = new List<Tmfc>();

            tmbReader.ParseItem( File, actors, tracks, entries, tmfcs, ref verified );
            var newEntry = entries[0];

            TmbRefreshIdsCommand command = new( File, false, true );
            AddEntry( command, newEntry );
            Command.Add( command );
        }

        public void AddEntry( CompoundCommand command, TmbEntry entry ) {
            command.Add( new GenericAddCommand<TmbEntry>( Entries, entry ) );
            command.Add( new GenericAddCommand<TmbEntry>( File.AllEntries, entry, AllEntriesIdx ) );
        }

        public void DeleteEntry( TmbEntry entry ) {
            if( !Entries.Contains( entry ) ) return;
            TmbRefreshIdsCommand command = new( File, false, true );
            DeleteEntry( command, entry );
            Command.Add( command );
        }

        public void DeleteEntry( CompoundCommand command, TmbEntry entry ) {
            if( !Entries.Contains( entry ) ) return;
            command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
            command.Add( new GenericRemoveCommand<TmbEntry>( File.AllEntries, entry ) );
        }

        public void DeleteAllEntries( TmbRefreshIdsCommand command ) {
            foreach( var entry in Entries ) {
                command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
                command.Add( new GenericRemoveCommand<TmbEntry>( File.AllEntries, entry ) );
            }
        }
    }
}
