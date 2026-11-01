using Dalamud.Game.ClientState.Objects.Types;
using System;

namespace VfxEditor.Interop.Structs.Vfx {
    public unsafe class ActorVfx : BaseVfx {
        public ActorVfx( IGameObject caster, IGameObject target, string path ) : this( caster.Address, target.Address, path ) { }

        public ActorVfx( IntPtr caster, IntPtr target, string path ) : base( path ) {
            Vfx = Plugin.ResourceLoader.ActorVfxCreate( path, caster, target, -1, ( char )0, 0, ( char )0 );
        }

        public override void Remove() {
            Plugin.ResourceLoader.ActorVfxRemove( Vfx, ( char )1 );
        }
    }
}