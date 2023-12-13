using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils {
    public class MovePathSettings : IUiItem {
        private readonly ParsedEnum<MovePathMode> Mode = new( "Mode" );
        private readonly ParsedByteBool AutoPlay = new( "Auto Play" );
        private readonly ParsedShort Time = new( "Time" );
        private readonly ParsedByteBool Loop = new( "Loop" );
        private readonly ParsedByteBool Reverse = new( "Reverse" );
        private readonly ParsedEnum<RotationType> Rotation = new( "Rotation Type" );
        private readonly ParsedShort AccelerateTime = new( "Accelerate Time" );
        private readonly ParsedShort DecelerateTime = new( "Decelerate Time" );
        private readonly ParsedFloat2 VerticalSwingRange = new( "Vertical Swing Range" );
        private readonly ParsedFloat2 HorizontalSwingRange = new( "Horizontal Swing Range" );
        private readonly ParsedFloat2 SwingMoveSpeedRange = new( "Swing Move Speed Range" );
        private readonly ParsedFloat2 SwingRotation = new( "Swing Rotation" );
        private readonly ParsedFloat2 SwingRotationSpeedRange = new( "Swing Rotation Speed Range" );

        public MovePathSettings() { }

        public void Read( BinaryReader reader ) {
            Mode.Read( reader );
            AutoPlay.Read( reader );
            reader.ReadByte(); // padding
            Time.Read( reader );
            Loop.Read( reader );
            Reverse.Read( reader );
            reader.ReadBytes( 2 ); // padding
            Rotation.Read( reader );
            AccelerateTime.Read( reader );
            DecelerateTime.Read( reader );
            VerticalSwingRange.Read( reader );
            HorizontalSwingRange.Read( reader );
            SwingMoveSpeedRange.Read( reader );
            SwingRotation.Read( reader );
            SwingRotationSpeedRange.Read( reader );
        }

        public void Draw() {
            Mode.Draw();
            AutoPlay.Draw();
            Time.Draw();
            Loop.Draw();
            Reverse.Draw();
            Rotation.Draw();
            AccelerateTime.Draw();
            DecelerateTime.Draw();
            VerticalSwingRange.Draw();
            HorizontalSwingRange.Draw();
            SwingMoveSpeedRange.Draw();
            SwingRotation.Draw();
            SwingRotationSpeedRange.Draw();
        }
    }
}
