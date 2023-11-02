using System;
using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Materials {
    public class ShpkMaterialParmeter : IUiItem {
        public readonly ParsedCrc Id = new( "Id" );
        public readonly ParsedShort Offset = new( "Offset" );
        public readonly ParsedShort Size = new( "Size" );

        public int StartSlot => Offset.Value / 4;
        public int EndSlot => ( Offset.Value + Size.Value ) / 4;

        public ShpkMaterialParmeter() { }

        public ShpkMaterialParmeter( BinaryReader reader ) {
            Id.Read( reader );
            Offset.Read( reader );
            Size.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            Offset.Write( writer );
            Size.Write( writer );
        }

        public void Draw() {
            Id.Draw( CrcMaps.MaterialParams );
            Offset.Draw();
            Size.Draw();
        }

        private static readonly string[] Swizzle = new[] { "x", "y", "z", "w" };

        public string MaterialSlotText =>
            $"MaterialParameters[{( ( int )Math.Floor( StartSlot / 4f ) ).ToString().PadLeft( 2, '0' )}].{Swizzle[StartSlot % 4]}";
    }
}
