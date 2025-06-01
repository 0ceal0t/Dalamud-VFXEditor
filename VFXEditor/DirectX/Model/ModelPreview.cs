using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat;
using VfxEditor.DirectX.Drawable;
using VfxEditor.DirectX.Renderers;
using Device = SharpDX.Direct3D11.Device;

namespace VfxEditor.DirectX {
    public class ModelPreview : ModelRenderer {
        public enum RenderMode {
            Color,
            Uv1,
            Uv2,
            Uv3,
            Uv4,
            Normal
        }

        private readonly D3dDrawable Model;
        private readonly D3dDrawable Emitters;

        public ModelPreview( Device device, DeviceContext ctx, string shaderPath ) : base( device, ctx, shaderPath ) {
            Model = new( 3, false,
                [
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
                    new( "COLOR", 0, Format.R32G32B32A32_Float, 16, 0 ),
                    new( "NORMAL", 0, Format.R32G32B32A32_Float, 32, 0 )
                ] );
            Model.AddPass( device, PassType.Final, Path.Combine( shaderPath, "Model.fx" ), ShaderPassFlags.Pixel | ShaderPassFlags.Geometry );

            Emitters = new( 2, true,
                [
                    new( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0 ),
                    new( "NORMAL", 0, Format.R32G32B32A32_Float, 16, 0, InputClassification.PerVertexData, 0 ),
                    new( "INSTANCE", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1 ),
                    new( "INSTANCE", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1 ),
                    new( "INSTANCE", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1 ),
                    new( "INSTANCE", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1 )
                ] );
            Emitters.AddPass( device, PassType.Final, Path.Combine( shaderPath, "Emitter.fx" ), ShaderPassFlags.Pixel );

            UpdatePyramidMesh();
        }

        public void UpdatePyramidMesh() {
            var builder = new MeshBuilder( true, false );
            builder.AddPyramid(
                new Vector3( 0, 0, 0 ),
                Vector3.UnitX, Vector3.UnitY,
                Plugin.Configuration.ModelEmittersSize.X,
                Plugin.Configuration.ModelEmittersSize.Y,
                true );
            var data = FromMeshBuilder( builder, null, false, false, false, out var emitterCount );
            Emitters.SetVertexes( Device, data, emitterCount );
        }

        public void LoadModel( AvfxModel model, RenderMode mode ) => LoadModel( model.Indexes.Indexes, model.Vertexes.Vertexes, model.AllEmitVertexes, mode );

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

                        data.Add( new Vector4( vertex.Position[0], vertex.Position[1], vertex.Position[2], 1.0f ) );
                        data.Add( mode switch {
                            RenderMode.Color => new Vector4( new Vector3( vertex.Color[0], vertex.Color[1], vertex.Color[2] ) / 255, 1.0f ),
                            RenderMode.Uv1 => new Vector4( vertex.Uv1.X + 0.5f, 0, vertex.Uv1.Y + 0.5f, 1.0f ),
                            RenderMode.Uv2 => new Vector4( vertex.Uv2.X + 0.5f, 0, vertex.Uv2.Y + 0.5f, 1.0f ),
                            RenderMode.Uv3 => new Vector4( vertex.Uv3.X + 0.5f, 0, vertex.Uv3.Y + 0.5f, 1.0f ),
                            RenderMode.Uv4 => new Vector4( vertex.Uv4.X + 0.5f, 0, vertex.Uv4.Y + 0.5f, 1.0f ),
                            RenderMode.Normal => new Vector4( normal.Normalized(), 1.0f ),
                            _ => throw new NotImplementedException()
                        } );
                        data.Add( new Vector4( normal, 0 ) );
                    }
                }

                Model.SetVertexes( Device, [.. data], modelIndexes.Count * 3 );
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
                Emitters.SetInstances( Device, [.. data], modelEmitters.Count );
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

        protected override void DrawPasses() {
            Model.Draw( Ctx, PassType.Final, VertexShaderBuffer, PixelShaderBuffer );
            if( Plugin.Configuration.ModelShowEmitters ) {
                Emitters.Draw( Ctx, PassType.Final, VertexShaderBuffer, PixelShaderBuffer );
            }
        }

        protected override void DrawPopup() => Plugin.Configuration.DrawDirectXVfx();

        public override void Dispose() {
            base.Dispose();
            Model?.Dispose();
            Emitters?.Dispose();
        }
    }
}
