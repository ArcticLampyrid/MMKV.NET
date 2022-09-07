using System;
using System.IO;
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

        [Test]
        public void BackupOne()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(tempDir);
                using (var mmkv = Mmkv.WithID("backupone", MmkvMode.SingleProcess))
                {
                    mmkv.Clear();
                    mmkv.Set("test", 123);
                    Mmkv.BackupOneToDirectory("backupone", tempDir);
                    mmkv.Clear();
                }
                Mmkv.RestoreOneFromDirectory("backupone", tempDir);
                using (var mmkv = Mmkv.WithID("backupone", MmkvMode.SingleProcess))
                {
                    Assert.That(mmkv.GetInt32("test"), Is.EqualTo(123));
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch (Exception e)
                {
                    TestContext.Out.WriteLine("Failed to delete backup directory: {0}", e);
                }
            }
        }

        [Test]
        public void BackupAll()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                Directory.CreateDirectory(tempDir);
                for (int i = 0; i < 10; i++)
                {
                    using (var mmkv = Mmkv.WithID($"backupall{i}", MmkvMode.SingleProcess))
                    {
                        mmkv.Clear();
                        mmkv.Set("test", i);
                    }
                }
                Mmkv.BackupAllToDirectory(tempDir);
                for (int i = 0; i < 10; i++)
                {
                    using (var mmkv = Mmkv.WithID($"backupall{i}", MmkvMode.SingleProcess))
                    {
                        mmkv.Clear();
                    }
                }
                Mmkv.RestoreAllFromDirectory(tempDir);
                for (int i = 0; i < 10; i++)
                {
                    using (var mmkv = Mmkv.WithID($"backupall{i}", MmkvMode.SingleProcess))
                    {
                        Assert.That(mmkv.GetInt32("test"), Is.EqualTo(i));
                    }
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(tempDir, true);
                }
                catch (Exception e)
                {
                    TestContext.Out.WriteLine("Failed to delete backup directory: {0}", e);
                }
            }
        }
    }
}