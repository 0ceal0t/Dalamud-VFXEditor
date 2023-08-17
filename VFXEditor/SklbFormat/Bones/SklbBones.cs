using Dalamud.Interface;
using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.DirectX;
using VfxEditor.FileManager;
using VfxEditor.SklbFormat.Animation;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat.Bones {
    public unsafe class SklbBones : HavokBones {
        private readonly SklbFile File;
        private static BoneNamePreview SklbPreview => Plugin.DirectXManager.SklbPreview;

        private bool DrawOnce = false;
        private SklbBone Selected;
        private SklbBone DraggingBone;

        public SklbBones( SklbFile file, string loadPath ) : base( loadPath ) {
            File = file;
        }

        public void Update() {
            var nameHandles = new List<nint>();

            var bones = new List<hkaBone>();
            var poses = new List<hkQsTransformf>();
            var parents = new List<short>();

            foreach( var bone in Bones ) {
                var parent = ( short )( bone.Parent == null ? -1 : Bones.IndexOf( bone.Parent ) );
                parents.Add( parent );

                bone.ToHavok( out var hkBone, out var hkPose, out var handle );
                bones.Add( hkBone );
                poses.Add( hkPose );
                nameHandles.Add( handle );
            }

            Skeleton->Bones.Length = bones.Count;
            Skeleton->Bones.CapacityAndFlags = Skeleton->Bones.Flags | bones.Count;

            Skeleton->ReferencePose.Length = poses.Count;
            Skeleton->ReferencePose.CapacityAndFlags = Skeleton->ReferencePose.Flags | poses.Count;

            Skeleton->ParentIndices.Length = parents.Count;
            Skeleton->ParentIndices.CapacityAndFlags = Skeleton->ParentIndices.Flags | parents.Count;

            var _bones = bones.ToArray();
            var _poses = poses.ToArray();
            var _parents = parents.ToArray();

            fixed( hkaBone* bonePtr = _bones ) {
                Skeleton->Bones.Data = bonePtr;

                fixed( hkQsTransformf* posePtr = _poses ) {
                    Skeleton->ReferencePose.Data = posePtr;

                    fixed( short* parentPtr = _parents ) {
                        Skeleton->ParentIndices.Data = parentPtr;

                        Write();
                        nameHandles.ForEach( Marshal.FreeHGlobal );
                    }
                }
            }
        }

        private void Write() {
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
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not export to: {Path}" );
            }
        }

        public void Draw() {
            if( SklbPreview.CurrentFile != File ) UpdatePreview();

            using var _ = ImRaii.PushId( "Bones " );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0, 4 ) ) ) {
                ImGui.Columns( 2, "Columns", true );

                // TODO: search
                // TODO: new
                // TODO: expand all

                using var left = ImRaii.Child( "Left" );
                style.Pop();

                using var indent = ImRaii.PushStyle( ImGuiStyleVar.IndentSpacing, 9 );

                // Draw left column
                Bones.Where( x => x.Parent == null ).ToList().ForEach( x => DrawTree( x, false ) );

                // TODO: null dragging
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
                    if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), UiUtils.RED_COLOR ) ) {
                        Selected = null;
                        UpdatePreview();
                    }
                }

                if( Selected != null ) {
                    DrawParentCombo( Selected );
                    Selected.DrawBody();
                    if( UiUtils.RemoveButton( "Delete", small: true ) ) Delete( Selected );

                    ImGui.Separator();
                }

                if( ImGui.Checkbox( "Bone Names", ref Plugin.Configuration.ShowBoneNames ) ) Plugin.Configuration.Save();
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

        private void DrawTree( SklbBone bone, bool blockPopup ) {
            var children = Bones.Where( x => x.Parent == bone ).ToList();
            var isLeaf = children.Count == 0;

            var flags =
                ImGuiTreeNodeFlags.DefaultOpen |
                ImGuiTreeNodeFlags.OpenOnArrow |
                ImGuiTreeNodeFlags.OpenOnDoubleClick |
                ImGuiTreeNodeFlags.SpanFullWidth;

            if( isLeaf ) flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
            if( Selected == bone ) flags |= ImGuiTreeNodeFlags.Selected;

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
                    Delete( bone );
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
                children.ForEach( x => DrawTree( x, blockPopup ) );
                ImGui.TreePop();
            }
        }

        private void Delete( SklbBone bone ) {
            var toDelete = new List<SklbBone> {
                bone
            };
            PopulateChildren( bone, toDelete );

            if( toDelete.Contains( Selected ) ) Selected = null;

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

        private void UpdatePreview() {
            if( BoneList?.Count == 0 ) SklbPreview.LoadEmpty( File );
            else SklbPreview.LoadSkeleton( File, BoneList, AnimationData.CreateSkeletonMesh( BoneList, Selected == null ? -1 : Bones.IndexOf( Selected ) ) );
        }

        public void Updated() {
            UpdateBones();
            if( File == SklbPreview.CurrentFile ) UpdatePreview();
        }

        public void Dispose() {
            if( SklbPreview.CurrentFile == File ) SklbPreview.ClearFile();
        }
    }
}
