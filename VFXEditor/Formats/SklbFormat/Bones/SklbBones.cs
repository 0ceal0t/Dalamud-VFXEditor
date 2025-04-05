using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using FFXIVClientStructs.Havok.Common.Base.Object;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.DirectX;
using VfxEditor.FileBrowser;
using VfxEditor.Formats.SklbFormat.Bones;
using VfxEditor.Interop.Havok;
using VfxEditor.Interop.Havok.SkeletonBuilder;
using VfxEditor.Interop.Structs.Animation;
using VfxEditor.SklbFormat.Mapping;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;
using static FFXIVClientStructs.Havok.Common.Serialize.Util.hkRootLevelContainer;

namespace VfxEditor.SklbFormat.Bones {
    public enum BoneDisplay {
        Connected,
        Blender_Style_Inline,
        Blender_Style_Perpendicular
    }

    public unsafe class SklbBones : HavokBones {
        private static readonly BoneDisplay[] BoneDisplayOptions = Enum.GetValues<BoneDisplay>();

        public readonly int RenderId = Renderer.NewId;
        private readonly SklbFile File;
        private static BoneNamePreview SklbPreview => Plugin.DirectXManager.SklbPreview;

        private bool DrawOnce = false;
        private SklbBone Selected;
        private SklbBone DraggingBone;
        private string SearchText = "";

        public readonly List<SklbMapping> Mappings = [];
        public readonly SklbMappingDropdown MappingView;
        public readonly SklbBoneListView ListView;

        public SklbBones( SklbFile file, string loadPath, bool init ) : base( loadPath, init ) {
            File = file;
            MappingView = new( File, Mappings );
            ListView = new( Bones );
        }

        protected override void OnHavokLoad() {
            base.OnHavokLoad();

            var variants = Container->NamedVariants;
            for( var i = 0; i < variants.Length; i++ ) {
                var variant = variants[i];
                if( variant.ClassName.String == "hkaSkeletonMapper" ) {
                    var mapper = ( SkeletonMapper* )variant.Variant.ptr;
                    // Mapper->SkeletonB is the same as HavokBones->Skeleton
                    Mappings.Add( new( this, mapper, variant.Name.String ) );
                }
            }
        }

        public void Write( HashSet<nint> handles ) {
            var bones = new List<hkaBone>();
            var poses = new List<hkQsTransformf>();
            var parents = new List<short>();

            foreach( var bone in Bones ) {
                var parent = ( short )( bone.Parent == null ? -1 : Bones.IndexOf( bone.Parent ) );
                parents.Add( parent );

                bone.ToHavok( handles, out var hkBone, out var hkPose );
                bones.Add( hkBone );
                poses.Add( hkPose );
            }

            Skeleton->Bones = CreateArray( handles, Skeleton->Bones, bones );
            Skeleton->ReferencePose = CreateArray( handles, Skeleton->ReferencePose, poses );
            Skeleton->ParentIndices = CreateArray( handles, Skeleton->ParentIndices, parents );

            var variants = new List<NamedVariant>();

            // Set up animation container variant
            var animClassName = Marshal.StringToHGlobalAnsi( "hkaAnimationContainer" );
            var aninName = Marshal.StringToHGlobalAnsi( "hkaAnimationContainer" );
            handles.Add( animClassName );
            handles.Add( aninName );

            variants.Add( new NamedVariant() {
                ClassName = new() {
                    StringAndFlag = ( byte* )animClassName
                },
                Name = new() {
                    StringAndFlag = ( byte* )aninName
                },
                Variant = new() {
                    ptr = ( hkReferencedObject* )AnimationContainer
                }
            } );

            // Set up mapping variants
            foreach( var (mapping, idx) in Mappings.WithIndex() ) {
                mapping.Write( handles );

                var mappingClassName = Marshal.StringToHGlobalAnsi( "hkaSkeletonMapper" );
                var mappingName = Marshal.StringToHGlobalAnsi( mapping.Name.Value );
                handles.Add( mappingClassName );
                handles.Add( mappingName );

                variants.Add( new NamedVariant() {
                    ClassName = new() {
                        StringAndFlag = ( byte* )mappingClassName
                    },
                    Name = new() {
                        StringAndFlag = ( byte* )mappingName
                    },
                    Variant = new() {
                        ptr = ( hkReferencedObject* )mapping.Mapper
                    }
                } );
            }

            // Update variants
            Container->NamedVariants = CreateArray( handles, Container->NamedVariants, variants );

            WriteHavok();
        }

