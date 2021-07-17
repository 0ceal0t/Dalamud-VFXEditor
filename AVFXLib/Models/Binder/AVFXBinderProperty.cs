using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXBinderProperty : Base
    {
        public string Name = "PrpS";

        public LiteralEnum<BindPoint> BindPointType = new LiteralEnum<BindPoint>("BPT");
        public LiteralEnum<BindTargetPoint> BindTargetPointType = new LiteralEnum<BindTargetPoint>("BPTP");
        public LiteralString BinderName = new LiteralString("Name", fixedSize:8);
        public LiteralInt BindPointId = new LiteralInt("BPID");
        public LiteralInt GenerateDelay = new LiteralInt("GenD");
        public LiteralInt CoordUpdateFrame = new LiteralInt("CoUF");
        public LiteralBool RingEnable = new LiteralBool("bRng");
        public LiteralInt RingProgressTime = new LiteralInt("RnPT");
        public LiteralFloat RingPositionX = new LiteralFloat("RnPX");
        public LiteralFloat RingPositionY = new LiteralFloat("RnPY");
        public LiteralFloat RingPositionZ = new LiteralFloat("RnPZ");
        public LiteralFloat RingRadius = new LiteralFloat("RnRd");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("Pos");

        List<Base> Attributes;

        public AVFXBinderProperty(string name) : base(name)
        {
            Name = name;
            Attributes = new List<Base>(new Base[]{
                BindPointType,
                BindTargetPointType,
                BinderName,
                BindPointId,
                GenerateDelay,
                CoordUpdateFrame,
                RingEnable,
                RingProgressTime,
                RingPositionX,
                RingPositionY,
                RingPositionZ,
                RingRadius,
                Position
            });
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void ToDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            BinderName.GiveValue("null");

            BindTargetPointType.GiveValue( BindTargetPoint.ByName );
            BindPointId.GiveValue( 3 );
            CoordUpdateFrame.GiveValue( -1 );
            RingProgressTime.GiveValue( 1 );
            Position.ToDefault();
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode(Name);
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
