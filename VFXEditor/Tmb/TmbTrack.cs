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
        public TmbTrack(BinaryReader reader) {
            var startPos = reader.BaseStream.Position;

            reader.ReadInt32(); // TMTR
            reader.ReadInt32(); // 0x18
            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            var offset = reader.ReadInt32(); // before [ITEM] + offset = spot on timeline
            Entry_Count = reader.ReadInt32();
            Unk_3 = reader.ReadInt32(); // 0

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset + 8 + 2 * (Entry_Count - 1), SeekOrigin.Begin );
            var endId = reader.ReadInt16();
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public void ReadEntries (BinaryReader reader) {
            for( var i = 0; i < Entry_Count; i++ ) {
                var name = Encoding.ASCII.GetString( reader.ReadBytes( 4 ) );
                reader.ReadInt32(); // size

                TmbItem newEntry = name switch {
                    "C063" => new C063( reader ),
                    "C006" => new C006( reader ),
                    "C010" => new C010( reader ),
                    "C131" => new C131( reader ),
                    "C002" => new C002( reader ),
                    "C011" => new C011( reader ),
                    "C012" => new C012( reader ),
                    "C067" => new C067( reader ),
                    "C053" => new C053( reader ),
                    _ => null
                };

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
            var offset = endPos - startPos - 8 - 2 * (Entries.Count - 1);

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

            // TODO: dialog here
        }
    }
}
