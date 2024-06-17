using System;
using System.IO;
using VfxEditor.Data.Command;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.ShpkFormat.Materials {
    public enum MaterialParameterSize : int {
        Float = 4,
        Vector2 = 8,
        Vector3 = 12,
        Vector4 = 16
    }

    public class ShpkMaterialParmeter : IUiItem {
        public readonly ShpkFile File;
        public readonly ParsedUIntHex Id = new( "Id" );
        public readonly ParsedShort Offset = new( "Offset" );
        public readonly ParsedEnum<MaterialParameterSize> Size = new( "Size", size: 2 );

        public ParsedBase DefaultValue { get; private set; }

        public int StartSlot => Offset.Value / 4;
        public int EndSlot => ( Offset.Value + ( int )Size.Value ) / 4;

        public ShpkMaterialParmeter( ShpkFile file ) {
            File = file;
        }

        public ShpkMaterialParmeter( ShpkFile file, BinaryReader reader ) : this( file ) {
            Id.Read( reader );
            Offset.Read( reader );
            Size.Read( reader );
            UpdateDefaultValue();
        }

        private void UpdateDefaultValue() {
            DefaultValue = Size.Value switch {
                MaterialParameterSize.Float => new ParsedFloat( "Default Value" ),
                MaterialParameterSize.Vector2 => new ParsedFloat2( "Default Value" ),
                MaterialParameterSize.Vector3 => new ParsedFloat3( "Default Value" ),
                MaterialParameterSize.Vector4 => new ParsedFloat4( "Default Value" ),
                _ => null
            };
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            Offset.Write( writer );
            Size.Write( writer );
        }

        public void Draw() {
            Id.Draw();
            Offset.Draw();
            using( var editing = new Edited() ) {
                Size.Draw();
                if( File.HasDefaultMaterialValues.Value && editing.IsEdited ) UpdateDefaultValue();
            }

            if( File.HasDefaultMaterialValues.Value ) DefaultValue?.Draw();
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
