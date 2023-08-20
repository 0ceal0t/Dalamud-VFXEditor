using Dalamud.Interface;
using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.DirectX;
using VfxEditor.FileManager;
using VfxEditor.Interop.Structs.Animation;
using VfxEditor.SklbFormat.Animation;
using VfxEditor.SklbFormat.Mapping;
using VfxEditor.Utils;
using VfxEditor.Utils.Gltf;

namespace VfxEditor.SklbFormat.Bones {
    public unsafe class SklbBones : HavokBones {
        private readonly SklbFile File;
        private static BoneNamePreview SklbPreview => Plugin.DirectXManager.SklbPreview;

        private bool DrawOnce = false;
        private SklbBone Selected;
        private SklbBone DraggingBone;
        private string SearchText = "";

        public readonly List<SklbMapping> Mappings = new();
        public readonly SklbMappingDropdown MappingView;

        public SklbBones( SklbFile file, string loadPath ) : base( loadPath ) {
            File = file;
            MappingView = new( Mappings );
        }

        protected override void OnLoad() {
            base.OnLoad();

            var variants = Container->NamedVariants;
            for( var i = 0; i < variants.Length; i++ ) {
                var variant = variants[i];
                if( variant.ClassName.String == "hkaSkeletonMapper" ) {
                    var mapper = ( SkeletonMapper* )variant.Variant.ptr;
                    Mappings.Add( new( this, mapper ) );
                }
            }
        }

        // ========== DRAWING ============

        public void Draw() {
            if( SklbPreview.CurrentFile != File ) UpdatePreview();

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
            }

            ImGui.SameLine();
            if( ImGui.Checkbox( "Show Bone Names", ref Plugin.Configuration.ShowBoneNames ) ) Plugin.Configuration.Save();

