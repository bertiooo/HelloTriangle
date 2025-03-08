# Hello Triangle

## Overview

This project intends to use Evergine's Low-Level API to draw a simple white triangle on the screen. However, right now it only works with a DirectX11 graphic context. 

Switching to a DirectX12 graphic context in the [class Renderable, method Initialize()](EvergineTriangle/Renderable.cs) produces an exception:

> An unhandled exception of type 'System.AccessViolationException' occurred in Vortice.Direct3D12.dll
Attempted to read or write protected memory. This is often an indication that other memory is corrupt.

Any help appreciated.

Edit: this was a driver issue. After updating the driver it works, the triangle is drawn.

## Credits

This project is using following packages:

* [Evergine](https://evergine.com): [Evergine License](https://www.nuget.org/packages/Evergine.Common/2024.10.24.804/license)
* Vortice by Amer Koleci: [The MIT License (MIT)](https://licenses.nuget.org/MIT)
