using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VFXEditor.Helper;

namespace VFXEditor.TMB.TMB {
    public class TMBTrack {
        private struct EntryType {
            public string Name;
            public Func<TMBItem> NewItem;
            public Func<BinaryReader, TMBItem> ReadItem;

            public EntryType( string name, Func<TMBItem> newItem, Func<BinaryReader, TMBItem> readItem ) {
                Name = name;
                NewItem = newItem;
                ReadItem = readItem;
            }
        }

        private static readonly Dictionary<string, EntryType> TypeDict = new() {
            { "C063", new EntryType( C063.DisplayName, () => new C063(), ( BinaryReader br ) => new C063( br ) ) },
            { "C006", new EntryType( C006.DisplayName, () => new C006(), ( BinaryReader br ) => new C006( br ) ) },
            { "C010", new EntryType( C010.DisplayName, () => new C010(), ( BinaryReader br ) => new C010( br ) ) },
            { "C131", new EntryType( C131.DisplayName, () => new C131(), ( BinaryReader br ) => new C131( br ) ) },
            { "C002", new EntryType( C002.DisplayName, () => new C002(), ( BinaryReader br ) => new C002( br ) ) },
            { "C011", new EntryType( C011.DisplayName, () => new C011(), ( BinaryReader br ) => new C011( br ) ) },
            { "C012", new EntryType( C012.DisplayName, () => new C012(), ( BinaryReader br ) => new C012( br ) ) },
            { "C067", new EntryType( C067.DisplayName, () => new C067(), ( BinaryReader br ) => new C067( br ) ) },
            { "C053", new EntryType( C053.DisplayName, () => new C053(), ( BinaryReader br ) => new C053( br ) ) },
            { "C075", new EntryType( C075.DisplayName, () => new C075(), ( BinaryReader br ) => new C075( br ) ) },
            { "C093", new EntryType( C093.DisplayName, () => new C093(), ( BinaryReader br ) => new C093( br ) ) },
            { "C009", new EntryType( C009.DisplayName, () => new C009(), ( BinaryReader br ) => new C009( br ) ) },
            { "C042", new EntryType( C042.DisplayName, () => new C042(), ( BinaryReader br ) => new C042( br ) ) },
            { "C014", new EntryType( C014.DisplayName, () => new C014(), ( BinaryReader br ) => new C014( br ) ) },
            { "C015", new EntryType( C015.DisplayName, () => new C015(), ( BinaryReader br ) => new C015( br ) ) },
            { "C118", new EntryType( C118.DisplayName, () => new C118(), ( BinaryReader br ) => new C118( br ) ) },
            { "C175", new EntryType( C175.DisplayName, () => new C175(), ( BinaryReader br ) => new C175( br ) ) },
            { "C174", new EntryType( C174.DisplayName, () => new C174(), ( BinaryReader br ) => new C174( br ) ) },
            { "C043", new EntryType( C043.DisplayName, () => new C043(), ( BinaryReader br ) => new C043( br ) ) },
            { "C031", new EntryType( C031.DisplayName, () => new C031(), ( BinaryReader br ) => new C031( br ) ) },
            { "C094", new EntryType( C094.DisplayName, () => new C094(), ( BinaryReader br ) => new C094( br ) ) },
            { "C203", new EntryType( C203.DisplayName, () => new C203(), ( BinaryReader br ) => new C203( br ) ) },
            { "C204", new EntryType( C204.DisplayName, () => new C204(), ( BinaryReader br ) => new C204( br ) ) },
            { "C198", new EntryType( C198.DisplayName, () => new C198(), ( BinaryReader br ) => new C198( br ) ) },
            { "C107", new EntryType( C107.DisplayName, () => new C107(), ( BinaryReader br ) => new C107( br ) ) },
            { "C120", new EntryType( C120.DisplayName, () => new C120(), ( BinaryReader br ) => new C120( br ) ) },
            { "C125", new EntryType( C125.DisplayName, () => new C125(), ( BinaryReader br ) => new C125( br ) ) },
            { "C173", new EntryType( C173.DisplayName, () => new C173(), ( BinaryReader br ) => new C173( br ) ) },
            { "C211", new EntryType( C211.DisplayName, () => new C211(), ( BinaryReader br ) => new C211( br ) ) },
        };

