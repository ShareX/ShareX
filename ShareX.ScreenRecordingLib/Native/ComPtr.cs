// SPDX-License-Identifier: GPL-3.0-or-later
namespace ShareX.ScreenRecordingLib.Native;

/// <summary>Owns exactly one native COM reference. Native API types never escape the library.</summary>
internal sealed unsafe class ComPtr<T> : IDisposable where T : unmanaged
{
    private nint pointer;
    public T* Pointer => (T*)pointer;

    public ComPtr(T* pointer)
    {
        if (pointer == null) throw new ArgumentNullException(nameof(pointer));
        this.pointer = (nint)pointer;
    }

    public void Dispose()
    {
        nint previous = Interlocked.Exchange(ref pointer, 0);
        if (previous != 0) ((IUnknown*)previous)->Release();
        GC.SuppressFinalize(this);
    }

    ~ComPtr() => Dispose();
}
