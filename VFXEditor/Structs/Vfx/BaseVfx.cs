using Dalamud.Game.ClientState.Actors;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;
using ImGuizmoNET;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VFXEditor.Structs.Vfx {

    /*
        *(undefined4 *)(vfx + 0x50) = DAT_01bb2850;
        *(undefined4 *)(vfx + 0x54) = DAT_01bb2854;
        *(undefined4 *)(vfx + 0x58) = DAT_01bb2858;
        uVar3 = uRam0000000001bb286c;
        uVar2 = uRam0000000001bb2868;
        uVar5 = uRam0000000001bb2864;
        *(undefined4 *)(vfx + 0x60) = _ZERO_VECTOR;
        *(undefined4 *)(vfx + 100) = uVar5;
        *(undefined4 *)(vfx + 0x68) = uVar2;
        *(undefined4 *)(vfx + 0x6c) = uVar3;
        *(undefined4 *)(vfx + 0x70) = DAT_01bb2870;
        *(undefined4 *)(vfx + 0x74) = DAT_01bb2874;
        uVar5 = DAT_01bb2878;
        *(undefined4 *)(vfx + 0x78) = DAT_01bb2878;
        *(ulonglong *)(vfx + 0x38) = *(ulonglong *)(vfx + 0x38) | 2;
     */


    [StructLayout( LayoutKind.Explicit, Size = 0x200 )] // idk what the size is lol
    public unsafe struct VfxStruct {
        [FieldOffset( 0x38 )] public byte Flags;
        [FieldOffset( 0x50 )] public Position3 Position;
        [FieldOffset( 0x60 )] public Quat Rotation;
        [FieldOffset( 0x70 )] public Position3 Scale;

        [FieldOffset( 0x128 )] public int ActorCaster;
        [FieldOffset( 0x130 )] public int ActorTarget;

        [FieldOffset( 0x1B8 )] public int StaticCaster;
        [FieldOffset( 0x1C0 )] public int StaticTarget;
    }

    public unsafe abstract class BaseVfx {
        public Plugin Plugin;
        public VfxStruct* Vfx;
        public string Path;
        public float[] matrix = GetIdentityMatrix();

        public BaseVfx( Plugin plugin, string path) {
            Plugin = plugin;
            Path = path;
        }

        public abstract void Remove();

        public void Update() {
            if( Vfx == null ) return;
            Vfx->Flags |= 0x2;
        }

        public void UpdatePosition( Vector3 position ) {
            if( Vfx == null ) return;
            Vfx->Position = new Position3
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z
            };
        }

        public void UpdatePosition( Actor actor ) {
            if( Vfx == null ) return;
            Vfx->Position = actor.Position;
        }

        public void UpdateScale( Vector3 scale ) {
            if( Vfx == null ) return;
            Vfx->Scale = new Position3
            {
                X = scale.X,
                Y = scale.Y,
                Z = scale.Z
            };
        }

        public void UpdateRotation( Vector3 rotation ) {
            if( Vfx == null ) return;

            Quaternion q = Quaternion.CreateFromYawPitchRoll( rotation.X, rotation.Y, rotation.Z );
            Vfx->Rotation = new Quat
            {
                X = q.X,
                Y = q.Y,
                Z = q.Z,
                W = q.W
            };
        }

        // ===========================

        public static float[] GetIdentityMatrix() {
            return new float[] {
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f
            };
        }

        public void UpdateMatrix() {
            if( Vfx == null ) return;

            float[] position = { Vfx->Position.X, Vfx->Position.Y, Vfx->Position.Z };
            float[] scale = { 1, 1, 1 }; // don't really care about these here
            float[] rotation = { 0, 0, 0 };

            ImGuizmo.RecomposeMatrixFromComponents( ref position[0], ref rotation[0], ref scale[0], ref matrix[0] );
        }

        public void PullMatrixUpdate() {
            float[] position = new float[3];
            float[] rotation = new float[3];
            float[] scale = new float[3];

            ImGuizmo.DecomposeMatrixToComponents( ref matrix[0], ref position[0], ref rotation[0], ref scale[0] );

            UpdatePosition( new Vector3( position[0], position[2], position[1] ) );
            UpdateScale( new Vector3( scale[0], scale[1], scale[2] ) );
            UpdateRotation( new Vector3( rotation[1], rotation[0], rotation[2] ) );
            Update();
        }
    }
}