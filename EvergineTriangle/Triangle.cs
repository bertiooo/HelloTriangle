using Evergine.Common.Graphics;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Mathematics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace EvergineTriangle
{
    internal class Triangle : MainRenderPassRenderable
    {
        private Buffer? _vertexBuffer;

        private Shader? _vertexShader;
        private Shader? _pixelShader;

        private GraphicsPipelineState? _pipelineState;

        public Triangle(uint width, uint height)
            : base(width, height)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            var graphicsContext = this.GraphicsContext;
            var resourceFactory = graphicsContext.Factory;
            
            // --- create buffers

            VertexPosition[] vertices =
            {
                new VertexPosition(new Vector3(0, 0.75f, 0)),
                new VertexPosition(new Vector3(0.75f, -0.75f, 0)),
                new VertexPosition(new Vector3(-0.75f, -0.75f, 0)),
            };

            var vertexBufferDesc = new BufferDescription((uint)(Unsafe.SizeOf<VertexPosition>() * vertices.Length), BufferFlags.VertexBuffer, ResourceUsage.Default);
            _vertexBuffer = resourceFactory.CreateBuffer(vertices, ref vertexBufferDesc);

            // --- create shaders

            var sourceCode = this.GetShaderSourceCode();

            var vertexShaderDesc = this.GetVertexShaderDescription(graphicsContext, sourceCode);
            var pixelShaderDesc = this.GetPixelShaderDescription(graphicsContext, sourceCode);

            _vertexShader = resourceFactory.CreateShader(ref vertexShaderDesc);
            _pixelShader = resourceFactory.CreateShader(ref pixelShaderDesc);

            // --- create pipeline

            var vertexLayouts = new InputLayouts().Add(VertexPosition.VertexFormat);
            var outputDescription = this.SwapChain.FrameBuffer.OutputDescription;

            var pipelineDescription = new GraphicsPipelineDescription
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = _vertexShader,
                    PixelShader = _pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.CullBack,
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Outputs = outputDescription,
                ResourceLayouts = Array.Empty<ResourceLayout>()
            };

            _pipelineState = resourceFactory.CreateGraphicsPipeline(ref pipelineDescription);
        }

        private string GetShaderSourceCode()
        {
            var path = "EvergineTriangle.HLSL.fx";
            var assembly = Assembly.GetCallingAssembly();

            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private ShaderDescription GetVertexShaderDescription(GraphicsContext graphicsContext, string sourceCode)
        {
            var entryPoint = "VSMain";
            var shaderStages = ShaderStages.Vertex;

            var compilerParams = CompilerParameters.Default; // with { Profile = GraphicsProfile.Level_12_0 };
            var byteCode = graphicsContext.ShaderCompile(sourceCode, entryPoint, shaderStages, compilerParams).ByteCode;

            return new ShaderDescription(shaderStages, entryPoint, byteCode);
        }

        private ShaderDescription GetPixelShaderDescription(GraphicsContext graphicsContext, string sourceCode)
        {
            var entryPoint = "PSMain";
            var shaderStages = ShaderStages.Pixel;

            var compilerParams = CompilerParameters.Default; // with { Profile = GraphicsProfile.Level_12_0 };
            var byteCode = graphicsContext.ShaderCompile(sourceCode, entryPoint, shaderStages, compilerParams).ByteCode;

            return new ShaderDescription(shaderStages, entryPoint, byteCode);
        }

        protected override void RenderInternal(CommandBuffer commandBuffer)
        {
            commandBuffer.SetGraphicsPipelineState(_pipelineState);

            commandBuffer.SetVertexBuffer(0, _vertexBuffer, 0);
            commandBuffer.Draw(3);
        }

        public override void Dispose()
        {
            _vertexBuffer?.Dispose();

            _pipelineState?.Dispose();

            _vertexShader?.Dispose();
            _pixelShader?.Dispose();

            base.Dispose();
        }
    }
}
