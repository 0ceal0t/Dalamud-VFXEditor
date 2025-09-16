using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Data;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.ScdFormat {
    public enum SoundObjectType {
        Null,
        Ambient,
        Direction,
        Point,
        PointDir,
        Line,
        Polyline,
        Surface,
        BoardObstruction,
        BoxObstruction,
        PolylineObstruction,
        Polygon,
        BoxExtController,
        LineExtController,
        PolygonObstruction
    }

    [Flags]
    public enum SoundObjectFlags1 {
        Use_Fixed_Direction = 0x01,
        Unbounded_Distance = 0x02,
        First_Inactive = 0x04,
        Bottom_Infinity = 0x08,
        Top_Infinity = 0x10,
        Flag3D = 0x20,
        Point_Expansion = 0x40,
        Is_Little_Endian = 0x80
    }

    [Flags]
    public enum SoundObjectFlags2 {
        Is_MaxRange_Interior = 0x01,
        Use_Distance_Filters = 0x02,
        Use_Dir_First_Pos = 0x04,
        Is_Woofer_Only = 0x08,
        Is_Fixed_Volume = 0x10,
        Is_Ignore_Obstruction = 0x20,
        Is_First_Fixed_Direction = 0x40,
        Is_Local_Fixed_Direction = 0x80
    }

    public class ScdLayoutEntry : ScdEntry, IUiItem, IItemWithData<ScdLayoutData> {
        public ushort Size = 0x80;
        public readonly ParsedDataEnum<SoundObjectType, ScdLayoutData> Type;
        public readonly ParsedByte Version = new( "Version" );
        public readonly ParsedFlag<SoundObjectFlags1> Flag1 = new( "Flag1", size: 1 );
        public readonly ParsedByte GroupNumber = new( "Group Number" );
        public readonly ParsedShort LocalId = new( "Local Id" );
        public readonly ParsedInt BankId = new( "Bank Id" );
        public readonly ParsedFlag<SoundObjectFlags2> Flag2 = new( "Flag2", size: 1 );
        public readonly ParsedByte ReverbType = new( "Reverb Type" );
        public readonly ParsedShort AbGroupNumber = new( "AB Group Number" );
        public readonly ParsedFloat4 Volume = new( "Volume" );

        private ScdLayoutData Data;

        private readonly List<ParsedBase> Parsed;

        public ScdLayoutEntry() {
            Type = new( this, "Type", size: 1 );
            Parsed = [
                Type,
                Version,
                Flag1,
                GroupNumber,
                LocalId,
                BankId,
                Flag2,
                ReverbType,
                AbGroupNumber,
                Volume
            ];
        }

        public override void Read( BinaryReader reader ) {
            Size = reader.ReadUInt16();
            Parsed.ForEach( x => x.Read( reader ) );

            UpdateData();
            Data?.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Size );
            Parsed.ForEach( x => x.Write( writer ) );

            Data?.Write( writer );
        }

        public void UpdateData() {
            Data = Type.Value switch {
                SoundObjectType.Ambient => new LayoutAmbientData(),
                SoundObjectType.Direction => new LayoutDirectionData(),
                SoundObjectType.Point => new LayoutPointData(),
                SoundObjectType.PointDir => new LayoutPointDirData(),
                SoundObjectType.Line => new LayoutLineData(),
                SoundObjectType.Polyline => new LayoutPolylineData(),
                SoundObjectType.Surface => new LayoutSurfaceData(),
                SoundObjectType.BoardObstruction => new LayoutBoardObstructionData(),
                SoundObjectType.BoxObstruction => new LayoutBoxObstructionData(),
                SoundObjectType.PolylineObstruction => new LayoutPolylineObstructionData(),
                SoundObjectType.Polygon => new LayoutPolygonData(),
                SoundObjectType.LineExtController => new LayoutLineExtControllerData(),
                SoundObjectType.PolygonObstruction => new LayoutPolygonObstructionData(),
                _ => null
            };
        }

        public void SetData( ScdLayoutData data ) { Data = data; }

        public ScdLayoutData GetData() => Data;

        public void Draw() {
            Parsed.ForEach( x => x.Draw() );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );

            using var _ = ImRaii.PushId( "Data" );
            Data?.Draw();
        }
    }
}
