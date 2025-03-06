using Evergine.Common.Graphics;
using Evergine.Common.Helpers;
using Evergine.Forms;

namespace EvergineTriangle
{
    /// <summary>
    /// Abstract class which creates the context on which subclasses can render to.
    /// </summary>
    internal abstract class Renderable : IDisposable
    {
        private readonly WindowsSystem _windowsSystem;
        private readonly Surface _surface;

        private GraphicsContext? _graphicsContext;

        private SwapChain? _swapChain;
        private CommandQueue? _commandQueue;

        private bool _isDisposed = false;

        private bool _surfaceInvalidated;
        private bool _surfaceResized;

        protected Renderable(uint width, uint height)
        {
            _windowsSystem = new FormsWindowsSystem();
            _surface = _windowsSystem.CreateWindow(this.GetType().Name, width, height);

            _surface.Closing += this.OnSurfaceClosing;
            _surface.OnScreenSizeChanged += this.OnScreenSizeChanged;
            _surface.OnSurfaceInfoChanged += this.OnSurfaceInfoChanged;
        }

        protected GraphicsContext GraphicsContext => _graphicsContext ?? throw new InvalidOperationException("Graphics context has not been set yet.");

        protected SwapChain SwapChain => _swapChain ?? throw new InvalidOperationException("Swap chain has not been set yet.");

        private void OnSurfaceClosing(object? sender, EventArgs eventArgs)
        {
            _surfaceInvalidated = true;
        }

        private void OnSurfaceInfoChanged(object? sender, SurfaceInfo surfaceInfo)
        {
            _swapChain?.RefreshSurfaceInfo(surfaceInfo);

            _surfaceInvalidated = false;
            _surfaceResized = true;
        }

        private void OnScreenSizeChanged(object? sender, SizeEventArgs e)
        {
            _surfaceResized = true;
        }

        protected virtual void Initialize()
        {
            // works
            //_graphicsContext = new Evergine.DirectX11.DX11GraphicsContext();

            // doesn't work
            _graphicsContext = new Evergine.DirectX12.DX12GraphicsContext();

#if DEBUG
            _graphicsContext.CreateDevice(new ValidationLayer(ValidationLayer.NotifyMethod.Trace));
#else
            _graphicsContext.CreateDevice();
#endif

            var swapChainDescriptor = new SwapChainDescription()
            {
                Width = _surface.Width,
                Height = _surface.Height,
                SurfaceInfo = _surface.SurfaceInfo,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60
            };

            _swapChain = _graphicsContext.CreateSwapChain(swapChainDescriptor);
            _swapChain.VerticalSync = false;

            var resourceFactory = _graphicsContext.Factory;
            _commandQueue = resourceFactory.CreateCommandQueue();
        }

        private void Render()
        {
            if (_isDisposed || _surfaceInvalidated)
                return;

            if (_swapChain == null || _commandQueue == null)
                return;

            if (_surfaceResized)
            {
                _surfaceResized = false;

                var screenWidth = _surface.Width;
                var screenHeight = _surface.Height;

                _swapChain.ResizeSwapChain(screenWidth, screenHeight);
                this.OnResize(screenWidth, screenHeight);
            }

            _swapChain.InitFrame();

            var commandBuffer = _commandQueue.CommandBuffer();

            commandBuffer.Begin();
            this.Render(commandBuffer);
            commandBuffer.End();

            commandBuffer.Commit();

            _commandQueue.Submit();
            _commandQueue.WaitIdle();

            _swapChain.Present();
        }

        protected abstract void Render(CommandBuffer commandBuffer);

        protected virtual void OnResize(uint width, uint height)
        {
        }

        public void Run()
        {
            _windowsSystem.Run(this.Initialize, this.Render);
        }

        public virtual void Dispose()
        {
            _commandQueue?.Dispose();
            _swapChain?.Dispose();

            _graphicsContext?.Dispose();

            _surface.Dispose();
            _windowsSystem.Dispose();

            _isDisposed = true;
        }
    }
}
