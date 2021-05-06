using SharpDX.D3DCompiler;
using System;
using System.IO;
using System.Reflection;

namespace ShareX.ScreenCaptureLib.AdvancedGraphics.Direct3D.Shaders
{
    class HLSLShaderIncludeHandler : Include
    {
        public IDisposable Shadow { get; set; }

        public void Close(Stream stream)
        {
            stream.Close();
            stream.Dispose();
        }

        public void Dispose()
        {
            Shadow?.Dispose();
        }

        public Stream Open(IncludeType type, string fileName, Stream parentStream)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream($"{ShaderConstant.ResourcePrefix}.{fileName}");
        }
    }
}
