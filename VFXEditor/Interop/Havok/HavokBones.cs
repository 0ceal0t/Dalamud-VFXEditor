using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Object;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System.Collections.Generic;
using VfxEditor.SklbFormat.Bones;

namespace VfxEditor.Interop.Havok {
    public unsafe class HavokBones : HavokData {
        public Dictionary<string, Bone> BoneMatrixes = [];
        public List<Bone> BoneList = [];
        public hkaSkeleton* Skeleton { get; private set; }

        private static int BONE_ID = 0;
        public static int NEW_BONE_ID => BONE_ID++;

        public readonly List<SklbBone> Bones = [];

        public HavokBones( string havokPath, bool init ) : base( havokPath, init ) { }

        protected override void OnHavokLoad() {
            Skeleton = AnimationContainer->Skeletons[0].ptr;

            for( var i = 0; i < Skeleton->Bones.Length; i++ ) {
                var bone = Skeleton->Bones[i];
                Bones.Add( new( Skeleton->Bones[i], Skeleton->ReferencePose[i], BONE_ID++ ) );
            }

            for( var i = 0; i < Skeleton->Bones.Length; i++ ) {
                var parentIdx = Skeleton->ParentIndices[i];
                if( parentIdx >= 0 ) Bones[i].Parent = Bones[parentIdx];
            }

            UpdateBones();
        }

        public virtual void UpdateBones() {
            BoneMatrixes = [];
            BoneList = [];
            if( Resource == null || Skeleton == null ) return;

            var parents = new List<int>();
            var refPoses = new List<Matrix>();
            var bindPoses = new List<Matrix>();

            foreach( var bone in Bones ) {
                var pos = bone.Pos;
                var rot = bone.Rot;
                var scl = bone.Scl;

                var matrix = Matrix.AffineTransformation(
                    scl.X,
                    new Quaternion( ( float )rot.X, ( float )rot.Y, ( float )rot.Z, ( float )rot.W ),
                    new Vector3( pos.X, pos.Y, pos.Z )
                );

                parents.Add( ParentIdx( bone ) );
                refPoses.Add( matrix );
                bindPoses.Add( Matrix.Identity );
            }

            for( var target = 0; target < Bones.Count; target++ ) {
                var current = target;
                while( current >= 0 ) {
                    bindPoses[target] = Matrix.Multiply( bindPoses[target], refPoses[current] );
                    current = parents[current];
                }
            }

            for( var i = 0; i < Bones.Count; i++ ) {
                var name = Bones[i].Name.Value;
                var bone = new Bone {
                    BindPose = bindPoses[i],
                    ParentIndex = parents[i],
                    Name = name
                };

                BoneList.Add( bone );
                BoneMatrixes[name] = bone;
            }
        }

        public override void RemoveReference() {
            base.RemoveReference();

            if( Resource == null ) return;

            var refSkeleton = ( hkReferencedObject* )Skeleton;
            refSkeleton->RemoveReference();
        }

        protected int ParentIdx( SklbBone bone ) => bone.Parent == null ? -1 : Bones.IndexOf( bone.Parent );
    }
}
