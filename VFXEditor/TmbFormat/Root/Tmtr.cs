using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Entries;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.FileManager;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat {
    public class Tmtr : TmbItemWithTime {
        public override string Magic => "TMTR";
        public override int Size => 0x18;
        public override int ExtraSize => !UseUnknownExtra ? 0 : 8 + ( 12 * UnknownData.Count );

        public readonly List<TmbEntry> Entries = new();
        private readonly List<int> TempIds;

        private bool UseUnknownExtra => UnknownExtraAssigned.Value == true;
        private readonly ParsedBool UnknownExtraAssigned = new( "Use Unknown Extra Data", defaultValue: false );
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

        public void Draw( string id, TmbFile file ) {
            DrawHeader( id );

            // ==== Unknown Data ====

            UnknownExtraAssigned.Draw( id, Command );
            if( UseUnknownExtra ) {
                ImGui.SameLine();
                if( ImGui.Button( $"+ New{id}-Unk" ) ) Command.Add( new GenericAddCommand<TmtrUnknownData>( UnknownData, new TmtrUnknownData( PapEmbedded ) ) );

                var unkExtraIdx = 0;
                foreach( var unknownItem in UnknownData ) {
                    var itemId = $"{id}-Unk{unkExtraIdx}";
                    if( ImGui.CollapsingHeader( $"Unknown Extra Item {unkExtraIdx}{itemId}" ) ) {
                        ImGui.Indent();
                        if( UiUtils.RemoveButton( $"Delete{itemId}", true ) ) {
                            Command.Add( new GenericRemoveCommand<TmtrUnknownData>( UnknownData, unknownItem ) );
                            ImGui.Unindent();
                            break;
                        }
                        unknownItem.Draw( itemId );
                        ImGui.Unindent();
                    }
                    unkExtraIdx++;
                }
            }

            // ======= Entries =======

            var entryIdx = 0;
            foreach( var entry in Entries ) {
                if( ImGui.CollapsingHeader( $"{entry.DisplayName}{id}{entryIdx}" ) ) {
                    ImGui.Indent();

                    if( UiUtils.RemoveButton( $"Delete{id}{entryIdx}", true ) ) { // REMOVE
                        TmbRefreshIdsCommand command = new( file, false, true );
                        command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
                        command.Add( new GenericRemoveCommand<TmbEntry>( file.Entries, entry ) );
                        Command.Add( command );

                        ImGui.Unindent(); break;
                    }

                    entry.Draw( $"{id}{entryIdx}" );
                    ImGui.Unindent();
                }
                entryIdx++;
            }

            if( ImGui.Button( $"+ New{id}" ) ) ImGui.OpenPopup( "New_Entry_Tmb" );

            if( ImGui.BeginPopup( "New_Entry_Tmb" ) ) { // NEW
                foreach( var entryOption in TmbUtils.ItemTypes.Values ) {
                    if( ImGui.Selectable( $"{entryOption.DisplayName}##New_Entry_Tmb" ) ) {
                        var type = entryOption.Type;
                        var constructor = type.GetConstructor( new Type[] { typeof( bool ) } );
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

        public void DeleteChildren( TmbRefreshIdsCommand command, TmbFile file ) {
            foreach( var entry in Entries ) {
                command.Add( new GenericRemoveCommand<TmbEntry>( Entries, entry ) );
                command.Add( new GenericRemoveCommand<TmbEntry>( file.Entries, entry ) );
            }
        }
    }
}
