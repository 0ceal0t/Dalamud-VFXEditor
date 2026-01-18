using Dalamud.Bindings.ImGui;
using HelixToolkit.Maths;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.DirectX.Model;
using BoneStruct = HelixToolkit.SharpDX.Animations.Bone;
using Vec2 = System.Numerics.Vector2;
using System;

namespace VfxEditor.DirectX.Bone {
    public class BoneNameInstance : ModelInstance {
        private bool IsSklb = false;
        private List<BoneStruct> BoneList;

        private static readonly ClosenessComparator Comparator = new();

        public BoneNameInstance() : base() { }

        public void SetSkeleton( bool isSklb, List<BoneStruct> boneList ) {
            IsSklb = isSklb;
            BoneList = boneList;
        }

        public void SetEmpty( bool isSklb ) {
            IsSklb = isSklb;
            BoneList = [];
        }

        public override void DrawInstanceTexture( Action? drawPopup ) {
            var viewProj = Matrix4x4.Multiply( ViewMatrix, ProjMatrix );
            var worldViewProj = LocalMatrix * viewProj;
            var drawList = ImGui.GetWindowDrawList();

            DrawImage( drawPopup );

            var boneScreenMap = new Dictionary<string, Vec2>();
            var boneDepthMap = new Dictionary<string, float>();

            var boneScreenList = new List<Vec2>();
            var boneDepthList = new List<float>();

            foreach( var bone in BoneList ) {
                var matrix = bone.BindPose * worldViewProj;

                var pos = Vector3Helper.TransformCoordinate( new( 0 ), matrix );
                var screenPos = LastMid + ( ( LastSize / 2f ) * new Vec2( pos.X, -1f * pos.Y ) );
                var depth = pos.Z;

                boneScreenMap[bone.Name] = screenPos;
                boneDepthMap[bone.Name] = depth;

                boneScreenList.Add( screenPos );
                boneDepthList.Add( depth );
            }

            // ===== CONNECTION LINES =======

            if( CurrentRenderId != -1 && IsSklb && Plugin.Configuration.SklbBoneDisplay != BoneDisplay.Connected ) {
                foreach( var bone in BoneList ) {
                    if( bone.ParentIndex == -1 ) continue;
                    if( !ValidDepth( boneDepthMap[bone.Name] ) || !ValidDepth( boneDepthList[bone.ParentIndex] ) ) continue;

                    var startPos = boneScreenMap[bone.Name];
                    var endPos = boneScreenList[bone.ParentIndex];

                    drawList.AddLine( startPos, endPos, ImGui.ColorConvertFloat4ToU32( Plugin.Configuration.SkeletonBoneLineColor ), 2f );
                }
            }

            // ===== NAMES =======

            if( Plugin.Configuration.ShowBoneNames ) {
                var groups = boneScreenMap.GroupBy( entry => entry.Value, entry => entry.Key, Comparator );
                foreach( var group in groups ) {
                    var idx = 0;
                    foreach( var item in group ) {
                        drawList.AddText( group.Key + new Vec2( 0, 12f * idx ), ImGui.ColorConvertFloat4ToU32( Plugin.Configuration.SkeletonBoneNameColor ), item );
                        idx++;
                    }
                }
            }
        }

        private static bool ValidDepth( float depth ) => depth > 0.5f && depth < 1f;

        private class ClosenessComparator : IEqualityComparer<Vec2> {
            public bool Equals( Vec2 x, Vec2 y ) => ( x - y ).Length() < 10f;
            public int GetHashCode( Vec2 obj ) => 0;
        }
    }
}
