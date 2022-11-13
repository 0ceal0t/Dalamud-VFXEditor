using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitterDataModel : AvfxData {
        public readonly AvfxInt ModelIdx = new( "Model Index", "MdNo" );
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxEnum<GenerateMethod> GenerateMethodType = new( "Generate Method", "GeMT" );
        public readonly AvfxCurve AX = new( "Angle X", "AnX" );
        public readonly AvfxCurve AY = new( "Angle Y", "AnY" );
        public readonly AvfxCurve AZ = new( "Angle Z", "AnZ" );
        public readonly AvfxCurve InjectionSpeed = new( "Injection Speed", "IjS" );
        public readonly AvfxCurve InjectionSpeedRandom = new( "Injection Speed Random", "IjSR" );

        public readonly UiNodeSelect<AvfxModel> ModelSelect;
        public readonly UiParameters Parameters;

        public AvfxEmitterDataModel( AvfxEmitter emitter ) : base() {
            Children = new() {
                ModelIdx,
                RotationOrderType,
                GenerateMethodType,
                AX,
                AY,
                AZ,
                InjectionSpeed,
                InjectionSpeedRandom
            };
            ModelIdx.SetValue( -1 );

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UiNodeSelect<AvfxModel>( emitter, "Model", emitter.NodeGroups.Models, ModelIdx ) );
            Parameters.Add( RotationOrderType );
            Parameters.Add( GenerateMethodType );
            Tabs.Add( AX );
            Tabs.Add( AY );
            Tabs.Add( AZ );
            Tabs.Add( InjectionSpeed );
            Tabs.Add( InjectionSpeedRandom );
        }

        public override void Enable() {
            ModelSelect.Enable();
        }

        public override void Disable() {
            ModelSelect.Disable();
        }
    }
}
