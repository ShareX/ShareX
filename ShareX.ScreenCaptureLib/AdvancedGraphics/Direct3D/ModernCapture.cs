using System;
using System.Reflection;
using System.IO;
using System.Threading;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;

using SharpDX;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using WIC = SharpDX.WIC;
using D3DCompiler = SharpDX.D3DCompiler;

using ShareX.ScreenCaptureLib.AdvancedGraphics.Direct3D.Shaders;
using System.Drawing;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics.Direct3D
{
    public class ModernCapture : IDisposable
    {
        private ModernCaptureItemDescription description;

        private IDirect3DDevice wrtD3D11Device;
        private D3D11.Device d3dDevice;
        private D3D11.DeviceContext d3dContext;

        private D3D11.VertexShader vsQuad;
        private D3DCompiler.ShaderSignature shaderInputSigVsQuad;
        private D3D11.InputElement[] shaderInputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, DXGI.Format.R32G32B32_Float, 0),
            new D3D11.InputElement("TEXCOORD", 0, DXGI.Format.R32G32_Float, 0)
        };
        private D3D11.InputLayout inputLayout;

        private D3D11.PixelShader psToneMapping;
        private D3D11.SamplerState samplerState;

        private D3D11.Texture2D textureSDRImage;
        private D3D11.RenderTargetView rtvSdrTexture;

        private WIC.ImagingFactory2 wicFactory;

        private ModernCaptureMonitorSession currentSession;

        public ModernCapture(ModernCaptureItemDescription itemDescription)
        {
            description = itemDescription;

            wrtD3D11Device = Direct3D11Helper.CreateDevice();
            d3dDevice = Direct3D11Helper.CreateSharpDXDevice(wrtD3D11Device);
            d3dContext = d3dDevice.ImmediateContext;
            wicFactory = new WIC.ImagingFactory2();

            InitializeShaders();
        }

        private void InitializeShaders()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var vxShaderStream = assembly.GetManifestResourceStream($"{ShaderConstant.ResourcePrefix}.PostProcessingQuad.cso"))
            using (var psShaderStream = assembly.GetManifestResourceStream($"{ShaderConstant.ResourcePrefix}.PostProcessingColor.cso"))
            {
                using (var vbc = D3DCompiler.ShaderBytecode.FromStream(vxShaderStream))
                using (var pbcTm = D3DCompiler.ShaderBytecode.FromStream(psShaderStream))
                {
                    psToneMapping = new D3D11.PixelShader(d3dDevice, pbcTm);
                    vsQuad = new D3D11.VertexShader(d3dDevice, vbc);
                    shaderInputSigVsQuad = D3DCompiler.ShaderSignature.GetInputSignature(vbc);
                }
            }
        }

        public Bitmap CaptureAndProcess()
        {
            // Initialize DirectX context (for the canvas)
            textureSDRImage = new D3D11.Texture2D(d3dDevice, new D3D11.Texture2DDescription
            {
                Width = description.CanvasRect.Width,
                Height = description.CanvasRect.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = DXGI.Format.B8G8R8A8_UNorm_SRgb,
                Usage = D3D11.ResourceUsage.Default,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                BindFlags = D3D11.BindFlags.RenderTarget,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None,
            });
            rtvSdrTexture = new D3D11.RenderTargetView(d3dDevice, textureSDRImage);
            inputLayout = new D3D11.InputLayout(d3dDevice, shaderInputSigVsQuad, shaderInputElements);
            samplerState = new D3D11.SamplerState(d3dDevice, new D3D11.SamplerStateDescription
            {
                AddressU = D3D11.TextureAddressMode.Wrap,
                AddressV = D3D11.TextureAddressMode.Wrap,
                AddressW = D3D11.TextureAddressMode.Wrap,
                MinimumLod = 0,
                MipLodBias = 0.0f,
                MaximumLod = float.MaxValue,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(0.0f, 0.0f, 0.0f, 0.0f),
                Filter = D3D11.Filter.MinMagMipLinear,
            });

            d3dContext.Rasterizer.SetViewport(new Viewport(0, 0, description.CanvasRect.Width, description.CanvasRect.Height));
            d3dContext.OutputMerger.SetRenderTargets(rtvSdrTexture);
            d3dContext.ClearRenderTargetView(rtvSdrTexture, new SharpDX.Mathematics.Interop.RawColor4 { A = 0.0f, B = 0.0f, G = 0.0f, R = 0.0f });

            foreach (var item in description.Regions)
            {
                using (var session = new ModernCaptureMonitorSession(wrtD3D11Device, item))
                {
                    Direct3D11CaptureFrame f;
                    currentSession = session;
                    session.Session.StartCapture();
                    while ((f = session.FramePool.TryGetNextFrame()) == null)
                    {
                        Thread.Sleep(1);
                    }
                    ProcessFrame(f);
                }  
            }

            // Process final 8-bit bitmap
            return new Bitmap(DumpAndSaveImage());
        }

        private Stream DumpAndSaveImage()
        {
            var stream = new MemoryStream();
            var textureSDRCpuCopy = new D3D11.Texture2D(d3dDevice, new D3D11.Texture2DDescription
            {
                Width = description.CanvasRect.Width,
                Height = description.CanvasRect.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = DXGI.Format.B8G8R8A8_UNorm_SRgb,
                Usage = D3D11.ResourceUsage.Staging,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                BindFlags = D3D11.BindFlags.None,
                CpuAccessFlags = D3D11.CpuAccessFlags.Read,
                OptionFlags = D3D11.ResourceOptionFlags.None,
            });

            DataStream rawSdrImageDataStream;
            d3dContext.CopyResource(textureSDRImage, textureSDRCpuCopy);

            var dataBox = d3dContext.MapSubresource(textureSDRCpuCopy, 0, 0, D3D11.MapMode.Read, D3D11.MapFlags.None, out rawSdrImageDataStream);
            var dataRectangle = new DataRectangle
            {
                DataPointer = rawSdrImageDataStream.DataPointer,
                Pitch = dataBox.RowPitch,
            };

            using (var bitmap = new WIC.Bitmap(wicFactory, description.CanvasRect.Width, description.CanvasRect.Height, WIC.PixelFormat.Format32bppBGRA, dataRectangle))
            using (var imageEncoder = new WIC.BmpBitmapEncoder(wicFactory, stream))
            using (var encodeInstance = new WIC.BitmapFrameEncode(imageEncoder))
            {
                encodeInstance.Initialize();
                encodeInstance.SetSize(bitmap.Size.Width, bitmap.Size.Height);
                var pixelFormat = WIC.PixelFormat.Format24bppBGR;
                encodeInstance.SetPixelFormat(ref pixelFormat);
                encodeInstance.WriteSource(bitmap);
                encodeInstance.Commit();
                imageEncoder.Commit();
                stream.Flush();
            }

            d3dContext.UnmapSubresource(textureSDRCpuCopy, 0);
            rawSdrImageDataStream.Dispose();
            textureSDRCpuCopy.Dispose();

            return stream;
        }

        private void ProcessFrame(Direct3D11CaptureFrame frame)
        {
            // Do proecssing
            using (frame)
            {
                using (var texture = Direct3D11Helper.CreateSharpDXTexture2D(frame.Surface))
                {
                    var hdrMetadata = currentSession.HdrMetadata;
                    var vertices = new ShaderInput[]
                    {
                            // Left-Top
                            new ShaderInput {
                                Position = new Vector3(currentSession.DestD3DVsTopLeft.X, currentSession.DestD3DVsTopLeft.Y, 0),
                                TextureCoord = new Vector2(currentSession.DestD3DPsSamplerTopLeft.X, currentSession.DestD3DPsSamplerTopLeft.Y),
                            },
                            // Right-Top
                            new ShaderInput {
                                Position = new Vector3(currentSession.DestD3DVsBottomRight.X, currentSession.DestD3DVsTopLeft.Y, 0),
                                TextureCoord = new Vector2(currentSession.DestD3DPsSamplerBottomRight.X, currentSession.DestD3DPsSamplerTopLeft.Y)
                            },
                            // Left-Bottom
                            new ShaderInput {
                                Position = new Vector3(currentSession.DestD3DVsTopLeft.X, currentSession.DestD3DVsBottomRight.Y, 0),
                                TextureCoord = new Vector2(currentSession.DestD3DPsSamplerTopLeft.X, currentSession.DestD3DPsSamplerBottomRight.Y)
                            },
                            // Right-Top
                            new ShaderInput {
                                Position = new Vector3(currentSession.DestD3DVsBottomRight.X, currentSession.DestD3DVsTopLeft.Y, 0),
                                TextureCoord = new Vector2(currentSession.DestD3DPsSamplerBottomRight.X, currentSession.DestD3DPsSamplerTopLeft.Y)
                            },
                            // Right-Bottom
                            new ShaderInput {
                                Position = new Vector3(currentSession.DestD3DVsBottomRight.X, currentSession.DestD3DVsBottomRight.Y, 0),
                                TextureCoord = new Vector2(currentSession.DestD3DPsSamplerBottomRight.X, currentSession.DestD3DPsSamplerBottomRight.Y)
                            },
                            // Left-Bottom
                            new ShaderInput {
                                Position = new Vector3(currentSession.DestD3DVsTopLeft.X, currentSession.DestD3DVsBottomRight.Y, 0),
                                TextureCoord = new Vector2(currentSession.DestD3DPsSamplerTopLeft.X, currentSession.DestD3DPsSamplerBottomRight.Y)
                            },
                    };

                    var triangleVertexBuffer = D3D11.Buffer.Create(d3dDevice, D3D11.BindFlags.VertexBuffer, vertices);
                    var hdrMetadataBuffer = new D3D11.Buffer(d3dDevice,
                        Utilities.SizeOf<ShaderHdrMetadata>(),
                        D3D11.ResourceUsage.Default,
                        D3D11.BindFlags.ConstantBuffer,
                        D3D11.CpuAccessFlags.None,
                        D3D11.ResourceOptionFlags.None,
                        0);

                    d3dContext.UpdateSubresource(ref hdrMetadata, hdrMetadataBuffer);

                    d3dContext.InputAssembler.PrimitiveTopology = D3D.PrimitiveTopology.TriangleList;
                    d3dContext.InputAssembler.InputLayout = inputLayout;
                    d3dContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(triangleVertexBuffer, Utilities.SizeOf<ShaderInput>(), 0));

                    d3dContext.VertexShader.Set(vsQuad);
                    d3dContext.PixelShader.SetConstantBuffer(0, hdrMetadataBuffer);
                    d3dContext.PixelShader.SetSampler(0, samplerState);

                    var canvasTexture = new D3D11.Texture2D(d3dDevice, new D3D11.Texture2DDescription
                    {
                        Width = texture.Description.Width,
                        Height = texture.Description.Height,
                        MipLevels = 1,
                        ArraySize = 1,
                        Format = currentSession.HdrMetadata.EnableHdrProcessing ? DXGI.Format.R16G16B16A16_Float : DXGI.Format.B8G8R8A8_UNorm_SRgb,
                        Usage = D3D11.ResourceUsage.Default,
                        SampleDescription = new DXGI.SampleDescription(1, 0),
                        BindFlags = D3D11.BindFlags.ShaderResource,
                        CpuAccessFlags = D3D11.CpuAccessFlags.None,
                        OptionFlags = D3D11.ResourceOptionFlags.None,
                    });

                    using (canvasTexture)
                    using (var shaderResView = new D3D11.ShaderResourceView(d3dDevice, canvasTexture))
                    {
                        d3dContext.CopyResource(texture, canvasTexture);
                        d3dContext.PixelShader.SetShaderResource(0, shaderResView);
                        d3dContext.PixelShader.Set(psToneMapping);
                        d3dContext.Draw(vertices.Length, 0);
                    }

                    triangleVertexBuffer.Dispose();
                    hdrMetadataBuffer.Dispose();
                }
            }

            // Cleanup and signal event to proceed
            currentSession.Session.Dispose();
        }

        public void Dispose()
        {
            inputLayout?.Dispose();
            rtvSdrTexture?.Dispose();
            textureSDRImage?.Dispose();
            samplerState?.Dispose();

            shaderInputSigVsQuad.Dispose();
            psToneMapping.Dispose();
            vsQuad.Dispose();

            wrtD3D11Device.Dispose();
            d3dDevice.Dispose();
            wicFactory.Dispose();
        }
    }
}
