using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils {
    public class SoundEffectParam : IUiItem {
        private readonly ParsedEnum<SoundEffectType> Type = new( "Sound Effect Type" );
        private readonly ParsedByteBool AutoPlay = new( "Auto Play" );
        private readonly ParsedByteBool IsNoFarClip = new( "No Far Clip" );
        private readonly ParsedUInt PointSelection = new( "Point Selection" );

        private byte[] Unknown = Array.Empty<byte>();

        public SoundEffectParam() { }

        public void Read( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position;
            Type.Read( reader );
            AutoPlay.Read( reader );
            IsNoFarClip.Read( reader );
            reader.ReadBytes( 2 ); // padding
            var offset = reader.ReadUInt32();
            var count = reader.ReadInt32();
            PointSelection.Read( reader );
            var endPos = reader.BaseStream.Position;

            reader.BaseStream.Position = startPos + offset;
            Unknown = reader.ReadBytes( count );

            reader.BaseStream.Position = endPos; // reset
        }

        public void Draw() {
            Type.Draw();
            AutoPlay.Draw();
            IsNoFarClip.Draw();
            PointSelection.Draw();

            ImGui.TextDisabled( $"Data length: 0x{Unknown.Length:X8}" );
        }
    }
}
