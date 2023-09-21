using System;
using System.Linq;
using VfxEditor.Interop;
using VfxEditor.Structs.Animation;

namespace VfxEditor.Spawn {
    public static unsafe class TmbSpawn {
        private static IntPtr ActorToReset = IntPtr.Zero;

        public static bool CanReset => !ActorToReset.Equals( IntPtr.Zero );

        public static void Apply( string path ) {
            var id = GetIdFromTmbPath( path );
            if( id > 0 ) Apply( id );
        }

        public static void Apply( uint animationId ) {
            if( animationId == 0 ) return;
            ApplyAnimationOverride( GetActor(), ( ushort )animationId, true );
        }

        public static void Reset() => ResetAnimationOverride( GetActor() );

        public static bool ApplyAnimationOverride( ActorMemoryStruct* memory, ushort animationId, bool interrupt ) {
            if( memory == null ) return false;
            if( !memory->CanAnimate ) return false;
            ActorToReset = new IntPtr( memory );
            ApplyBaseAnimationInternal( memory, animationId, interrupt, CharacterModes.AnimLock, 0 );
            return true;
        }

        public static void ResetAnimationOverride( ActorMemoryStruct* memory ) {
            if( memory == null ) return;
            ActorToReset = IntPtr.Zero;
            ApplyBaseAnimationInternal( memory, 0, true, CharacterModes.Normal, 0 );

            var animation = GetAnimation( memory );
            animation->LipsOverride = 0;

            // LinkSpeeds = true
            var speed = animation->Speeds[0];
            for( var i = 0; i < 13; i++ ) {
                animation->Speeds[i] = speed;
            }

            animation->Speeds![( int )AnimationSlots.FullBody] = 1.0f;
        }

        private static void ApplyBaseAnimationInternal( ActorMemoryStruct* memory, ushort animationId, bool interrupt, CharacterModes mode, byte modeInput ) {
            var animation = GetAnimation( memory );

            if( animation->BaseOverride != animationId ) {
                animation->BaseOverride = animationId;
            }

            if( memory->CharacterModeInput != modeInput ) {
                memory->CharacterModeInput = modeInput;
            }

            if( ( CharacterModes )memory->CharacterMode != mode ) {
                memory->CharacterMode = ( byte )mode;
            }

            if( interrupt ) {
                animation->AnimationIds[( int )AnimationSlots.FullBody] = 0;
            }
        }

        private static AnimationMemory* GetAnimation( ActorMemoryStruct* memory ) => ( AnimationMemory* )( new IntPtr( memory ) + Constants.AnimationMemoryOffset );

        private static ActorMemoryStruct* GetActor() => ( ActorMemoryStruct* )Plugin.PlayerObject?.Address;

        public static uint GetIdFromTmbPath( string path ) {
            if( string.IsNullOrEmpty( path ) ) return 0;
            var monster = path.Contains( "mon_sp" );
            var trimmed = path.Replace( ".tmb", "" );
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.ActionTimeline>()
                .Where( x => !string.IsNullOrEmpty( x.Key ) );

            foreach( var item in sheet ) {
                var key = item.Key.ToString();
                if( monster && key.Contains( "[SKL_ID]" ) ) {
                    var split = key.Split( "[SKL_ID]" )[1];
                    if( trimmed.EndsWith( split ) ) return item.RowId;
                }
                else {
                    if( trimmed.EndsWith( key ) ) return item.RowId;
                }
            }
            return 0;
        }

        public static void Dispose() {
            if( ActorToReset == IntPtr.Zero ) return;
            var actor = GetActor();
            if( actor == null ) return; // local player no longer exists
            if( new IntPtr( actor ).Equals( ActorToReset ) ) ResetAnimationOverride( actor );
        }
    }
}
