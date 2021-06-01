using Dalamud.Game.ClientState.Actors.Types;
using ImGuizmoNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Structs.Vfx {
    public abstract class BaseVfx {
        public Plugin Plugin;
        public IntPtr Vfx;
        public string Path;

        public BaseVfx( Plugin plugin, string path) {
            Plugin = plugin;
            Path = path;
        }

        public abstract void Remove();

        public static int GetId(IntPtr vfx, int offset) {
            if( vfx == IntPtr.Zero ) return 0;
            var addr = IntPtr.Add( vfx, offset );
            byte[] bytes = new byte[4];
            Marshal.Copy( addr, bytes, 0, 4 );
            return BitConverter.ToInt32( bytes, 0 );
        }

        // ======= MATRIX STUFF ===========
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

        public float[] matrix = GetIdentityMatrix();

        public void Update() {
            if( Vfx == IntPtr.Zero ) return;
            var flagAddr = IntPtr.Add( Vfx, 0x38 );
            byte currentFlag = Marshal.ReadByte( flagAddr );
            currentFlag |= 0x2;
            Marshal.WriteByte( flagAddr, currentFlag );
        }

        public static float[] GetIdentityMatrix() {
            return new float[] {
                1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f
            };
        }

        public void UpdatePosition( Vector3 position ) {
            if( Vfx == IntPtr.Zero ) return;
            IntPtr addr = IntPtr.Add( Vfx, 0x50 );
            var x = BitConverter.GetBytes( position.X );
            var y = BitConverter.GetBytes( position.Y );
            var z = BitConverter.GetBytes( position.Z );
            Marshal.Copy( x, 0, addr, 4 );
            Marshal.Copy( z, 0, addr + 0x4, 4 );
            Marshal.Copy( y, 0, addr + 0x8, 4 );
        }

        public void UpdatePosition( Actor actor ) {
            UpdatePosition( new Vector3( actor.Position.X, actor.Position.Y, actor.Position.Z ) );
        }

        public void UpdateScale( Vector3 scale ) {
            if( Vfx == IntPtr.Zero ) return;
            IntPtr addr = IntPtr.Add( Vfx, 0x70 );
            var x = BitConverter.GetBytes( scale.X );
            var y = BitConverter.GetBytes( scale.Y );
            var z = BitConverter.GetBytes( scale.Z );
            Marshal.Copy( x, 0, addr, 4 );
            Marshal.Copy( y, 0, addr + 0x4, 4 );
            Marshal.Copy( z, 0, addr + 0x8, 4 );
        }

        public void UpdateRotation( Vector3 rotation ) {
            if( Vfx == IntPtr.Zero ) return;

            Quaternion q = Quaternion.CreateFromYawPitchRoll( rotation.X, rotation.Y, rotation.Z );

            IntPtr addr = IntPtr.Add( Vfx, 0x60 );
            var x = BitConverter.GetBytes( q.X );
            var y = BitConverter.GetBytes( q.Y );
            var z = BitConverter.GetBytes( q.Z );
            var w = BitConverter.GetBytes( q.W );
            Marshal.Copy( x, 0, addr, 4 );
            Marshal.Copy( y, 0, addr + 0x4, 4 );
            Marshal.Copy( z, 0, addr + 0x8, 4 );
            Marshal.Copy( w, 0, addr + 0xc, 4 );
        }

        public void UpdateMatrix() {
            if( Vfx == IntPtr.Zero ) return;
            byte[] bytes = new byte[12];

            // POSITION
            IntPtr posAddr = IntPtr.Add( Vfx, 0x50 );
            Marshal.Copy( posAddr, bytes, 0, 4 );
            Marshal.Copy( posAddr + 0x4, bytes, 4, 4 );
            Marshal.Copy( posAddr + 0x8, bytes, 8, 4 );
            float[] position = { BitConverter.ToSingle( bytes, 0 ), BitConverter.ToSingle( bytes, 4 ), BitConverter.ToSingle( bytes, 8 ) };

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