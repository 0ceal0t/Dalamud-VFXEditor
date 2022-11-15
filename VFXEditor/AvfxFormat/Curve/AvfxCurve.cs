using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurve : AvfxOptional {
        public readonly AvfxEnum<CurveBehavior> PreBehavior = new( "Pre Behavior", "BvPr" );
        public readonly AvfxEnum<CurveBehavior> PostBehavior = new( "Post Behavior", "BvPo" );
        public readonly AvfxEnum<RandomType> Random = new( "RandomType", "RanT" );
        public readonly AvfxCurveKeys Keys = new();

        private readonly List<AvfxBase> Parsed;
        private readonly List<IAvfxUiBase> Display;
        private readonly UiCurveEditor CurveEditor;

        private readonly string Name;
        private readonly bool Color;
        private readonly bool Locked;

        public AvfxCurve( string name, string avfxName, bool color = false, bool locked = false ) : base( avfxName ) {
            Name = name;
            Color = color;
            Locked = locked;

            Parsed = new() {
                PreBehavior,
                PostBehavior,
                Random,
                Keys
            };

            CurveEditor = new UiCurveEditor( this, Color );
            Display = new() {
                PreBehavior,
                PostBehavior
            };
            if( !Color ) Display.Add( Random );
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

        public override void DrawUnassigned( string parentId ) {
            AssignedCopyPaste( this, Name );
            DrawAddButtonRecurse( this, Name, parentId );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            AssignedCopyPaste( this, Name );
            if( !Locked && DrawRemoveButton( this, Name, id ) ) return;
            IAvfxUiBase.DrawList( Display, id );
            CurveEditor.Draw( id );
        }

        public override string GetDefaultText() => Name;

        // ======== STATIC DRAW ==========

        public static void DrawUnassignedCurves( List<AvfxCurve> curves, string id ) {
            var idx = 0;
            foreach( var curve in curves ) {
                if( !curve.IsAssigned() ) {
                    if( idx % 5 != 0 ) ImGui.SameLine();
                    curve.Draw( id );
                    idx++;
                }
            }
        }

        public static void DrawAssignedCurves( List<AvfxCurve> curves, string id ) {
            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                foreach( var curve in curves ) {
                    if( curve.IsAssigned() ) {
                        if( ImGui.BeginTabItem( curve.Name + id ) ) {
                            curve.DrawAssigned( id );
                            ImGui.EndTabItem();
                        }
                    }
                }
                ImGui.EndTabBar();
            }
        }
    }
}
