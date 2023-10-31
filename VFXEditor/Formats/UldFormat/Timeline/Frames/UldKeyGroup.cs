using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Data;
using VfxEditor.Ui.Interfaces;
using VfxEditor.UldFormat.Timeline.Frames;
using VfxEditor.Utils;

namespace VfxEditor.UldFormat.Timeline {
    public enum KeyUsage : int {
        Position = 0x0,
        Rotation = 0x1,
        Scale = 0x2,
        Alpha = 0x3,
        NodeColor = 0x4,
        TextColor = 0x5,
        EdgeColor = 0x6,
        Number = 0x7,
    }

    public enum KeyGroupType : int {
        Float1 = 0x0,
        Float2 = 0x1,
        Float3 = 0x2,
        SByte1 = 0x3,
        SByte2 = 0x4,
        SByte3 = 0x5,
        Byte1 = 0x6,
        Byte2 = 0x7,
        Byte3 = 0x8,
        Short1 = 0x9,
        Short2 = 0xA,
        Short3 = 0xB,
        UShort1 = 0xC,
        UShort2 = 0xD,
        UShort3 = 0xE,
        Int1 = 0xF,
        Int2 = 0x10,
        Int3 = 0x11,
        UInt1 = 0x12,
        UInt2 = 0x13,
        UInt3 = 0x14,
        Bool1 = 0x15,
        Bool2 = 0x16,
        Bool3 = 0x17,
        Color = 0x18,
        Label = 0x19,
        Number = 0x1A,
    }

    public class UldKeyGroup : IUiItem {
        public readonly ParsedEnum<KeyUsage> Usage = new( "Usage", size: 2 );
        public readonly ParsedDataListEnum<KeyGroupType, UldKeyframe> Type;

        public readonly List<UldKeyframe> Keyframes = new();

        public UldKeyGroup() {
            Type = new( Keyframes, "Type", size: 2 );
        }

        public UldKeyGroup( BinaryReader reader ) : this() {
            var pos = reader.BaseStream.Position;

            Usage.Read( reader );
            Type.Read( reader );
            var size = reader.ReadUInt16();
            var count = reader.ReadUInt16();

            for( var i = 0; i < count; i++ ) {
                Keyframes.Add( new( reader, Type.Value ) );
            }

            reader.BaseStream.Position = pos + size;
        }

        public void Write( BinaryWriter writer ) {
            var pos = writer.BaseStream.Position;

            Usage.Write( writer );
            Type.Write( writer );

            var savePos = writer.BaseStream.Position;
            writer.Write( ( ushort )0 );

            writer.Write( ( ushort )Keyframes.Count );
            Keyframes.ForEach( x => x.Write( writer ) );

            var finalPos = writer.BaseStream.Position;
            var size = finalPos - pos;
            writer.BaseStream.Position = savePos;
            writer.Write( ( ushort )size );
            writer.BaseStream.Position = finalPos;
        }

        public void Draw() {
            Usage.Draw();
            Type.Draw();

            for( var idx = 0; idx < Keyframes.Count; idx++ ) {
                if( DrawKeyframe( Keyframes[idx], idx ) ) break;
            }

            if( ImGui.Button( "+ New" ) ) { // NEW
                CommandManager.Add( new GenericAddCommand<UldKeyframe>( Keyframes, new UldKeyframe( Type.Value ) ) );
            }
        }

        private bool DrawKeyframe( UldKeyframe item, int idx ) {
            using var _ = ImRaii.PushId( idx );
            if( ImGui.CollapsingHeader( $"Keyframe {idx}" ) ) {
                using var indent = ImRaii.PushIndent();

                if( UiUtils.RemoveButton( $"Delete", true ) ) { // REMOVE
                    CommandManager.Add( new GenericRemoveCommand<UldKeyframe>( Keyframes, item ) );
                    return true;
                }

                item.Draw();
            }
            return false;
        }
    }
}
