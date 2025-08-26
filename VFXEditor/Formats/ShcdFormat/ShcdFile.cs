using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.ShpkFormat.Shaders;
using VfxEditor.Parsing;
using VfxEditor.Utils;
using static VfxEditor.Utils.ShaderUtils;

namespace VfxEditor.Formats.ShcdFormat {
    public class ShcdFile : FileManagerFile {
        private readonly byte[] Version;
        private readonly uint DxMagic;
        public DX DxVersion => GetDxVersion( DxMagic );
        public bool Shcd3 => Version[1] == 3;

        public readonly ParsedEnum<ShaderStage> Stage = new( "Stage", 1 );
        public readonly ShpkShader Shader;

        public ShcdFile( BinaryReader reader, bool verify ) : base() {
            reader.ReadInt32(); // Magic
            Version = reader.ReadBytes( 3 );
            Stage.Read( reader );
            DxMagic = reader.ReadUInt32();

            reader.ReadInt32(); // File length
            var shaderOffset = reader.ReadUInt32();
            var parameterOffset = reader.ReadUInt32();

            Shader = new( reader, Stage.Value, DxVersion, !Shcd3, ShaderFileType.Shcd, false );
            Shader.Read( reader, parameterOffset, shaderOffset );

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( 0x64436853u );
            writer.Write( Version );
            Stage.Write( writer );
            writer.Write( DxMagic );

            var placeholderPos = writer.BaseStream.Position;
            writer.Write( 0 ); // size
            writer.Write( 0 ); // shader offset
            writer.Write( 0 ); // parameter offset

            var stringPositions = new List<(long, string)>();
            var shaderPositions = new List<(long, ShpkShader)>();

            Shader.Write( writer, stringPositions, shaderPositions );

            WriteOffsets( writer, placeholderPos, stringPositions, shaderPositions );
        }

        public override void Draw() {
            ImGui.Separator();
            ImGui.TextDisabled( $"Version: {Version[1]} DirectX: {DxVersion}" );

            Stage.Draw();
            Shader.Draw();
        }
    }
}
