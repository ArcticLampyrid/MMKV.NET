using Alampy.ManagedMmkv;
using NUnit.Framework;

namespace ManagedMmkvTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void VersionName()
        {
            Assert.That(Mmkv.VersionName, Does.StartWith("v"));
        }

        [Test]
        public void SimpleUsage()
        {
            using (var mmkv = Mmkv.Default(MmkvMode.SingleProcess))
            {
                mmkv.Clear();
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
        }
    }
}