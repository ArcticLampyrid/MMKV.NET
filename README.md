# ManagedMmkv
[![NuGet](https://img.shields.io/nuget/v/Alampy.ManagedMmkv.svg)](https://www.nuget.org/packages/Alampy.ManagedMmkv)  
[Tencent MMKV](https://github.com/Tencent/MMKV) bindings for modern .NET (unofficial)

## Sample Code
```csharp
using (var mmkv = Mmkv.Default(MmkvMode.SingleProcess))
{
    mmkv.Set("test", 123);
    Assert.That(mmkv.GetInt32("test"), Is.EqualTo(123));
}
using (var mmkv = Mmkv.Default(MmkvMode.SingleProcess))
{
    Assert.Multiple(() =>
    {
        Assert.That(mmkv.GetInt32("test"), Is.EqualTo(123));
        Assert.Catch(() => mmkv.GetString("test"));
        Assert.That(mmkv.GetInt32OrDefault("test456", 456), Is.EqualTo(456));
    });
}
```

## Requirements
- .NET Core 3.1+ or .NET 5+
- Running on one of the following architectures:
- - win-x86
- - win-x64
- - win-arm64
- - linux-x64
- - linux-arm64
- - mac-x64

## License
Licensed under the MIT License