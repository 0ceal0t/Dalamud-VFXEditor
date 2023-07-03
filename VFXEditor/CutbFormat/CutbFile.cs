using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.CutbFormat.Actor;
using VfxEditor.CutbFormat.Animation;
using VfxEditor.CutbFormat.CB;
using VfxEditor.CutbFormat.EX;
using VfxEditor.CutbFormat.Resource;
using VfxEditor.CutbFormat.Scene;
using VfxEditor.CutbFormat.State;
using VfxEditor.CutbFormat.Timeline;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;
using VfxEditor.Utils;

namespace VfxEditor.CutbFormat {
    public class CutbFile : FileManagerFile {
        public readonly CutbResourceList ResourceList;
        public readonly CutbInitialState InitialState;
        public readonly CutbDefaultScene DefaultScene;
        public readonly List<CutbEX> EXs = new();
        public readonly CutbActorList ActorList;
        public readonly CutbCB CB;
        public readonly CutbAnimation Animation;
        public readonly List<CutbTimeline> Timelines = new();

        private readonly SimpleDropdown<CutbTimeline> TimelineDropdown;

        public CutbFile( BinaryReader reader, string hkxTemp, bool checkOriginal = true ) : base( new CommandManager( Plugin.CutbManager ) ) {
            var original = checkOriginal ? FileUtils.GetOriginal( reader ) : null;

            reader.ReadInt32(); // CUTB
            reader.ReadInt32(); // size
            var headercount = reader.ReadInt32();

            // Actor List
            // CB
            // Animation
            // Timeline [0+]

            /*
             * TODO: how does this interact with the cutscene sheets like
             * CutsceneActorSize_0
             * CutsceneEventMotion_0
             * CutActionTimeline_0
             * TalkSubtitle
             */

            for( var i = 0; i < headercount; i++ ) {
                var magic = FileUtils.ReadString( reader, 4 );
                var headerSize = reader.ReadInt32();
                var startPosition = reader.BaseStream.Position; // startPosition + offset is the data
                var bodyOffset = reader.ReadInt32();
                var bodySize = reader.ReadInt32();

                var savePos = reader.BaseStream.Position;
                reader.BaseStream.Seek( startPosition + bodyOffset, SeekOrigin.Begin );

                switch( magic ) {
                    case CutbResourceList.MAGIC:
                        ResourceList = new( reader );
                        break;
                    case CutbInitialState.MAGIC:
                        InitialState = new( reader );
                        break;
                    case CutbDefaultScene.MAGIC:
                        DefaultScene = new( reader );
                        break;
                    case CutbEX.MAGIC:
                        EXs.Add( new( reader ) );
                        break;
                    case CutbActorList.MAGIC:
                        ActorList = new( reader );
                        break;
                    case CutbCB.MAGIC:
                        CB = new( reader );
                        break;
                    case CutbAnimation.MAGIC:
                        Animation = new( reader );
                        break;
                    case CutbTimeline.MAGIC:
                        Timelines.Add( new( reader, this ) );
                        break;
                }

                reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            }

            // Padded to 16 bytes after each body, and after headers

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );

            TimelineDropdown = new( "Timeline", Timelines,
                null, () => new( this ), () => CommandManager.Cutb );
        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Resources" ) ) {
                if( tab ) ResourceList?.Draw();
            }

            using( var tab = ImRaii.TabItem( "Initial State" ) ) {
                if( tab ) InitialState?.Draw();
            }

            using( var tab = ImRaii.TabItem( "Default Scene" ) ) {
                if( tab ) DefaultScene?.Draw();
            }

            using( var tab = ImRaii.TabItem( "Timelines" ) ) {
                if( tab ) TimelineDropdown.Draw();
            }
        }
    }
}
