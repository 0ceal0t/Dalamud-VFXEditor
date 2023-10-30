using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Nodes;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxBinder : AvfxNodeWithData<BinderType> {
        public const string NAME = "Bind";

        public readonly AvfxBool StartToGlobalDirection = new( "Start To Global Direction", "bStG" );
        public readonly AvfxBool VfxScaleEnabled = new( "VFX Scale", "bVSc" );
        public readonly AvfxFloat VfxScaleBias = new( "VFX Scale Bias", "bVSb" );
        public readonly AvfxBool VfxScaleDepthOffset = new( "VFX Scale Depth Offset", "bVSd" );
        public readonly AvfxBool VfxScaleInterpolation = new( "VFX Scale Interpolation", "bVSi" );
        public readonly AvfxBool TransformScale = new( "Transform Scale", "bTSc" );
        public readonly AvfxBool TransformScaleDepthOffset = new( "Transform Scale Depth Offset", "bTSd" );
        public readonly AvfxBool TransformScaleInterpolation = new( "Transform Scale Interpolation", "bTSi" );
        public readonly AvfxBool FollowingTargetOrientation = new( "Following Target Orientation", "bFTO" );
        public readonly AvfxBool DocumentScaleEnabled = new( "Document Scale Enabled", "bDSE" );
        public readonly AvfxBool AdjustToScreenEnabled = new( "Adjust to Screen", "bATS" );
        public readonly AvfxBool BET_Unknown = new( "BET (Unknown)", "bBET" );
        public readonly AvfxInt Life = new( "Life", "Life" );
        public readonly AvfxEnum<BinderRotation> BinderRotationType = new( "Binder Rotation Type", "RoTp" );
        public readonly AvfxBinderProperties PropStart = new( "Properties Start", "PrpS" );
        public readonly AvfxBinderProperties Prop1 = new( "Properties 1", "Prp1" );
        public readonly AvfxBinderProperties Prop2 = new( "Properties 2", "Prp2" );
        public readonly AvfxBinderProperties PropGoal = new( "Properties Goal", "PrpG" );

        private readonly List<AvfxBase> Parsed;

        private readonly AvfxDisplaySplitView<AvfxBinderProperties> PropSplitDisplay;
        private readonly UiDisplayList Parameters;

        public AvfxBinder() : base( NAME, AvfxNodeGroupSet.BinderColor, "BnVr" ) {
            Parsed = new() {
                StartToGlobalDirection,
                VfxScaleEnabled,
                VfxScaleBias,
                VfxScaleDepthOffset,
                VfxScaleInterpolation,
                TransformScale,
                TransformScaleDepthOffset,
                TransformScaleInterpolation,
                FollowingTargetOrientation,
                DocumentScaleEnabled,
                AdjustToScreenEnabled,
                BET_Unknown,
                Life,
                BinderRotationType,
                Type,
                PropStart,
                Prop1,
                Prop2,
                PropGoal
            };

            Parameters = new( "Parameters", new() {
                new UiNodeGraphView( this ),
                StartToGlobalDirection,
                VfxScaleEnabled,
                VfxScaleBias,
                VfxScaleDepthOffset,
                VfxScaleInterpolation,
                TransformScale,
                TransformScaleDepthOffset,
                TransformScaleInterpolation,
                FollowingTargetOrientation,
                DocumentScaleEnabled,
                AdjustToScreenEnabled,
                BET_Unknown,
                Life,
                BinderRotationType
            } );

            PropSplitDisplay = new AvfxDisplaySplitView<AvfxBinderProperties>( "Properties", new() {
                PropStart,
                Prop1,
                Prop2,
                PropGoal
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
                BinderType.Point => new AvfxBinderDataPoint(),
                BinderType.Linear => new AvfxBinderDataLinear(),
                BinderType.Spline => new AvfxBinderDataSpline(),
                BinderType.Camera => new AvfxBinderDataCamera(),
                BinderType.Unknown_4 => new AvfxBinderDataUnknown4(),
                _ => null,
            };
            Data?.SetAssigned( true );
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Binder" );
            DrawRename();
            Type.Draw();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) Parameters.Draw();
            }

            DrawData();

            using( var tab = ImRaii.TabItem( "Properties" ) ) {
                if( tab ) PropSplitDisplay.Draw();
            }
        }

        private void DrawData() {
            if( Data == null ) return;

            using var tabItem = ImRaii.TabItem( "Data" );
            if( !tabItem ) return;

            Data.Draw();
        }

        public override string GetDefaultText() => $"Binder {GetIdx()} ({Type.Value})";

        public override string GetWorkspaceId() => $"Bind{GetIdx()}";
    }
}
