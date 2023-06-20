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

        public Tmtr( bool papEmbedded ) : base( papEmbedded ) { }

        public Tmtr( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            TempIds = reader.ReadOffsetTimeline();
            reader.ReadAtOffset( ( binaryReader ) => {
                UnknownExtraAssigned.Value = true;

                binaryReader.ReadInt32(); // 8
                var count = binaryReader.ReadInt32();
                for( var i = 0; i < count; i++ ) UnknownData.Add( new TmtrUnknownData( binaryReader, papEmbedded ) );
            } );
        }

        public void PickEntries( TmbReader reader ) => Entries.AddRange( reader.Pick<TmbEntry>( TempIds ) );

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
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

        public void Draw( TmbFile file ) {
            DrawHeader();
            DrawUnknownData();
            DrawEntries( file );
        }

        private void DrawUnknownData() {
            UnknownExtraAssigned.Draw( Command );
            if( UseUnknownExtra ) {
                using var _ = ImRaii.PushId( "Unk" );

                ImGui.SameLine();
                if( ImGui.Button( "+ New" ) ) Command.Add( new GenericAddCommand<TmtrUnknownData>( UnknownData, new TmtrUnknownData( PapEmbedded ) ) );

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

        private void DrawEntries( TmbFile file ) {
            for( var idx = 0; idx < Entries.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                if( Entries[idx].Draw( file, this ) ) break;
            }

            if( ImGui.Button( "+ New" ) ) ImGui.OpenPopup( "NewEntryPopup" );

            if( ImGui.BeginPopup( "NewEntryPopup" ) ) { // NEW
                using var _ = ImRaii.PushId( "NewEntry" );
                foreach( var entryOption in TmbUtils.ItemTypes.Values ) {
                    if( ImGui.Selectable( entryOption.DisplayName ) ) {
                        var constructor = entryOption.Type.GetConstructor( new Type[] { typeof( bool ) } );
                        var newEntry = ( TmbEntry )constructor.Invoke( new object[] { PapEmbedded } );
                        var idx = Entries.Count == 0 ? 0 : file.Entries.IndexOf( Entries.Last() ) + 1;

                        TmbRefreshIdsCommand command = new( file, false, true );
                        command.Add( new GenericAddCommand<TmbEntry>( Entries, newEntry ) );
                        command.Add( new GenericAddCommand<TmbEntry>( file.Entries, newEntry, idx ) );
                        Command.Add( command );
                    }
                }
                ImGui.EndPopup();
            }
        }

        public void DuplicateEntry( TmbFile file, TmbEntry entry ) {
            var data = entry.ToBytes();

            using var ms = new MemoryStream( data );
            using var reader = new BinaryReader( ms );
            var tmbReader = new TmbReader( reader );

            var verified = true;
            var actors = new List<Tmac>();
            var tracks = new List<Tmtr>();
            var entries = new List<TmbEntry>();
            var tmfcs = new List<Tmfc>();

            tmbReader.ParseItem( actors, tracks, entries, tmfcs, file.PapEmbedded, ref verified );

            var newEntry = entries[0];
            var idx = Entries.Count == 0 ? 0 : file.Entries.IndexOf( Entries.Last() ) + 1;

            TmbRefreshIdsCommand command = new( file, false, true );
            command.Add( new GenericAddCommand<TmbEntry>( Entries, newEntry ) );
            command.Add( new GenericAddCommand<TmbEntry>( file.Entries, newEntry, idx ) );
            Command.Add( command );
        }

        public void DeleteEntry( TmbFile file, TmbEntry entry ) {
            TmbRefreshIdsCommand command = new( file, false, true );
            command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
            command.Add( new GenericRemoveCommand<TmbEntry>( file.Entries, entry ) );
            Command.Add( command );
        }

        public void DeleteChildren( TmbRefreshIdsCommand command, TmbFile file ) {
            foreach( var entry in Entries ) {
                command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
                command.Add( new GenericRemoveCommand<TmbEntry>( file.Entries, entry ) );
            }
        }
    }
}
