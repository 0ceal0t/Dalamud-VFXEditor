using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using HelixToolkit.SharpDX.Core.Animations;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VfxEditor.SklbFormat.Animation;

namespace VfxEditor.SklbFormat.Bones {
    public unsafe class HavokBones {
        public readonly string Path;
        public readonly hkResource* Resource;
        public readonly hkRootLevelContainer* Container;
        public readonly hkaSkeleton* Skeleton;

        public Dictionary<string, Bone> BoneMatrixes = new();
        public List<Bone> BoneList = new();

        protected static int BONE_ID = 0;
        protected readonly List<SklbBone> Bones = new();

        public HavokBones( string havokPath ) {
            Path = havokPath;

            try {
                var path = Marshal.StringToHGlobalAnsi( Path );

                var loadOptions = stackalloc hkSerializeUtil.LoadOptions[1];
                loadOptions->TypeInfoRegistry = hkBuiltinTypeRegistry.Instance()->GetTypeInfoRegistry();
                loadOptions->ClassNameRegistry = hkBuiltinTypeRegistry.Instance()->GetClassNameRegistry();
                loadOptions->Flags = new hkFlags<hkSerializeUtil.LoadOptionBits, int> {
                    Storage = ( int )hkSerializeUtil.LoadOptionBits.Default
                };

                Resource = hkSerializeUtil.LoadFromFile( ( byte* )path, null, loadOptions );

                if( Resource == null ) {
                    PluginLog.Error( $"Could not read file: {Path}" );
                }

                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    Container = ( hkRootLevelContainer* )Resource->GetContentsPointer( n1, hkBuiltinTypeRegistry.Instance()->GetTypeInfoRegistry() );
                    var animationName = @"hkaAnimationContainer"u8;
                    fixed( byte* n2 = animationName ) {
                        var anim = ( hkaAnimationContainer* )Container->findObjectByName( n2, null );
                        Skeleton = anim->Skeletons[0].ptr;
                        OnLoad();
                    }
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not read file: {Path}" );
            }
        }

        protected void OnLoad() {
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

                var matrix = AnimationData.CleanMatrix( Matrix.AffineTransformation(
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

        public virtual void RemoveReference() {
            if( Resource == null ) return;
            var refResource = ( hkReferencedObject* )Resource;
            refResource->RemoveReference();
        }

        protected int ParentIdx( SklbBone bone ) => bone.Parent == null ? -1 : Bones.IndexOf( bone.Parent );
    }
}