        // ========== DRAWING ============

        public void Draw() {
            if( SklbPreview.CurrentRenderId != RenderId ) UpdatePreview();

            var expandAll = false;
            var searchSet = GetSearchSet();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.Button( "Export" ) ) ImGui.OpenPopup( "ExportPopup" );

                using( var popup = ImRaii.Popup( "ExportPopup" ) ) {
                    if( popup ) {
                        if( ImGui.Selectable( "GLTF" ) ) ExportGltf();
                        if( ImGui.Selectable( "HKX" ) ) ExportHavok();
                    }
                }

                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) ImportDialog();

                ImGui.SameLine();
                UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Blender-to-Edit-Skeletons-and-Animations" );
            }

            ImGui.SameLine();
            if( ImGui.Checkbox( "Show Bone Names", ref Plugin.Configuration.ShowBoneNames ) ) Plugin.Configuration.Save();

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 200f );
            if( UiUtils.EnumComboBox( "##BoneDisplay", BoneDisplayOptions, Plugin.Configuration.SklbBoneDisplay, out var newBoneDisplay ) ) {
                Plugin.Configuration.SklbBoneDisplay = newBoneDisplay;
                Plugin.Configuration.Save();
                UpdatePreview();
            }

            ImGui.Separator();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 4 ) ) ) {
                ImGui.Columns( 2, "Columns", true );

                using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                    // New bone
                    using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                        if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                            var newId = NEW_BONE_ID;
                            var newBone = new SklbBone( newId );
                            newBone.Name.Value = $"bone_{newId}";
                            CommandManager.Add( new ListAddCommand<SklbBone>( Bones, newBone ) );
                        }
                    }
                    UiUtils.Tooltip( "Create new bone at root" );

                    // Expand
                    ImGui.SameLine();
                    using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                        if( ImGui.Button( FontAwesomeIcon.Expand.ToIconString() ) ) {
                            expandAll = true;
                        }
                    }
                    UiUtils.Tooltip( "Expand all tree nodes" );

                    // Search
                    ImGui.SameLine();
                    ImGui.InputTextWithHint( "##Search", "Search", ref SearchText, 255 );
                }

                using var left = ImRaii.Child( "Left" );
                style.Pop();

                using var indent = ImRaii.PushStyle( ImGuiStyleVar.IndentSpacing, 9 );

                // Draw left column
                Bones.Where( x => x.Parent == null ).ToList().ForEach( x => DrawTree( x, searchSet, expandAll ) );

                // Drag-drop to root
                using var rootStyle = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) );
                rootStyle.Push( ImGuiStyleVar.FramePadding, new Vector2( 0 ) );

                ImGui.BeginChild( "EndChild", new Vector2( ImGui.GetContentRegionAvail().X, 1 ), false );
                ImGui.EndChild();

                using var dragDrop = ImRaii.DragDropTarget();
                if( dragDrop ) StopDragging( null );
            }

            if( !DrawOnce ) {
                ImGui.SetColumnWidth( 0, 200 );
                DrawOnce = true;
            }
            ImGui.NextColumn();

            using( var right = ImRaii.Child( "Right" ) ) {
                // Draw right column
                if( Selected != null ) {
                    using var font = ImRaii.PushFont( UiBuilder.IconFont );
                    if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), new( 0.7f, 0.7f, 0.7f, 1 ) ) ) {
                        ClearSelected();
                        UpdatePreview();
                    }
                }

                if( Selected != null ) {
                    DrawParentCombo( Selected );
                    Selected.DrawBody( Bones.IndexOf( Selected ) );

                    using var color = ImRaii.PushColor( ImGuiCol.Button, UiUtils.RED_COLOR );
                    if( UiUtils.IconButton( FontAwesomeIcon.Trash, "Delete" ) ) DeleteBone( Selected );
                }

                SklbPreview.DrawInline();
            }

            ImGui.Columns( 1 );
        }

        private void DrawParentCombo( SklbBone bone ) {
            using var combo = ImRaii.Combo( "Parent", bone.Parent == null ? "[NONE]" : bone.Parent.Name.Value );
            if( !combo ) return;

            if( ImGui.Selectable( "[NONE]", bone.Parent == null ) ) {
                CommandManager.Add( new SklbBoneParentCommand( bone, null ) );
            }

            var idx = 0;

            foreach( var item in Bones ) {
                if( item == bone ) continue;
                using var _ = ImRaii.PushId( idx );
                var selected = bone.Parent == item;

                if( ImGui.Selectable( item.Name.Value, selected ) ) {
                    CommandManager.Add( new SklbBoneParentCommand( bone, item ) );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
                idx++;
            }
        }

        private void DrawTree( SklbBone bone, HashSet<SklbBone> searchSet, bool expandAll ) {
            if( searchSet != null && !searchSet.Contains( bone ) ) return;

            var children = Bones.Where( x => x.Parent == bone ).ToList();
            var isLeaf = children.Count == 0;

            var flags =
                ImGuiTreeNodeFlags.DefaultOpen |
                ImGuiTreeNodeFlags.OpenOnArrow |
                ImGuiTreeNodeFlags.OpenOnDoubleClick |
                ImGuiTreeNodeFlags.SpanFullWidth;

            if( isLeaf ) flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            if( Selected == bone ) flags |= ImGuiTreeNodeFlags.Selected;

            if( expandAll ) ImGui.SetNextItemOpen( true );
            var nodeOpen = ImGui.TreeNodeEx( $"{bone.Name.Value}##{bone.Id}", flags );

            DragDrop( bone );

            if( ImGui.BeginPopupContextItem() ) {
                if( UiUtils.IconSelectable( FontAwesomeIcon.Plus, "Create sub-bone" ) ) {
                    var newId = NEW_BONE_ID;
                    var newBone = new SklbBone( newId );
                    newBone.Name.Value = $"bone_{newId}";
                    var commands = new List<ICommand> {
                        new ListAddCommand<SklbBone>( Bones, newBone ),
                        new SklbBoneParentCommand( newBone, bone )
                    };
                    CommandManager.Add( new CompoundCommand( commands ) );
                }

                if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                    DeleteBone( bone );
                    ImGui.CloseCurrentPopup();
                }

                bone.Name.Draw( 128, "##Rename", 0, ImGuiInputTextFlags.AutoSelectAll );
                ImGui.EndPopup();
            }

            if( ImGui.IsItemClicked( ImGuiMouseButton.Left ) && !ImGui.IsItemToggledOpen() ) {
                Selected = bone;
                UpdatePreview();
            }

            if( !isLeaf && nodeOpen ) {
                children.ForEach( x => DrawTree( x, searchSet, expandAll ) );
                ImGui.TreePop();
            }
        }

        private HashSet<SklbBone> GetSearchSet() {
            if( string.IsNullOrEmpty( SearchText ) ) return null;
            var searchSet = new HashSet<SklbBone>();

            var validBones = Bones.Where( x => x.Name.Value.Contains( SearchText, StringComparison.CurrentCultureIgnoreCase ) ).ToList();
            validBones.ForEach( x => PopulateSearchSet( searchSet, x ) );

            return searchSet;
        }

        private static void PopulateSearchSet( HashSet<SklbBone> searchSet, SklbBone bone ) {
            searchSet.Add( bone );
            if( bone.Parent != null ) PopulateSearchSet( searchSet, bone.Parent );
        }

        // ======= DRAGGING ==========

        private void DragDrop( SklbBone bone ) {
            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                StartDragging( bone );
                ImGui.Text( bone.Name.Value );
                ImGui.EndDragDropSource();
            }

            if( ImGui.BeginDragDropTarget() ) {
                StopDragging( bone );
                ImGui.EndDragDropTarget();
            }
        }

        private void StartDragging( SklbBone bone ) {
            ImGui.SetDragDropPayload( "SKLB_BONES", IntPtr.Zero, 0 );
            DraggingBone = bone;
        }

        public unsafe bool StopDragging( SklbBone destination ) {
            if( DraggingBone == null ) return false;
            var payload = ImGui.AcceptDragDropPayload( "SKLB_BONES" );
            if( payload.NativePtr == null ) return false;

            if( DraggingBone != destination ) {
                if( destination != null && destination.IsChildOf( DraggingBone ) ) {
                    Dalamud.Log( "Tried to put bone into itself" );
                }
                else {
                    CommandManager.Add( new SklbBoneParentCommand( DraggingBone, destination ) );
                }
            }

            DraggingBone = null;
            return true;
        }

        public void ClearSelected() {
            Selected = null;
        }

        // ======= IMPORT EXPORT ==========

        private void ExportHavok() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".hkx", "", "hkx", ( bool ok, string res ) => {
                if( ok ) System.IO.File.Copy( Path, res, true );
            } );
        }

        private void ExportGltf() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".gltf", "skeleton", "gltf", ( bool ok, string res ) => {
                if( ok ) GltfSkeleton.ExportSkeleton( Bones, res );
            } );
        }

        private void ImportDialog() {
            FileBrowserManager.OpenFileDialog( "Select a File", "Skeleton{.hkx,.gltf,.glb},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                if( res.Contains( ".hkx" ) ) {
                    var importHavok = new HavokBones( res, true );
                    var newBones = importHavok.Bones;
                    importHavok.RemoveReference();
                    CommandManager.Add( new SklbBonesImportCommand( this, newBones ) );
                }
                else {
                    try {
                        var newBones = GltfSkeleton.ImportSkeleton( res, Bones );
                        CommandManager.Add( new SklbBonesImportCommand( this, newBones ) );
                    }
                    catch( Exception e ) {
                        Dalamud.Error( e, "Could not import data" );
                    }
                }
            } );
        }

        // ======= UPDATING ==========

        private void UpdatePreview() {
            if( BoneList?.Count == 0 ) {
                SklbPreview.LoadEmpty( RenderId, File );
            }
            else {
                var selectedIdx = Selected == null ? -1 : Bones.IndexOf( Selected );
                SkeletonMeshBuilder builder = Plugin.Configuration.SklbBoneDisplay switch {
                    BoneDisplay.Connected => new ConnectedSkeletonMeshBuilder( BoneList, selectedIdx ),
                    BoneDisplay.Blender_Style_Perpendicular => new DisconnectedSkeletonMeshBuilder( BoneList, selectedIdx, true ),
                    BoneDisplay.Blender_Style_Inline => new DisconnectedSkeletonMeshBuilder( BoneList, selectedIdx, false ),
                    _ => null
                };

                SklbPreview.LoadSkeleton( RenderId, File, BoneList, builder.Build() );
            }
        }

        public void Updated() {
            UpdateBones();
            if( SklbPreview.CurrentRenderId == RenderId ) UpdatePreview();
        }

        private void DeleteBone( SklbBone bone ) {
            var toDelete = new List<SklbBone> {
                bone
            };
            PopulateChildren( bone, toDelete );

            if( toDelete.Contains( Selected ) ) ClearSelected();

            var commands = new List<ICommand>();
            foreach( var item in toDelete ) {
                commands.Add( new ListRemoveCommand<SklbBone>( Bones, item ) );
            }
            CommandManager.Add( new CompoundCommand( commands ) );
        }

        public void PopulateChildren( SklbBone parent, List<SklbBone> children ) {
            foreach( var bone in Bones ) {
                if( bone.Parent == parent ) {
                    if( children.Contains( bone ) ) continue;
                    children.Add( bone );
                    PopulateChildren( bone, children );
                }
            }
        }
    }
}
