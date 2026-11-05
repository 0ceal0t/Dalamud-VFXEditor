using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Vfx;
using System.Text;

namespace VfxEditor.Interop.Structs.Vfx {
    public unsafe class ActorVfx : BaseVfx {
        public readonly VfxData* Vfx;

        public ActorVfx( IGameObject caster, IGameObject target, string path ) : this( ( GameObject* )caster.Address, ( GameObject* )target.Address, path ) { }

        public ActorVfx( GameObject* caster, GameObject* target, string path ) : base( path ) {
            var pathBytes = Encoding.UTF8.GetBytes( path + '\0' );
            fixed( byte* pathPtr = pathBytes ) {
                Vfx = Plugin.ResourceLoader.ActorVfxCreate( pathPtr, caster, target, -1, ( char )0, 0, ( char )0 );
            }
        }

        public override nint GetAddress() => ( nint )Vfx;

        public override void Remove() {
            if( Vfx == null || Plugin.ResourceLoader == null ) return;
            Plugin.ResourceLoader.ActorVfxRemove( Vfx, ( char )1 );
        }
    }
}