using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Nodes;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffector : AvfxNodeWithData<EffectorType> {
        public const string NAME = "Efct";

        public readonly AvfxEnum<RotationOrder> RotationOrder = new( "Rotation Order", "RoOT" );
        public readonly AvfxEnum<CoordComputeOrder> CoordComputeOrder = new( "Coordinate Compute Order", "CCOT" );
        public readonly AvfxBool AffectOtherVfx = new( "Affect Other VFX", "bAOV" );
        public readonly AvfxBool AffectGame = new( "Affect Game", "bAGm" );
        public readonly AvfxInt LoopPointStart = new( "Loop Start", "LpSt" );
        public readonly AvfxInt LoopPointEnd = new( "Loop End", "LpEd" );

        private readonly List<AvfxBase> Parsed;

        private readonly UiDisplayList Parameters;

        public AvfxEffector() : base( NAME, AvfxNodeGroupSet.EffectorColor, "EfVT" ) {
            Parsed = new() {
                Type,
                RotationOrder,
                CoordComputeOrder,
                AffectOtherVfx,
                AffectGame,
                LoopPointStart,
                LoopPointEnd
            };

            Parameters = new( "Parameters", new() {
                new UiNodeGraphView( this ),
                RotationOrder,
                CoordComputeOrder,
                AffectOtherVfx,
                AffectGame,
                LoopPointStart,
                LoopPointEnd
            } );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size );

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    UpdateData();
                    Data?.Read( _reader, _size );
                }
            }, size );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) {
            RecurseAssigned( Parsed, assigned );
            RecurseAssigned( Data, assigned );
        }

        public override void WriteContents( BinaryWriter writer ) {
            WriteNested( writer, Parsed );
            Data?.Write( writer );
        }

        public override void UpdateData() {
            Data = Type.Value switch {
                EffectorType.PointLight => new AvfxEffectorDataPointLight(),
                EffectorType.DirectionalLight => new AvfxEffectorDataDirectionalLight(),
                EffectorType.RadialBlur => new AvfxEffectorDataRadialBlur(),
                EffectorType.BlackHole => null,
                EffectorType.CameraQuake2_Unknown or EffectorType.CameraQuake => new AvfxEffectorDataCameraQuake(),
                _ => null
            };
            Data?.SetAssigned( true );
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Effector" );
            DrawRename();
            Type.Draw();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) Parameters.Draw();
            }

            DrawData();
        }

        private void DrawData() {
            if( Data == null ) return;

            using var tabItem = ImRaii.TabItem( "Data" );
            if( !tabItem ) return;

            Data.Draw();
        }

        public override string GetDefaultText() => $"Effector {GetIdx()} ({Type.Value})";

        public override string GetWorkspaceId() => $"Effct{GetIdx()}";
    }
}
