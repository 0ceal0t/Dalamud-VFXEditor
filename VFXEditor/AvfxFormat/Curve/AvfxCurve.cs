using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public enum CurveType {
        Color,
        Angle,
        Base
    }

    public class AvfxCurve : AvfxOptional {
        public readonly AvfxEnum<CurveBehavior> PreBehavior = new( "Pre Behavior", "BvPr" );
        public readonly AvfxEnum<CurveBehavior> PostBehavior = new( "Post Behavior", "BvPo" );
        public readonly AvfxEnum<RandomType> Random = new( "RandomType", "RanT" );
        public readonly AvfxCurveKeys Keys = new();

        private readonly List<AvfxBase> Parsed;
        private readonly List<IUiItem> Display;
        private readonly UiCurveEditor CurveEditor;

        private readonly string Name;
        private readonly bool Locked;

        public AvfxCurve( string name, string avfxName, CurveType type = CurveType.Base, bool locked = false ) : base( avfxName ) {
            Name = name;
            Locked = locked;

            Parsed = new() {
                PreBehavior,
                PostBehavior,
                Random,
                Keys
            };

            CurveEditor = new UiCurveEditor( this, type );
            Display = new() {
                PreBehavior,
                PostBehavior
            };
            if( type != CurveType.Color ) Display.Add( Random );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            CurveEditor.Initialize();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            WriteLeaf( writer, "KeyC", 4, Keys.Keys.Count );
            WriteNested( writer, Parsed );
        }

        public override void DrawUnassigned() {
            using var _ = ImRaii.PushId( Name );

            AssignedCopyPaste( this, Name );
            DrawAddButtonRecurse( this, Name );
        }

        public override void DrawAssigned() {
            using var _ = ImRaii.PushId( Name );

            AssignedCopyPaste( this, Name );
            if( !Locked && DrawRemoveButton( this, Name ) ) return;
            DrawItems( Display );
            CurveEditor.Draw();
        }

        public override string GetDefaultText() => Name;

        // ======== STATIC DRAW ==========

        public static void DrawUnassignedCurves( List<AvfxCurve> curves ) {
            var assignedIdx = 0;
            foreach( var curve in curves ) {
                if( !curve.IsAssigned() ) {
                    if( assignedIdx % 5 != 0 ) ImGui.SameLine();
                    curve.Draw();
                    assignedIdx++;
                }
            }
        }

        public static void DrawAssignedCurves( List<AvfxCurve> curves ) {
            using var tabBar = ImRaii.TabBar( $"Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            foreach( var curve in curves.Where( x => x.IsAssigned() ) ) {
                if( ImGui.BeginTabItem( curve.Name ) ) {
                    curve.DrawAssigned();
                    ImGui.EndTabItem();
                }
            }
        }
    }
}
