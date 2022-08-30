using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using VFXEditor.Utils;
using VFXEditor.TmbFormat.Entries;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat {
    public class Tmtr : TmbItemWithTime {
        public override string Magic => "TMTR";
        public override int Size => 0x18;
        public override int ExtraSize => !UseUnknownExtra ? 0 : 8 + ( 12 * UnknownData.Count );

        public readonly List<TmbEntry> Entries = new();
        private readonly List<int> TempIds;

        private bool UseUnknownExtra = false;
        private readonly List<TmtrUnknownData> UnknownData = new();

        public Tmtr() { }

        public Tmtr( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            TempIds = reader.ReadOffsetTimeline();
            reader.ReadAtOffset( ( binaryReader ) => {
                UseUnknownExtra = true;

                binaryReader.ReadInt32(); // 8
                var count = binaryReader.ReadInt32();
                for( var i = 0; i < count; i++ ) {
                    UnknownData.Add( new TmtrUnknownData( binaryReader ) );
                }
            } );
        }

        public void PickEntries( TmbReader reader ) {
            Entries.AddRange( reader.Pick<TmbEntry>( TempIds ) );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.WriteOffsetTimeline( Entries );
            if ( !UseUnknownExtra ) {
                writer.Write( 0 );
            }
            else {
                writer.WriteExtra( ( binaryWriter ) => {
                    binaryWriter.Write( 8 );
                    binaryWriter.Write( UnknownData.Count );

                    UnknownData.ForEach( x => x.Write( binaryWriter ) );
                } );
            }
        }

        public void Draw( string id, List<TmbEntry> entriesMaster ) {
            FileUtils.ShortInput( $"Time{id}", ref Time );

            // ==== Unknown Data ====

            ImGui.Checkbox( $"Unknown Extra Data{id}", ref UseUnknownExtra );
            if( UseUnknownExtra ) {
                ImGui.SameLine();
                if( ImGui.Button( $"+ New{id}-Unk" ) ) UnknownData.Add( new TmtrUnknownData() );

                var unkExtraIdx = 0;
                foreach( var unknownItem in UnknownData ) {
                    var itemId = $"{id}-Unk{unkExtraIdx}";
                    if( ImGui.CollapsingHeader( $"Unknown Extra Item {unkExtraIdx}{itemId}" ) ) {
                        ImGui.Indent();
                        if( UiUtils.RemoveButton( $"Delete{itemId}" ) ) {
                            UnknownData.Remove( unknownItem );
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
                    if( UiUtils.RemoveButton( $"Delete{id}{entryIdx}" ) ) {
                        Entries.Remove( entry );
                        entriesMaster.Remove( entry );
                        ImGui.Unindent();
                        break;
                    }
                    entry.Draw( $"{id}{entryIdx}" );
                    ImGui.Unindent();
                }
                entryIdx++;
            }

            if( ImGui.Button( $"+ New{id}" ) ) ImGui.OpenPopup( "New_Entry_Tmb" );

            if( ImGui.BeginPopup( "New_Entry_Tmb" ) ) {
                foreach( var entryOption in TmbUtils.ItemTypes.Values ) {
                    if( ImGui.Selectable( $"{entryOption.DisplayName}##New_Entry_Tmb" ) ) {
                        var type = entryOption.Type;
                        var constructor = type.GetConstructor( Array.Empty<Type>() );
                        var newEntry = (TmbEntry) constructor.Invoke( Array.Empty<object>() );

                        if( Entries.Count == 0 ) {
                            entriesMaster.Add( newEntry );
                        }
                        else {
                            var idx = entriesMaster.IndexOf( Entries.Last() );
                            entriesMaster.Insert( idx + 1, newEntry );
                        }
                        Entries.Add( newEntry );
                    }
                }
                ImGui.EndPopup();
            }
        }
    }
}