            ImGui.Separator();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 4 ) ) ) {
                ImGui.Columns( 2, "Columns", true );

                using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                    // New bone
                    using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                        if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                            var newId = BONE_ID++;
                            var newBone = new SklbBone( newId );
                            newBone.Name.Value = $"bone_{newId}";
                            CommandManager.Sklb.Add( new GenericAddCommand<SklbBone>( Bones, newBone ) );
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
            var text = bone.Parent == null ? "[NONE]" : bone.Parent.Name.Value;

            using var combo = ImRaii.Combo( "Parent", text );
            if( !combo ) return;

            if( ImGui.Selectable( "[NONE]", bone.Parent == null ) ) {
                CommandManager.Sklb.Add( new SklbBoneParentCommand( bone, null ) );
            }

            var idx = 0;

            foreach( var item in Bones ) {
                if( item == bone ) continue;
                using var _ = ImRaii.PushId( idx );
                var selected = bone.Parent == item;

                if( ImGui.Selectable( item.Name.Value, selected ) ) {
                    CommandManager.Sklb.Add( new SklbBoneParentCommand( bone, item ) );
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
                    var newId = BONE_ID++;
                    var newBone = new SklbBone( newId );
                    newBone.Name.Value = $"bone_{newId}";
                    var command = new CompoundCommand( false, true );
                    command.Add( new GenericAddCommand<SklbBone>( Bones, newBone ) );
                    command.Add( new SklbBoneParentCommand( newBone, bone ) );
                    CommandManager.Sklb.Add( command );
                }

                if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                    DeleteBone( bone );
                    ImGui.CloseCurrentPopup();
                }

                bone.Name.Draw( CommandManager.Sklb, 128, "##Rename", ImGuiInputTextFlags.AutoSelectAll );
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

            var validBones = Bones.Where( x => x.Name.Value.ToLower().Contains( SearchText.ToLower() ) ).ToList();
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
                    PluginLog.Log( "Tried to put bone into itself" );
                }
                else {
                    CommandManager.Sklb.Add( new SklbBoneParentCommand( DraggingBone, destination ) );
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
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".hkx", "", "hkx", ( bool ok, string res ) => {
                if( ok ) System.IO.File.Copy( Path, res, true );
            } );
        }

        private void ExportGltf() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".gltf", "skeleton", "gltf", ( bool ok, string res ) => {
                if( !ok ) return;
                GltfSkeleton.ExportSkeleton( Bones, res );
            } );
        }

        private void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", "Skeleton{.hkx,.gltf},.*", ( bool ok, string res ) => {
                if( !ok ) return;
                if( res.Contains( ".hkx" ) ) {
                    var importHavok = new HavokBones( res );
                    var newBones = importHavok.Bones;
                    importHavok.RemoveReference();
                    CommandManager.Sklb.Add( new SklbBonesImportCommand( this, newBones ) );
                }
                else {
                    try {
                        var newBones = GltfSkeleton.ImportSkeleton( res, Bones );
                        CommandManager.Sklb.Add( new SklbBonesImportCommand( this, newBones ) );
                    }
                    catch( Exception e ) {
                        PluginLog.Error( e, "Could not import data" );
                    }
                }
            } );
        }

        // ======= UPDATING ==========

        public static hkArray<T> CreateArray<T>( hkArray<T> currentArray, List<T> data, out nint handle ) where T : unmanaged {
            var flags = currentArray.Flags | data.Count;

            var size = Marshal.SizeOf( typeof( T ) );
            var arr = Marshal.AllocHGlobal( size * data.Count + 1 );
            var _arr = ( T* )arr;

            for( var i = 0; i < data.Count; i++ ) {
                _arr[i] = data[i];
            }

            handle = arr;

            var ret = new hkArray<T>() {
                CapacityAndFlags = flags,
                Length = data.Count,
                Data = _arr
            };
            return ret;
        }

        public void Write() {
            var handles = new List<nint>();

            Mappings.ForEach( x => x.Write( handles ) );

            var bones = new List<hkaBone>();
            var poses = new List<hkQsTransformf>();
            var parents = new List<short>();

            foreach( var bone in Bones ) {
                var parent = ( short )( bone.Parent == null ? -1 : Bones.IndexOf( bone.Parent ) );
                parents.Add( parent );

                bone.ToHavok( out var hkBone, out var hkPose, out var handle );
                bones.Add( hkBone );
                poses.Add( hkPose );
                handles.Add( handle );
            }

            Skeleton->Bones = CreateArray( Skeleton->Bones, bones, out var boneHandle );
            handles.Add( boneHandle );

            Skeleton->ReferencePose = CreateArray( Skeleton->ReferencePose, poses, out var poseHandle );
            handles.Add( poseHandle );

            Skeleton->ParentIndices = CreateArray( Skeleton->ParentIndices, parents, out var parentHandle );
            handles.Add( parentHandle );

            WriteHavok();
            handles.ForEach( Marshal.FreeHGlobal );
        }

        private void WriteHavok() {
            try {
                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    var result = stackalloc hkResult[1];

                    var className = hkBuiltinTypeRegistry.Instance()->GetClassNameRegistry()->GetClassByName( n1 );

                    var path = Marshal.StringToHGlobalAnsi( Path );
                    var oStream = new hkOstream();
                    oStream.Ctor( ( byte* )path );

                    var saveOptions = new hkSerializeUtil.SaveOptions {
                        Flags = new hkFlags<hkSerializeUtil.SaveOptionBits, int> {
                            Storage = ( int )hkSerializeUtil.SaveOptionBits.Default
                        }
                    };

                    hkSerializeUtil.Save( result, Container, className, oStream.StreamWriter.ptr, saveOptions );

                    oStream.Dtor();
                    Marshal.FreeHGlobal( path );
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not export to: {Path}" );
            }
        }

        private void UpdatePreview() {
            if( BoneList?.Count == 0 ) SklbPreview.LoadEmpty( File );
            else SklbPreview.LoadSkeleton( File, BoneList, AnimationData.CreateSkeletonMesh( BoneList, Selected == null ? -1 : Bones.IndexOf( Selected ) ) );
        }

        public void Updated() {
            UpdateBones();
            if( File == SklbPreview.CurrentFile ) UpdatePreview();
        }

        private void DeleteBone( SklbBone bone ) {
            var toDelete = new List<SklbBone> {
                bone
            };
            PopulateChildren( bone, toDelete );

            if( toDelete.Contains( Selected ) ) ClearSelected();

            var command = new CompoundCommand( false, true );
            foreach( var item in toDelete ) {
                command.Add( new GenericRemoveCommand<SklbBone>( Bones, item ) );
            }
            CommandManager.Sklb.Add( command );
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

        public void Dispose() {
            if( SklbPreview.CurrentFile == File ) SklbPreview.ClearFile();
        }
    }
}
