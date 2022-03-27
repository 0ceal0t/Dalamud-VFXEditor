using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class UICurve2Axis : UIItem {
        public AVFXCurve2Axis Curve;
        public string Name;
        public bool Locked;
        public UICombo<AxisConnect> AxisConnectSelect;
        public UICombo<RandomType> AxisConnectRandomSelect;
        public UICurve X;
        public UICurve Y;
        private List<UICurve> Curves;

        public UICurve2Axis( AVFXCurve2Axis curve, string name, bool locked = false ) {
            Curve = curve;
            Name = name;
            Locked = locked;
            Init();
        }

        public override void Init() {
            base.Init();
            Curves = new List<UICurve>();
            if( !Curve.Assigned ) { Assigned = false; return; }

            AxisConnectSelect = new UICombo<AxisConnect>( "Axis Connect", Curve.AxisConnectType );
            AxisConnectRandomSelect = new UICombo<RandomType>( "Axis Connect Random", Curve.AxisConnectRandomType );

            Curves.Add( X = new UICurve( Curve.X, "X" ) );
            Curves.Add( Y = new UICurve( Curve.Y, "Y" ) );
            Curves.Add( new UICurve( Curve.RX, "Random X" ) );
            Curves.Add( new UICurve( Curve.RY, "Random Y" ) );
        }

        public override void Draw( string parentId ) {
            if( !Assigned ) {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.TreeNode( Name + parentId ) ) {
                DrawBody( parentId );
                ImGui.TreePop();
            }
        }

        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                Curve.ToDefault();
                Init();
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked ) {
                if( UiHelper.RemoveButton( "Delete " + Name + id, small: true ) ) {
                    Curve.Assigned = false;
                    Init();
                    return;
                }
            }

            var idx = 0;
            foreach( var c in Curves ) {
                if( !c.Assigned ) {
                    if( idx % 5 != 0 ) {
                        ImGui.SameLine();
                    }
                    c.Draw( id );
                    idx++;
                }
            }

            AxisConnectSelect.Draw( id );
            AxisConnectRandomSelect.Draw( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                foreach( var c in Curves ) {
                    if( c.Assigned ) {
                        if( ImGui.BeginTabItem( c.Name + id ) ) {
                            c.DrawBody( id );
                            ImGui.EndTabItem();
                        }
                    }
                }
                ImGui.EndTabBar();
            }
        }

        public override string GetDefaultText() => Name;
    }
}
