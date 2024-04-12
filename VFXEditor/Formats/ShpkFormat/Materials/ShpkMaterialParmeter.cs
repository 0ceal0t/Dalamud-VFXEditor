using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Materials {
    public class ShpkMaterialParmeter : IUiItem {
        public readonly ParsedUIntHex Id = new( "Id" );
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
            Id.Draw();
            Offset.Draw();
            Size.Draw();
        }

        private static readonly string[] Swizzle = ["x", "y", "z", "w"];

        public string GetText() {
            var ret = $"g_MaterialParameter[{( int )Math.Floor( StartSlot / 4f ):D2}].";
            for( var slot = StartSlot; slot < EndSlot; slot++ ) {
                ret += $"{Swizzle[slot % 4]}";
            }
            return ret;
        }
    }
}
