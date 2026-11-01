using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System.Numerics;

namespace VfxEditor.Interop.Structs.Vfx {
    public abstract unsafe class BaseVfx {
        public VfxObject* Vfx;
        public string Path;

        public BaseVfx( string path ) {
            Path = path;
        }

        public abstract void Remove();

        public void UpdatePosition( Vector3 position ) {
            if( Vfx == null ) return;
            Vfx->Position = new Vector3 {
                X = position.X,
                Y = position.Y,
                Z = position.Z
            };
        }

        public void UpdatePosition( IGameObject actor ) {
            if( Vfx == null ) return;
            Vfx->Position = actor.Position;
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
    }
}