        // =====================================

        private class UnknownDataItem {
            public int Unk1;
            public short Unk2;
            public short Unk3;
            public int Unk4;

            public UnknownDataItem() {
                Unk1 = 0;
                Unk2 = 0;
                Unk3 = 0;
                Unk4 = 0;
            }

            public UnknownDataItem( BinaryReader reader ) {
                Unk1 = reader.ReadInt32();
                Unk2 = reader.ReadInt16();
                Unk3 = reader.ReadInt16();
                Unk4 = reader.ReadInt32();
            }

            public void Write( BinaryWriter writer ) {
                writer.Write( Unk1 );
                writer.Write( Unk2 );
                writer.Write( Unk3 );
                writer.Write( Unk4 );
            }

            public void Draw( string id ) {
                ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
                FileHelper.ShortInput( $"Unknown 2{id}", ref Unk2 );
                FileHelper.ShortInput( $"Unknown 3{id}", ref Unk3 );
                ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
            }
        }

        // ================================

        public static void ParseEntries( BinaryReader reader, List<TMBItem> entries, List<TMBTrack> tracks, int entryCount, ref bool entriesOk ) {
            for( var i = 0; i < entryCount; i++ ) {
                var name = Encoding.ASCII.GetString( reader.ReadBytes( 4 ) );
                var size = reader.ReadInt32(); // size

                if( name == "TMTR" ) {
                    tracks.Add( new TMBTrack( entries, reader ) );
                    continue;
                }

                var newEntry = TypeDict.TryGetValue( name, out var entryType ) ? entryType.ReadItem( reader ) : null;

                if( newEntry == null ) {
                    PluginLog.Log( $"Unknown Entry {name}" );
                    reader.ReadBytes( size - 8 ); // skip it
                    entries.Add( null );
                    entriesOk = false;
                }
                else {
                    entries.Add( newEntry );
                }
            }
        }

        // ==================================

        public short Id { get; private set; }

        private readonly int EntryCount_Temp;
        private readonly short LastId_Temp;
        private readonly int Offset_Temp;

        private short Time = 0;

        private bool UseUnknownExtraData = false;
        private readonly List<UnknownDataItem> UnknownExtraData = new();

        private readonly List<TMBItem> EntriesMaster;
        public readonly List<TMBItem> Entries = new();

        public TMBTrack( List<TMBItem> entriesMaster ) {
            EntriesMaster = entriesMaster;
        }
        public TMBTrack( List<TMBItem> entriesMaster, BinaryReader reader ) : this( entriesMaster ) {
            var startPos = reader.BaseStream.Position; // [TMTR] + 8

            Id = reader.ReadInt16(); // id
            Time = reader.ReadInt16();
            var offset = reader.ReadInt32(); // before [ITEM] + offset = spot on timeline
            EntryCount_Temp = reader.ReadInt32();
            var unknownOffset = reader.ReadInt32();

            // ====== READ SOME UNKNOWN DATA (SO FAR ONLY IN REAPER) =========
            if( unknownOffset > 0 ) {
                UseUnknownExtraData = true;

                var savePos2 = reader.BaseStream.Position;
                reader.BaseStream.Seek( startPos + unknownOffset, SeekOrigin.Begin );

                reader.ReadInt32(); // 8
                var unknownExtraCount = reader.ReadInt32();
                for( var i = 0; i < unknownExtraCount; i++ ) {
                    UnknownExtraData.Add( new UnknownDataItem( reader ) );
                }

                reader.BaseStream.Seek( savePos2, SeekOrigin.Begin );
            }

            Offset_Temp = offset;

            // ====== READ IDS =========
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset + 2 * ( EntryCount_Temp - 1 ), SeekOrigin.Begin );
            LastId_Temp = reader.ReadInt16();
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public void PickEntries( List<TMBItem> entries, int startId ) {
            Entries.AddRange( entries.GetRange( LastId_Temp - startId - EntryCount_Temp + 1, EntryCount_Temp ).Where( x => x != null ) );
        }

