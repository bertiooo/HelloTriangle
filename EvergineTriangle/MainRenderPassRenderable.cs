using Evergine.Common.Graphics;
using Evergine.Mathematics;

namespace EvergineTriangle
{
    /// <summary>
    /// Handles the main render pass to which subclasses render to.
    /// </summary>
    internal abstract class MainRenderPassRenderable : Renderable
    {
        private Viewport[] _viewports;
        private Rectangle[] _scissors;

        private RenderPassDescription _renderPassDescription;

        protected MainRenderPassRenderable(uint width, uint height) 
            : base(width, height)
        {
            _viewports = [];
            _scissors = [];
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.OnFrameBufferChanged();
        }

        protected override void Render(CommandBuffer commandBuffer)
        {
            commandBuffer.BeginRenderPass(ref _renderPassDescription);

            commandBuffer.SetViewports(_viewports);
            commandBuffer.SetScissorRectangles(_scissors);

            this.RenderInternal(commandBuffer);

            commandBuffer.EndRenderPass();
        }

        protected abstract void RenderInternal(CommandBuffer commandBuffer);

        protected override void OnResize(uint width, uint height)
        {
            base.OnResize(width, height);
            this.OnFrameBufferChanged();
        }

        private void OnFrameBufferChanged()
        {
            var frameBuffer = this.SwapChain.FrameBuffer;

            _viewports =
            [
                new Viewport(0, 0, frameBuffer.Width, frameBuffer.Height)
            ];

            _scissors =
            [
                new Rectangle(0, 0, (int)frameBuffer.Width, (int)frameBuffer.Height)
            ];

            _renderPassDescription = new RenderPassDescription(frameBuffer, ClearValue.Default);
        }
    }
}
