using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UICurve : UIBase
    {
        public AVFXCurve Curve;
        public string Name;
        public bool Color = false;
        //=======================
        public List<UIKey> Keys;

        public UICurve(AVFXCurve curve, string name, bool color=false)
        {
            Curve = curve;
            Name = name;
            Color = color;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Keys = new List<UIKey>();
            if (!Curve.Assigned) { Assigned = false; return; }
            //=====================
            Attributes.Add(new UICombo<CurveBehavior>("Pre Behavior", Curve.PreBehavior));
            Attributes.Add(new UICombo<CurveBehavior>("Post Behavior", Curve.PostBehavior));
            if (!Color)
            {
                Attributes.Add(new UICombo<RandomType>("Random Type", Curve.Random));
            }
            foreach (var key in Curve.Keys)
            {
                Keys.Add(new UIKey(key, this, Color));
            }
        }

        // ======= DRAW ================
        public override void Draw( string parentId )
        {
            if( !Assigned )
            {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.TreeNode( Name + parentId ) )
            {
                DrawBody( parentId );
                ImGui.TreePop();
            }
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.Selectable( Name + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        private void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ " + Name + parentId ) )
            {
                Curve.toDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/" + Name;
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                Curve.Assigned = false;
                Init();
            }
            // =====================
            DrawAttrs( id );
            if( ImGui.TreeNode( "Keys" + id ) )
            {
                int keyIdx = 0;
                foreach( var key in Keys )
                {
                    key.Idx = keyIdx;
                    key.Draw( id );
                    keyIdx++;
                }

                if( ImGui.Button( "+ Key" + id ) )
                {
                    Curve.addKey();
                    Init();
                }
                ImGui.TreePop();
            }
            ImGui.TreePop();
        }
    }

    public class UIKey
    {
        public AVFXKey Key;
        public UICurve Curve;
        public int Idx;
        // ====================
        public int Time;
        public Vector3 Data;
        public bool Color;
        public static readonly string[] TypeOptions = Enum.GetNames(typeof(KeyType));
        public int TypeIdx;

        public UIKey(AVFXKey key, UICurve curve, bool color=false)
        {
            Key = key;
            Curve = curve;
            Time = key.Time;
            Color = color;
            Data = new Vector3(key.X, key.Y, key.Z);
            TypeIdx = Array.IndexOf(TypeOptions, Key.Type.ToString());
        }

        public void Draw(string parentId)
        {
            string id = parentId + "/Key" + Idx;

            if (UIUtils.RemoveButton("Delete Key" + id))
            {
                Curve.Curve.removeKey(Idx);
                Curve.Init();
            }
            if (ImGui.InputInt("Time" + id, ref Time))
            {
                Key.Time = Time;
            }
            if (UIUtils.EnumComboBox("Type" + id, TypeOptions, ref TypeIdx))
            {
                Enum.TryParse(TypeOptions[TypeIdx], out KeyType newKeyType);
                Key.Type = newKeyType;
            }
            //=====================
            if (Color)
            {
                if (ImGui.ColorEdit3("Color" + id, ref Data, ImGuiColorEditFlags.Float))
                {
                    Key.X = Data.X;
                    Key.Y = Data.Y;
                    Key.Z = Data.Z;
                }
            }
            else
            {
                if (ImGui.InputFloat3("Value" + id, ref Data))
                {
                    Key.X = Data.X;
                    Key.Y = Data.Y;
                    Key.Z = Data.Z;
                }
            }
        }
    }
}