        public void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<int, int> timelinePos ) {
            var lastId = Entries.Count > 0 ? Entries.Last().Id : Id;

            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = timelinePos.TryGetValue( lastId, out var pos ) ? pos : 0;
            var offset = endPos - startPos - 8 - ( 2 * ( Entries.Count == 0 ? 0 : ( Entries.Count - 1 ) ) );

            if( Entries.Count == 0 ) offset = Offset_Temp;

            FileHelper.WriteString( entryWriter, "TMTR" );
            entryWriter.Write( 0x18 );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( offset );
            entryWriter.Write( Entries.Count );

            // ======= WRITE UNKNOWN DATA ===========

            if( !UseUnknownExtraData ) {
                entryWriter.Write( 0 );
            }
            else {
                var extraEndPos = ( int )extraWriter.BaseStream.Position + extraPos;
                var extraOffset = extraEndPos - startPos - 8;
                entryWriter.Write( extraOffset );

                extraWriter.Write( 8 );
                extraWriter.Write( UnknownExtraData.Count );
                foreach( var item in UnknownExtraData ) {
                    item.Write( extraWriter );
                }
            }
        }

        public void CalculateId( ref short id ) {
            Id = id++;
        }

        public void PopulateStringList( List<string> stringList ) {
            foreach( var entry in Entries ) entry.PopulateStringList( stringList );
        }

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Time{id}", ref Time );

            // =======================

            ImGui.Checkbox( $"Unknown Extra Data{id}", ref UseUnknownExtraData );
            if( UseUnknownExtraData ) {
                ImGui.SameLine();
                if( ImGui.Button( $"+ New{id}-Unk" ) ) UnknownExtraData.Add( new UnknownDataItem() );

                var unkExtraIdx = 0;
                foreach( var item in UnknownExtraData ) {
                    var itemId = $"{id}-Unk{unkExtraIdx}";
                    if( ImGui.CollapsingHeader( $"Unknown Extra Item {unkExtraIdx}{itemId}" ) ) {
                        ImGui.Indent();
                        if( UIHelper.RemoveButton( $"Delete{itemId}" ) ) {
                            UnknownExtraData.Remove( item );
                            ImGui.Unindent();
                            break;
                        }
                        item.Draw( itemId );
                        ImGui.Unindent();
                    }
                    unkExtraIdx++;
                }
            }

            // =========================

            var entryIdx = 0;
            foreach( var entry in Entries ) {
                if( ImGui.CollapsingHeader( $"{entry.GetDisplayName()}{id}{entryIdx}" ) ) {
                    ImGui.Indent();
                    if( UIHelper.RemoveButton( $"Delete{id}{entryIdx}" ) ) {
                        Entries.Remove( entry );
                        EntriesMaster.Remove( entry );
                        ImGui.Unindent();
                        break;
                    }
                    entry.Draw( $"{id}{entryIdx}" );
                    ImGui.Unindent();
                }
                entryIdx++;
            }

            if( ImGui.Button( $"+ New{id}" ) ) {
                ImGui.OpenPopup( "New_Entry_Tmb" );
            }

            if( ImGui.BeginPopup( "New_Entry_Tmb" ) ) {
                foreach( var entryType in TypeDict.Values ) {
                    if( ImGui.Selectable( $"{entryType.Name}##New_Entry_Tmb" ) ) {
                        var newEntry = entryType.NewItem();
                        if( Entries.Count == 0 ) {
                            EntriesMaster.Add( newEntry );
                        }
                        else {
                            var idx = EntriesMaster.IndexOf( Entries.Last() );
                            EntriesMaster.Insert( idx + 1, newEntry );
                        }
                        Entries.Add( newEntry );
                    }
                }
                ImGui.EndPopup();
            }
        }

        public int GetExtraSize() => !UseUnknownExtraData ? 0 : 8 + ( 12 * UnknownExtraData.Count );
    }
}
