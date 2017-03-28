using System;
using System.Windows.Forms;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Direct2D1;
using SharpDX.Windows;
using System.Collections.Generic;

namespace ads.direct2d
{
    public class Direct2dWindowFactory
    {
        public string Caption { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public Direct2dWindowFactory()
        {
            Caption = "Direct2D Application";
            WindowWidth = 640;
            WindowHeight = 480;
        }

        public void Run(Action<RenderTarget, HashSet<Keys>> renderMethod)
        {
            var keys = new HashSet<Keys>();

            // The window in which all action will take place
            var window = new RenderForm(Caption);
            window.KeyDown += (o, e) => keys.Add(e.KeyCode);
            window.KeyUp += (o, e) => keys.Remove(e.KeyCode);

            // Describes the swap chain, i.e. the number (and properties) of buffer used for displaying frames.
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 2,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = window.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            // Create the swap chain and Direct3D device. BgraSupport is needed for Direct2D
            SharpDX.Direct3D11.Device device;
            SwapChain swapChain;
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, 
                DeviceCreationFlags.BgraSupport, swapChainDesc, out device, out swapChain);

            // Get back buffer in a Direct2D-compatible format (DXGI surface)
            Surface backBuffer = Surface.FromSwapChain(swapChain, 0);

            // Create a Direct2D factory
            RenderTarget renderTarget;
            using (var factory = new SharpDX.Direct2D1.Factory())
            {
                // Get desktop DPI
                var dpi = factory.DesktopDpi;

                // Create bitmap render target from DXGI surface
                renderTarget = new RenderTarget(factory, backBuffer, new RenderTargetProperties()
                {
                    DpiX = dpi.Width,
                    DpiY = dpi.Height,
                    MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT,
                    PixelFormat = new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Ignore),
                    Type = RenderTargetType.Default,
                    Usage = RenderTargetUsage.None
                });
            }

            // Disable automatic ALT + Enter processing because it doesn't work properly with WinForms,
            // and add a special event handler for ALT + Enter.
            using (var factory = swapChain.GetParent<SharpDX.DXGI.Factory1>())
                factory.MakeWindowAssociation(window.Handle, WindowAssociationFlags.IgnoreAltEnter);
            window.KeyDown += (o, e) =>
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                    swapChain.IsFullScreen = !swapChain.IsFullScreen;
            };

            // Set window size
            window.Size = new System.Drawing.Size(WindowWidth, WindowHeight);

            // Prevent window from being re-sized
            window.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Rendering function
            RenderLoop.Run(window, () =>
            {
                renderTarget.BeginDraw();
                renderMethod(renderTarget, keys);
                renderTarget.EndDraw();
                swapChain.Present(0, PresentFlags.None);
            });

            renderTarget.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }
    }
}
