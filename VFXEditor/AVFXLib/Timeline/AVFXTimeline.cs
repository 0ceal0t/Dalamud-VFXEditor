using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Timeline {
    public class AVFXTimeline : AVFXBase {
        public const string NAME = "TmLn";

        public readonly AVFXInt LoopStart = new( "LpSt" );
        public readonly AVFXInt LoopEnd = new( "LpEd" );
        public readonly AVFXInt BinderIdx = new( "BnNo" );
        public readonly AVFXInt TimelineCount = new( "TICn" );
        public readonly AVFXInt ClipCount = new( "CpCn" );

        public readonly List<AVFXTimelineSubItem> Items = new();
        public readonly List<AVFXTimelineClip> Clips = new();

        private readonly List<AVFXBase> Children;

        public AVFXTimeline() : base( NAME ) {
            Children = new List<AVFXBase> {
                LoopStart,
                LoopEnd,
                BinderIdx,
                TimelineCount,
                ClipCount
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size );

            AVFXTimelineItem lastItem = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Item" ) {
                    lastItem = new AVFXTimelineItem();
                    lastItem.Read( _reader, _size );
                }
                else if( _name == "Clip" ) {
                    var clip = new AVFXTimelineClip();
                    clip.Read( _reader, _size );
                    Clips.Add( clip );
                }
            }, size );

            if( lastItem != null ) {
                Items.AddRange( lastItem.Items );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            TimelineCount.SetValue( Items.Count );
            ClipCount.SetValue( Clips.Count );
            WriteNested( writer, Children );

            // Item
            for( var i = 0; i < Items.Count; i++ ) {
                var item = new AVFXTimelineItem();
                item.Items.AddRange( Items.GetRange( 0, i + 1 ) );
                item.Write( writer );
            }

            foreach( var clip in Clips ) clip.Write( writer );
        }
    }
}
