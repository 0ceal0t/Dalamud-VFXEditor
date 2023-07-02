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
                        Timelines.Add( new( reader ) );
                        break;
                }

                reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            }

            // Padded to 16 bytes after each body, and after headers

            if( checkOriginal ) Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
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

            using( var tab = ImRaii.TabItem( "Animation" ) ) {
                if( tab ) Animation?.Draw();
            }
        }

        /*
struct default_scene_header_t {
  uint32_t level_path_offset;
  uint32_t id;
  focus_state_t focus;
  weather_state_t weather;
  instance_list_t ignore_list;
  default_scene_flags_t flags;
  uint32_t keep_range_id_list; // I don't actually know much about this
  uint32_t keep_range_id_list_count;
}

enum default_scene_flags_t {
  IgnoreList = 0x01,
  UseLayerSet = 0x02,
  KeepRange = 0x04,
}

struct focus_state_t {
  focus_type_t type;
  uint32_t actor_instance_id;
  float x;
  float y;
  float z;
  float size;
}

enum focus_type_t {
  None = 0x00,
  Camera = 0x01,
  Actor = 0x02,
  Position = 0x03
}

struct weather_state_t {
  uint8_t use_weather;
  uint8_t use_time;
  uint8_t use_speed;
  uint8_t use_angle;
  uint8_t use_blend;
  uint8_t _unknown[3]
  uint32_t weather_id;
  uint32_t time;
  float speed;
  uint32_t angle;
  uint32_t blend;
}

struct instance_list_header_t {
  uint32_t offset;
  uint32_t count;
}

//at 'offset'
instance_list_t instance_list[count];

struct instance_list_t {
  uint8_t flag; // think this decides visibility and shit, not sure
  asset_type_t type;
  uint16_t layer_id;
  uint32_t instance_id;
}

enum asset_type_t {
  None = 0x00,
  BG = 0x01,
  Attribute = 0x02,
  LayLight = 0x03,
  VFX = 0x04,
  //... etc, idk, if you have all of these great, if not, ask and I'll see what I can do
}
        */
    }
}
