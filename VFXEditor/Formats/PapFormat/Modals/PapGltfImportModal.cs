using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Data;
using VfxEditor.Formats.PapFormat.Commands;
using VfxEditor.Formats.PapFormat.Motion;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.Formats.PapFormat.Modals {
    public unsafe class PapGltfImportModal : Modal {
        private readonly PapMotion Motion;
        private readonly int HavokIndex;
        private readonly string ImportPath;

        private bool Compress = false;
        private bool SkipOriginal = false;
        private int AnimationIndex = 0;
        private readonly List<string> AnimationNames = [];
        private readonly ModelRoot Model;
        private readonly string LoadError;
        private readonly List<string> BoneNames = [];

        private bool Exclude = false;
        private ExcludedBonesConfiguration SelectedExcludeList;

        public PapGltfImportModal( PapMotion motion, int index, string importPath ) : base( "Animation Import", true ) {
            Motion = motion;
            HavokIndex = index;
            ImportPath = importPath;

            for( var i = 0; i < motion.Skeleton->Bones.Length; i++ ) {
                BoneNames.Add( motion.Skeleton->Bones[i].Name.String );
            }

            try {
                Model = ModelRoot.Load( importPath );
                foreach( var animation in Model.LogicalAnimations ) {
                    AnimationNames.Add( animation.Name );
                }
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Failed to load {importPath}" );
                LoadError = e.Message;
            }
        }

        private List<string> GetExcludeBones() =>
            ( !Exclude || SelectedExcludeList == null ) ? [] : [.. SelectedExcludeList.Bones.Select( x => x.BoneName ).Where( x => !string.IsNullOrEmpty( x ) )];

        protected override void DrawBody() {
            if( UiUtils.IconButton( FontAwesomeIcon.InfoCircle, "Wiki" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Blender-to-Edit-Skeletons-and-Animations" );

            if( Model == null ) {
                ImGui.TextColored( UiUtils.RED_COLOR, $"Could not load file: {LoadError}" );
                return;
            }

            if( AnimationNames.Count == 0 ) {
                ImGui.TextColored( UiUtils.RED_COLOR, "File has no animations" );
                return;
            }

            using( var nodes = ImRaii.TreeNode( "Nodes Being Imported" ) ) {
                if( nodes ) {
                    var tracks = new List<string>();
                    var skipped = new List<string>();
                    GltfAnimation.GetImportTracks(
                        Model.LogicalAnimations[AnimationIndex], BoneNames,
                        GltfAnimation.GetSkipBones( Motion, BoneNames, SkipOriginal, GetExcludeBones() ), tracks, skipped );

                    using var child = ImRaii.Child( "Child", new( ImGui.GetContentRegionAvail().X, 300 ) );
                    using var _ = ImRaii.PushIndent();
                    foreach( var nodeName in tracks ) ImGui.Text( nodeName );
                    using var color = ImRaii.PushColor( ImGuiCol.Text, ImGui.GetColorU32( ImGuiCol.TextDisabled ) );
                    foreach( var nodeName in skipped ) ImGui.Text( $"{nodeName} (skipped)" );
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
            if( Model == null || AnimationNames.Count == 0 ) return;

            GltfImportResult result = null;
            var command = new PapHavokCommand( Motion.File, () => {
                result = GltfAnimation.ImportAnimation(
                    Motion.File.Handles,
                    Motion.File.MotionData.Skeleton,
                    Motion,
                    HavokIndex,
                    AnimationIndex,
                    Compress,
                    SkipOriginal,
                    GetExcludeBones(),
                    ImportPath );
            } );

            if( result == null || !result.Success ) {
                // Nothing was changed, so don't add an empty command to the undo stack
                Dalamud.ErrorNotification( $"Havok import failed: {result?.Error ?? "unknown error"}" );
                return;
            }

            Command.AddAndExecute( command );

            var total = result.Bound.Count + result.Skipped.Count;
            if( result.Skipped.Count > 0 ) {
                var list = string.Join( ", ", result.Skipped.Take( 10 ) ) + ( result.Skipped.Count > 10 ? ", ..." : "" );
                Dalamud.WarningNotification( $"Havok data imported: bound {result.Bound.Count} of {total} bones. Skipped: {list}" );
            }
            else {
                Dalamud.OkNotification( $"Havok data imported: bound {result.Bound.Count} of {total} bones" );
            }
        }
    }
}