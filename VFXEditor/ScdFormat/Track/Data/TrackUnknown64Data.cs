using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackUnknown64Data : ScdTrackData {
        public readonly ParsedByte Version = new( "Version" );
        public readonly ParsedShort Unk1 = new( "Unknown 1" );

        private readonly List<TrackUnknown64Item> Items = new();

        public override void Read( BinaryReader reader ) {
            Version.Read( reader );
            var count = reader.ReadByte();
            Unk1.Read( reader );

            for( var i = 0; i < count; i++ ) {
                var newPoint = new TrackUnknown64Item();
                newPoint.Read( reader );
                Items.Add( newPoint );
            }
        }

        public override void Write( BinaryWriter writer ) {
            Version.Write( writer );
            writer.Write( ( byte )Items.Count );
            Unk1.Write( writer );

            Items.ForEach( x => x.Write( writer ) );
        }

        public override void Draw( string parentId ) {
            Version.Draw( parentId, CommandManager.Scd );
            Unk1.Draw( parentId, CommandManager.Scd );

            for( var idx = 0; idx <  Items.Count; idx++ ) {
                Items[idx].Draw( $"{parentId}{idx}" );
            }
        }
    }

    public class TrackUnknown64Item {
        public readonly ParsedShort BankNumber = new( "Bank Number" );
        public readonly ParsedShort Index = new( "Index" );
        public readonly ParsedInt Unk1 = new( "Unknown 1" );
        public readonly ParsedFloat Unk2 = new( "Unknown 2" );

        public void Read( BinaryReader reader ) {
            BankNumber.Read( reader );
            Index.Read( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            BankNumber.Write( writer );
            Index.Write( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
        }

        public void Draw( string parentId ) {
            BankNumber.Draw( parentId, CommandManager.Scd );
            Index.Draw( parentId, CommandManager.Scd );
            Unk1.Draw( parentId, CommandManager.Scd );
            Unk2.Draw( parentId, CommandManager.Scd );
        }
    }
}
