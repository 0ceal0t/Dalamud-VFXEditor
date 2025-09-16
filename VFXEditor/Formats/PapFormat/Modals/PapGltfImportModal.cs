using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.PapFormat {
    public unsafe class PapGltfImportModal : Modal {
        private readonly PapMotion Motion;
        private readonly int HavokIndex;
        private readonly string ImportPath;

        private bool Compress = false;
        private bool SkipOriginal = false;
        private int AnimationIndex = 0;
        private readonly List<string> AnimationNames = [];
        private readonly List<string> NodeNames = [];

        private bool Exclude = false;
        private ExcludedBonesConfiguration SelectedExcludeList;

        public PapGltfImportModal( PapMotion motion, int index, string importPath ) : base( "Animation Import", true ) {
            Motion = motion;
            HavokIndex = index;
            ImportPath = importPath;

            var model = ModelRoot.Load( importPath );

            var boneNames = new List<string>();
            for( var i = 0; i < motion.Skeleton->Bones.Length; i++ ) {
                boneNames.Add( motion.Skeleton->Bones[i].Name.String );
            }

            foreach( var node in model.LogicalNodes ) {
                if( string.IsNullOrEmpty( node.Name ) ||
                    node.Name.Contains( "mesh", StringComparison.CurrentCultureIgnoreCase ) ||
                    node.Name.Contains( "armature", StringComparison.CurrentCultureIgnoreCase )
                ) continue;
                if( !boneNames.Contains( node.Name ) || !node.IsTransformAnimated ) continue;
                NodeNames.Add( node.Name );
            }

            foreach( var animation in model.LogicalAnimations ) {
                AnimationNames.Add( animation.Name );
            }
        }

        protected override void DrawBody() {
            if( UiUtils.IconButton( FontAwesomeIcon.InfoCircle, "Wiki" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Blender-to-Edit-Skeletons-and-Animations" );

            using var nodes = ImRaii.TreeNode( "Nodes Being Imported" );
            if( nodes ) {
                using var child = ImRaii.Child( "Child", new( ImGui.GetContentRegionAvail().X, 300 ) );
                using var _ = ImRaii.PushIndent();
                foreach( var nodeName in NodeNames ) {
                    ImGui.Text( nodeName );
                }
            }

            var text = AnimationNames.Count == 0 ? "[NONE]" : AnimationNames[AnimationIndex];
            using( var combo = ImRaii.Combo( "Animation to Import", text ) ) {
                if( combo ) {
                    for( var i = 0; i < AnimationNames.Count; i++ ) {
                        using var _ = ImRaii.PushId( i );
                        if( ImGui.Selectable( $"{AnimationNames[i]}##Name", i == AnimationIndex ) ) {
                            AnimationIndex = i;
                        }
                    }
                }
            }

            ImGui.Checkbox( "Compress Animation", ref Compress );
            ImGui.Checkbox( "Skip Bones Unanimated in Original Motion", ref SkipOriginal );
            ImGui.Checkbox( "Exclude Bones", ref Exclude );

            if( Exclude ) {
                using var _ = ImRaii.PushId( "Exclude" );
                using var __ = ImRaii.PushIndent( 10f );
                DrawExclude();
            }
        }

        private void DrawExclude() {
            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.BeginCombo( "##List", SelectedExcludeList == null ? "[NONE]" : SelectedExcludeList.Name ) ) {
                    using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                        if( ImGui.Selectable( FontAwesomeIcon.Plus.ToIconString(), false, ImGuiSelectableFlags.SpanAllColumns ) ) {
                            var newList = new ExcludedBonesConfiguration();
                            SelectedExcludeList = newList;
                            Plugin.Configuration.ExcludedBones.Add( newList );
                        }
                    }
                    ImGui.SameLine();
                    ImGui.Text( "New List" );

                    ImGui.Separator();

                    using( var _ = ImRaii.PushColor( ImGuiCol.Text, ImGui.GetColorU32( ImGuiCol.TextDisabled ) ) ) {
                        if( ImGui.Selectable( "[NONE]" ) ) SelectedExcludeList = null;
                    }

                    foreach( var item in Plugin.Configuration.ExcludedBones ) {
                        if( ImGui.Selectable( item.Name, SelectedExcludeList == item ) ) SelectedExcludeList = item;
                    }

                    ImGui.EndCombo();
                }

                if( SelectedExcludeList != null ) {
                    using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                        ImGui.SameLine();
                        if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                            Plugin.Configuration.ExcludedBones.Remove( SelectedExcludeList );
                            SelectedExcludeList = null;
                        }

                        ImGui.SameLine();
                        if( ImGui.Button( FontAwesomeIcon.Edit.ToIconString() ) ) ImGui.OpenPopup( "Rename" );
                    }

                    if( SelectedExcludeList != null ) { // Might be removed by previous
                        if( ImGui.BeginPopup( "Rename" ) ) {
                            ImGui.InputText( "##Name", ref SelectedExcludeList.Name, 255 );
                            ImGui.EndPopup();
                        }
                    }
                }
            }

            using var child = ImRaii.Child( "Child", new( Math.Max( ImGui.GetContentRegionAvail().X, 300 ), ImGui.GetFrameHeightWithSpacing() * 10 ), true );
            SelectedExcludeList?.Draw();
        }

        protected override void OnCancel() {
            Plugin.Configuration.Save();
        }

        protected override void OnOk() {
            Plugin.Configuration.Save();
            Command.AddAndExecute( new PapHavokCommand( Motion.File, () => {
                GltfAnimation.ImportAnimation(
                    Motion.File.Handles,
                    Motion.File.MotionData.Skeleton,
                    Motion,
                    HavokIndex,
                    AnimationIndex,
                    Compress,
                    SkipOriginal,
                    ( !Exclude || SelectedExcludeList == null ) ? [] : SelectedExcludeList.Bones.Select( x => x.BoneName ).Where( x => !string.IsNullOrEmpty( x ) ).ToList(),
                    ImportPath );
            } ) );
            Dalamud.OkNotification( "Havok data imported" );
        }
    }
}
