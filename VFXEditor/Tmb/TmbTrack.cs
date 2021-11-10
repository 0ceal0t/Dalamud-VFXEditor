using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Tmb {
    public class TmbTrack {
        private struct EntryType {
            public string Name;
            public Func<TmbItem> NewItem;
            public Func<BinaryReader, TmbItem> ReadItem;

            public EntryType( string name, Func<TmbItem> newItem, Func<BinaryReader, TmbItem> readItem ) {
                Name = name;
                NewItem = newItem;
                ReadItem = readItem;
            }
        }

        private static Dictionary<string, EntryType> TypeDict = new() {
            { "C063", new EntryType( C063.Name, () => new C063(), ( BinaryReader br ) => new C063( br ) ) },
            { "C006", new EntryType( C006.Name, () => new C006(), ( BinaryReader br ) => new C006( br ) ) },
            { "C010", new EntryType( C010.Name, () => new C010(), ( BinaryReader br ) => new C010( br ) ) },
            { "C131", new EntryType( C131.Name, () => new C131(), ( BinaryReader br ) => new C131( br ) ) },
            { "C002", new EntryType( C002.Name, () => new C002(), ( BinaryReader br ) => new C002( br ) ) },
            { "C011", new EntryType( C011.Name, () => new C011(), ( BinaryReader br ) => new C011( br ) ) },
            { "C012", new EntryType( C012.Name, () => new C012(), ( BinaryReader br ) => new C012( br ) ) },
            { "C067", new EntryType( C067.Name, () => new C067(), ( BinaryReader br ) => new C067( br ) ) },
            { "C053", new EntryType( C053.Name, () => new C053(), ( BinaryReader br ) => new C053( br ) ) },
        };

        public short Id { get; private set; } // temp
        private int Entry_Count;

        private short Time = 0;
        private List<TmbItem> Entries = new();
        private int Unk_3 = 0;

        public int EntrySize => 0x18 + Entries.Select( x => x.GetSize() ).Sum();
        public int ExtraSize => Entries.Select( x => x.GetExtraSize() ).Sum();
        public int StringSize => Entries.Select( x => x.GetStringSize() ).Sum();
        public int EntryCount => 1 + Entries.Count;

        public TmbTrack() { }
        public TmbTrack( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position;

            reader.ReadInt32(); // TMTR
            reader.ReadInt32(); // 0x18
            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            var offset = reader.ReadInt32(); // before [ITEM] + offset = spot on timeline
            Entry_Count = reader.ReadInt32();
            Unk_3 = reader.ReadInt32(); // 0

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset + 8 + 2 * ( Entry_Count - 1 ), SeekOrigin.Begin );
            var endId = reader.ReadInt16();
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public void ReadEntries( BinaryReader reader ) {
            for( var i = 0; i < Entry_Count; i++ ) {
                var name = Encoding.ASCII.GetString( reader.ReadBytes( 4 ) );
                reader.ReadInt32(); // size

                TmbItem newEntry = TypeDict.TryGetValue( name, out var entryType ) ? entryType.ReadItem( reader ) : null;

                if( newEntry == null ) {
                    PluginLog.Log( $"Unknown Entry {name}" );
                }
                else {
                    Entries.Add( newEntry );
                }
            }
        }

        public void CalculateId( ref short id ) {
            Id = id++;
        }

        public void Write( BinaryWriter entryWriter, int entryPos, int timelinePos ) {
            var lastId = Entries.Count > 0 ? Entries.Last().Id : Id;

            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = timelinePos + ( lastId - 2 ) * 2;
            var offset = endPos - startPos - 8 - 2 * ( Entries.Count - 1 );

            TmbFile.WriteString( entryWriter, "TMTR" );
            entryWriter.Write( 0x18 );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( offset );
            entryWriter.Write( Entries.Count );
            entryWriter.Write( Unk_3 );
        }

        public void CalculateEntriesId( ref short id ) {
            foreach( var entry in Entries ) entry.CalculateId( ref id );
        }

        public void WriteEntries( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos ) {
            foreach( var entry in Entries ) entry.Write( entryWriter, entryPos, extraWriter, extraPos, stringWriter, stringPos, timelinePos );
        }

        private string AddSelected = "[NONE]";

        public void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );

            var i = 0;
            foreach( var entry in Entries ) {
                if( ImGui.CollapsingHeader( $"{entry.GetName()}{id}{i}" ) ) {
                    ImGui.Indent();

                    if( ImGui.Button( $"Delete{id}{i}" ) ) {
                        Entries.Remove( entry );
                        ImGui.Unindent();
                        break;
                    }
                    entry.Draw( $"{id}{i}" );

                    ImGui.Unindent();
                }
                i++;
            }

            // ==== SELECTION DIALOG =========
            if( ImGui.BeginCombo( $"{id}Add", AddSelected ) ) {
                if( ImGui.Selectable( $"Movement Cancel (C131){id}", AddSelected.Equals( "C131" ) ) ) {
                    AddSelected = "C131";
                    Entries.Add( new C131() );
                }
                if( ImGui.Selectable( $"Movement Cancel (C131){id}", AddSelected.Equals( "C131" ) ) ) {
                    AddSelected = "C131";
                    Entries.Add( new C131() );
                }
                ImGui.EndCombo();
            }
        }
    }
}
