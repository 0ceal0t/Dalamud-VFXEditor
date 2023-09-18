using ImGuiNET;
using SharpDX;
using System.IO;
using VfxEditor.EidFormat.BindPoint;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;

namespace VfxEditor.EidFormat {
    public class EidBindPointNew : EidBindPoint {
        public readonly ParsedPaddedString Name = new( "Bone Name", "n_root", 32, 0x00 );
        public readonly ParsedInt Id = new( "Id" );
        public readonly ParsedFloat3 Position = new( "Position" );
        public readonly ParsedRadians3 Rotation = new( "Rotation" );

        public EidBindPointNew() { }

        public EidBindPointNew( BinaryReader reader ) {
            Name.Read( reader );
            Id.Read( reader );
            Position.Read( reader );
            Rotation.Read( reader );
            reader.ReadInt32(); // padding
        }

        public override void Write( BinaryWriter writer ) {
            Name.Write( writer );
            Id.Write( writer );
            Position.Write( writer );
            Rotation.Write( writer );
            writer.Write( 0 );
        }

        public override string GetName() => $"{Id.Value} ({Name.Value})";

        public override void Draw() {
            ImGui.TextDisabled( "Data Version: [NEW]" );

            Name.Draw( CommandManager.Eid );
            Id.Draw( CommandManager.Eid );
            Position.Draw( CommandManager.Eid );
            Rotation.Draw( CommandManager.Eid );
        }

        protected override Vector3 GetOffset() => new( Position.Value.X, Position.Value.Y, Position.Value.Z );

        protected override Quaternion GetRotation() => Quaternion.RotationYawPitchRoll( Rotation.Value.X, Rotation.Value.Y, Rotation.Value.Z );

        protected override string GetBoneName() => Name.Value;
    }
}
