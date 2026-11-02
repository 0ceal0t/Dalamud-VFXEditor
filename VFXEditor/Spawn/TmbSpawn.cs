using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;
using System.Linq;
using VfxEditor.Interop.Structs.Animation;

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

        public static bool ApplyAnimationOverride( Character* character, ushort animationId, bool interrupt ) {
            if( character == null ) return false;
            if( !CanAnimate( character ) ) return false;
            ActorToReset = new IntPtr( character );
            ApplyBaseAnimationInternal( character, animationId, interrupt, CharacterModes.AnimLock, 0 );
            return true;
        }

        public static void ResetAnimationOverride( Character* character ) {
            if( character == null ) return;
            ActorToReset = IntPtr.Zero;
            ApplyBaseAnimationInternal( character, 0, true, CharacterModes.Normal, 0 );

            character->Timeline.LipsOverride = 0;

            // LinkSpeeds = true
            var speeds = character->Timeline.TimelineSequencer.TimelineSpeeds;
            var speed = speeds[0];
            for( var i = 0; i < speeds.Length; i++ ) {
                speeds[i] = speed;
            }

            speeds[( int )AnimationSlots.FullBody] = 1.0f;
        }

        private static bool CanAnimate( Character* character ) => character->Mode == CharacterModes.Normal || character->Mode == CharacterModes.AnimLock;

        private static void ApplyBaseAnimationInternal( Character* character, ushort animationId, bool interrupt, CharacterModes mode, byte modeParam ) {
            if( character->Timeline.BaseOverride != animationId ) {
                character->Timeline.BaseOverride = animationId;
            }

            if( character->ModeParam != modeParam ) {
                character->ModeParam = modeParam;
            }

            if( character->Mode != mode ) {
                character->Mode = mode;
            }

            if( interrupt ) {
                character->Timeline.TimelineSequencer.TimelineIds[( int )AnimationSlots.FullBody] = 0;
            }
        }

        private static Character* GetActor() => ( Character* )Plugin.PlayerObject?.Address;

        public static uint GetIdFromTmbPath( string path ) {
            if( string.IsNullOrEmpty( path ) ) return 0;
            var monster = path.Contains( "mon_sp" );
            var trimmed = path.Replace( ".tmb", "" );
            var sheet = Dalamud.DataManager.GetExcelSheet<Lumina.Excel.Sheets.ActionTimeline>()
                .Where( x => !string.IsNullOrEmpty( x.Key.ExtractText() ) );

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