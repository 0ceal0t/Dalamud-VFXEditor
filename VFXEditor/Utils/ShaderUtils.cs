using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.ShpkFormat.Shaders;

namespace VfxEditor.Utils {
    public static class ShaderUtils {
        public enum ShaderStage {
            Vertex = 0,
            Pixel = 1,
            Geometry = 2,
            Compute = 3,
            Hull = 4,
            Domain = 5,
        }

        public enum ShaderFileType {
            Shpk,
            Shcd
        }

        public enum DX {
            DX9,
            DX11,
            UNKNOWN
        }

        public static DX GetDxVersion( uint magic ) => magic switch {
            0x00395844u => DX.DX9,
            0x31315844u => DX.DX11,
            _ => DX.UNKNOWN
        };

        public static CommandManager GetCommand( ShaderFileType type ) => type switch {
            ShaderFileType.Shpk => CommandManager.Shpk,
            ShaderFileType.Shcd => CommandManager.Shcd,
            _ => null
        };

        public static void WriteOffsets( BinaryWriter writer, long placeholderPos, List<(long, string)> stringPositions, List<(long, ShpkShader)> shaderPositions ) {
            var shaderOffset = writer.BaseStream.Position;

            shaderPositions.ForEach( x => x.Item2.WriteByteCode( writer, shaderOffset, x.Item1 ) );

            var parameterOffset = writer.BaseStream.Position;

            var stringOffsets = new Dictionary<string, uint>();
            foreach( var item in stringPositions ) {
                var value = item.Item2;
                if( stringOffsets.ContainsKey( value ) ) continue;

                stringOffsets[value] = ( uint )( writer.BaseStream.Position - parameterOffset );
                FileUtils.WriteString( writer, value, true );
            }

            foreach( var item in stringPositions ) {
                var offset = stringOffsets[item.Item2];
                writer.BaseStream.Seek( item.Item1, SeekOrigin.Begin );
                writer.Write( offset );
            }

            writer.BaseStream.Seek( placeholderPos, SeekOrigin.Begin );
            writer.Write( ( uint )writer.BaseStream.Length );
            writer.Write( ( uint )shaderOffset );
            writer.Write( ( uint )parameterOffset );
        }
    }
}
