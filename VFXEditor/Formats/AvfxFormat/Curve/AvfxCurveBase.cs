using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Formats.AvfxFormat.Curve {
    public abstract class AvfxCurveBase : AvfxOptional {
        public readonly string Name;
        public readonly bool Locked;

        protected AvfxCurveBase( string name, string avfxName, bool locked ) : base( avfxName ) {
            Name = name;
            Locked = locked;
        }

        public override string GetDefaultText() => Name;

        public override void DrawUnassigned() {
            using var _ = ImRaii.PushId( Name );

            AssignedCopyPaste( Name );
            DrawAssignButton( Name, true );
        }

        public override void DrawAssigned() {
            using var _ = ImRaii.PushId( Name );
            AssignedCopyPaste( Name );
            if( !Locked && DrawUnassignButton( Name ) ) return;

            DrawBody();
        }

        protected abstract void DrawBody();

        // ======== STATIC DRAW ==========

        public static void DrawUnassignedCurves( List<AvfxCurveBase> curves ) {
            var first = true;
            var currentX = 0f;
            var maxX = ImGui.GetContentRegionAvail().X;
            var imguiStyle = ImGui.GetStyle();
            var spacing = imguiStyle.ItemInnerSpacing.X;

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( spacing, imguiStyle.ItemSpacing.Y ) );

            foreach( var curve in curves.Where( x => !x.IsAssigned() ) ) {
                var width = ImGui.CalcTextSize( $"+ {curve.Name}" ).X + imguiStyle.FramePadding.X * 2 + spacing;
                if( first ) {
                    currentX += width;
                }
                else {
                    if( ( maxX - currentX - width ) > spacing + 4 ) {
                        currentX += width;
                        ImGui.SameLine();
                    }
                    else {
                        currentX = width;
                    }
                }

                curve.Draw();
                first = false;
            }
        }

        public static void DrawAssignedCurves( List<AvfxCurveBase> curves ) {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
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
