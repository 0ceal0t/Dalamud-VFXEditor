using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class NpcInstanceObject : GameInstanceObject {
        private readonly ParsedUInt PopWeather = new( "Pop Weather" );
        private readonly ParsedByte PopTimeStart = new( "Pop Time Start" );
        private readonly ParsedByte PopTimeEnd = new( "Pop Time End" );
        private readonly ParsedUInt MoveAi = new( "Move Ai" );
        private readonly ParsedByte WanderingRange = new( "Wandering Range" );
        private readonly ParsedByte Route = new( "Route" );
        private readonly ParsedShort EventGroup = new( "Event Group" );

        public NpcInstanceObject( LayerEntryType type ) : base( type ) { }

        public NpcInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            base.DrawBody();
            PopWeather.Draw();
            PopTimeStart.Draw();
            PopTimeEnd.Draw();
            MoveAi.Draw();
            WanderingRange.Draw();
            Route.Draw();
            EventGroup.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            base.ReadBody( reader, startPos );
            PopWeather.Read( reader );
            PopTimeStart.Read( reader );
            PopTimeEnd.Read( reader );
            reader.ReadBytes( 2 ); // padding
            MoveAi.Read( reader );
            WanderingRange.Read( reader );
            Route.Read( reader );
            EventGroup.Read( reader );
            reader.ReadBytes( 8 ); // padding
        }
    }
}
