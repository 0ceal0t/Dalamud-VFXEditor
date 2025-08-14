using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Utils.Gltf {
    [Serializable]
    public class ExcludedBonesConfiguration {
        public string Name = "New Exclude List";
        public List<ExcludedBoneConfiguration> Bones = [];

        public void Draw() {
            using var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );

            foreach( var (bone, idx) in Bones.WithIndex() ) {
                using var _ = ImRaii.PushId( idx );
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Minus.ToIconString() ) ) {
                        Bones.Remove( bone );
                        break;
                    }
                }
                ImGui.SameLine();
                bone.Draw();
            }

            using var font2 = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) Bones.Add( new() );
        }
    }

    [Serializable]
    public class ExcludedBoneConfiguration {
        public string BoneName = "";

        public void Draw() {
            ImGui.InputTextWithHint( "##Bone", "Bone Name", ref BoneName, 255 );
        }
    }
}
