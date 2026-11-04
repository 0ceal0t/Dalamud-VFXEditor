using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.Graphics.Vfx;
using System.Numerics;

namespace VfxEditor.Interop.Structs.Vfx {
    public unsafe class StaticVfx : BaseVfx {
        public readonly VfxObject* Vfx;

        public StaticVfx( string path, Vector3 position, float rotation ) : base( path ) {
            Vfx = VfxObject.Create( path, "Client.System.Scheduler.Instance.VfxObject" );
            if( Vfx == null ) return;

            Plugin.ResourceLoader.StaticVfxRun( Vfx, 0.0f, 0xFFFFFFFF );

            UpdatePosition( position );
            UpdateRotation( rotation );
            Update();
        }

        public void UpdatePosition( Vector3 position ) {
            if( Vfx == null ) return;
            Vfx->Position = new Vector3 {
                X = position.X,
                Y = position.Y,
                Z = position.Z
            };
        }

        public void UpdateScale( Vector3 scale ) {
            if( Vfx == null ) return;
            Vfx->Scale = new Vector3 {
                X = scale.X,
                Y = scale.Y,
                Z = scale.Z
            };
        }

        public void UpdateRotation( float rotation ) {
            if( Vfx == null ) return;

            Vfx->Rotation = FFXIVClientStructs.FFXIV.Common.Math.Quaternion.CreateFromYawPitchRoll(
                rotation,
                0,
                0
            );
        }

        protected void Update() {
            if( Vfx == null ) return;
            Vfx->UpdateTransforms( true );
        }

        public override nint GetAddress() => ( nint )Vfx;

        public override void Remove() => Plugin.ResourceLoader.StaticVfxRemove( Vfx );
    }
}