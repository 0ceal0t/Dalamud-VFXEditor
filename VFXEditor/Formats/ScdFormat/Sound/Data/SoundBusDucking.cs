using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.ScdFormat.Sound.Data {
    [Flags]
    public enum BusOverride {
        Override_1 = 0x01,
        Override_2 = 0x02,
        Override_3 = 0x04,
        Override_4 = 0x08,
    }
    public class SoundBusDucking {
        private byte Size = 0x10;
        public readonly ParsedByte Number = new( "Number" );
        private readonly ParsedByteBool ExtraBusEnabled = new( "Enable Bus Overrides" );
        private readonly ParsedFlag<BusOverride> BusOverrides = new( "Overrides", size: 1 );
        public readonly ParsedInt FadeTime = new( "Fade Time" );
        public readonly ParsedFloat Volume = new( "Volume" );
        private readonly ParsedIntByte4 ExtraNum = new( "Bus Overrides" );

        public void Read( BinaryReader reader ) {
            Size = reader.ReadByte();
            Number.Read( reader );
            ExtraBusEnabled.Read( reader );
            BusOverrides.Read( reader );
            FadeTime.Read( reader );
            Volume.Read( reader );
            ExtraNum.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Size );
            Number.Write( writer );
            ExtraBusEnabled.Write( writer );
            BusOverrides.Write( writer );
            FadeTime.Write( writer );
            Volume.Write( writer );
            ExtraNum.Write( writer );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "BusDucking" );

            Number.Draw();
            FadeTime.Draw();
            Volume.Draw();
            ImGui.NewLine();
            ImGui.Separator();
            ImGui.NewLine();
            ExtraBusEnabled.Draw();
            using( var indent = ImRaii.PushIndent() ) {
                BusOverrides.Draw();
            }
            ExtraNum.Draw();
        }
    }
}
