using FFXIVClientStructs.Havok;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System.Collections.Generic;
using VfxEditor.SklbFormat.Bones;

namespace VfxEditor.Interop.Havok {
    public unsafe class HavokBones : HavokData {
        public Dictionary<string, Bone> BoneMatrixes = new();
        public List<Bone> BoneList = new();
        public hkaSkeleton* Skeleton { get; private set; }

        protected static int BONE_ID = 0;
        public readonly List<SklbBone> Bones = new();

        public HavokBones( string havokPath ) : base( havokPath ) { }

        protected override void OnLoad() {
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
            BoneMatrixes = new();
            BoneList = new();
            if( Resource == null || Skeleton == null ) return;

            var parents = new List<int>();
            var refPoses = new List<Matrix>();
            var bindPoses = new List<Matrix>();

            foreach( var bone in Bones ) {
                var pos = bone.Pos;
                var rot = bone.Rot;
                var scl = bone.Scl;

                var matrix = HavokUtils.CleanMatrix( Matrix.AffineTransformation(
                    scl.X,
                    new Quaternion( rot.X, rot.Y, rot.Z, rot.W ),
                    new Vector3( pos.X, pos.Y, pos.Z )
                ) );

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
