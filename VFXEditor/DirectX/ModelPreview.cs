using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat;
using VfxEditor.DirectX.Drawable;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class ModelPreview : ModelRenderer {
        public enum RenderMode {
            Color,
            Uv1,
            Uv2,
            Normal
        }

        private readonly DirectXDrawable Model;
        private readonly DirectXDrawable Emitters;

        public ModelPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            Model = new( Device, Path.Combine( shaderPath, "Model.fx" ), 3, true, false,
                new InputElement[] {
                    new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new InputElement( "COLOR", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new InputElement( "NORMAL", 0, Format.R32G32B32A32_Float, 32, 0 )
                } );

            Emitters = new( Device, Path.Combine( shaderPath, "Emitter.fx" ), 2, false, true,
                new InputElement[] {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0, InputClassification.PerVertexData, 0),
                    new InputElement("INSTANCE", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                    new InputElement("INSTANCE", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1)
                } );

            var builder = new MeshBuilder( true, false );
            builder.AddPyramid( new Vector3( 0, 0, 0 ), Vector3.UnitX, Vector3.UnitY, 0.25f, 0.5f, true );
            var data = FromMeshBuilder( builder, null, false, false, false, out var emitterCount );
            Emitters.SetVertexes( Device, data, emitterCount );
        }

        public void LoadModel( AvfxModel model, RenderMode mode ) => LoadModel( model.Indexes.Indexes, model.Vertexes.Vertexes, model.CombinedEmitVertexes, mode );

        public void LoadModel( List<AvfxIndex> modelIndexes, List<AvfxVertex> modelVertexes, List<UiEmitVertex> modelEmitters, RenderMode mode ) {
            if( modelIndexes.Count == 0 ) {
                Model.ClearVertexes();
            }
            else {
                var data = new List<Vector4>();

                for( var index = 0; index < modelIndexes.Count; index++ ) { // each face
                    var indexes = new int[] { modelIndexes[index].I1, modelIndexes[index].I2, modelIndexes[index].I3 };
                    for( var j = 0; j < indexes.Length; j++ ) { // push all 3 vertices per face
                        var vertex = modelVertexes[indexes[j]];
                        var normal = new Vector3( vertex.Normal[0], vertex.Normal[1], vertex.Normal[2] );
                        var color = new Vector3( vertex.Color[0], vertex.Color[1], vertex.Color[2] );
                        var uv1 = new Vector4( vertex.Uv1[0] + 0.5f, 0, vertex.Uv1[1] + 0.5f, 1.0f );
                        var uv2 = new Vector4( vertex.Uv2[2] + 0.5f, 0, vertex.Uv2[3] + 0.5f, 1.0f );

                        data.Add( new Vector4( vertex.Position[0], vertex.Position[1], vertex.Position[2], 1.0f ) );
                        data.Add( mode switch {
                            RenderMode.Color => new Vector4( color / 255, 1.0f ),
                            RenderMode.Uv1 => uv1,
                            RenderMode.Uv2 => uv2,
                            RenderMode.Normal => new Vector4( normal.Normalized(), 1.0f ),
                            _ => throw new NotImplementedException()
                        } );
                        data.Add( new Vector4( normal, 0 ) );
                    }
                }

                Model.SetVertexes( Device, data.ToArray(), modelIndexes.Count * 3 );
            }

            // ========= EMITTERS =====

            if( modelEmitters.Count == 0 ) {
                Emitters.ClearInstances();
            }
            else {
                var data = new List<Matrix>();
                for( var idx = 0; idx < modelEmitters.Count; idx++ ) {
                    var emitter = modelEmitters[idx];
                    var pos = new Vector3( emitter.Position.X, emitter.Position.Y, emitter.Position.Z );
                    var rot = GetEmitterRotationQuat( new Vector3( emitter.Normal.X, emitter.Normal.Y, emitter.Normal.Z ) );

                    data.Add( Matrix.AffineTransformation( 1f, rot, pos ) );
                }

                Emitters.SetInstances( Device, data.ToArray(), modelEmitters.Count );
            }

            UpdateDraw();
        }

        private static Quaternion GetEmitterRotationQuat( Vector3 normal ) {
            var originalNormal = Vector3.UnitY;
            if( normal.Equals( originalNormal ) ) return Quaternion.Identity;

            var rotationAxis = Vector3.Cross( normal, originalNormal );
            if( rotationAxis.Length() == 0f ) { // N = -N'
                return Quaternion.RotationAxis( Vector3.UnitX, ( float )Math.PI );
            }

            var rotationAngle = Math.Acos( Vector3.Dot( normal, originalNormal ) / ( normal.Length() * originalNormal.Length() ) );

            return Quaternion.RotationAxis( rotationAxis, ( float )rotationAngle );
        }

        public override void OnDraw() {
            Model.Draw( Ctx, VertexShaderBuffer, PixelShaderBuffer );
            Emitters.Draw( Ctx, VertexShaderBuffer, PixelShaderBuffer );
        }

        public override void OnDispose() {
            Model?.Dispose();
            Emitters?.Dispose();
        }
    }
}
