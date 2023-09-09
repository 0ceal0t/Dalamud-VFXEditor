using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.AtchFormat.Entry;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.Formats.AtchFormat {
    public class AtchFile : FileManagerFile {
        public readonly ushort NumStates;
        public readonly List<AtchEntry> Entries = new();
        private readonly SimpleSplitview<AtchEntry> EntryView;

        public readonly ParsedUInt Flags1 = new( "Flags 1" );
        public readonly ParsedUInt Flags2 = new( "Flags 2" );
        public readonly ParsedUInt Flags3 = new( "Flags 3" );
        public readonly ParsedUInt Flags4 = new( "Flags 4" );

        public AtchFile( BinaryReader reader ) : base( new CommandManager( Plugin.AtchManager ) ) {
            Verified = VerifiedStatus.UNSUPPORTED; // verifying these is fucked. The format is pretty simple though, so it's not a big deal

            var numEntries = reader.ReadUInt16();
            NumStates = reader.ReadUInt16();

            for( var i = 0; i < numEntries; i++ ) {
                Entries.Add( new( reader ) );
            }

            Flags1.Read( reader );
            Flags2.Read( reader );
            Flags3.Read( reader );
            Flags4.Read( reader );

            var dataEnd = reader.BaseStream.Position;

            Entries.ForEach( x => x.ReadBody( reader, NumStates ) );
            EntryView = new( "Entry", Entries, false, ( AtchEntry item, int idx ) => item.Name.Value, () => new(), () => CommandManager.Atch );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( ushort )Entries.Count );
            writer.Write( NumStates );

            Entries.ForEach( x => x.Write( writer ) );

            Flags1.Write( writer );
            Flags2.Write( writer );
            Flags3.Write( writer );
            Flags4.Write( writer );

            var stringStartPos = 2 + 2 + ( 4 * Entries.Count ) + 16 + ( 32 * Entries.Count * NumStates );
            using var stringMs = new MemoryStream();
            using var stringWriter = new BinaryWriter( stringMs );
            var stringPos = new Dictionary<string, int>();

            Entries.ForEach( x => x.WriteBody( writer, stringStartPos, stringWriter, stringPos ) );

            writer.Write( stringMs.ToArray() );
        }

        public override void Draw() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Entries" ) ) {
                if( tab ) EntryView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Flags" ) ) {
                if( tab ) {
                    using var child = ImRaii.Child( "Child" );
                    Flags1.Draw( CommandManager.Atch );
                    Flags2.Draw( CommandManager.Atch );
                    Flags3.Draw( CommandManager.Atch );
                    Flags4.Draw( CommandManager.Atch );
                }
            }
        }
    }
}
