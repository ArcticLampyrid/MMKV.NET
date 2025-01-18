using System;
using System.Collections.Generic;
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
        public void WithExpire()
        {
            using (var mmkv = Mmkv.WithID("test-with-expire", MmkvMode.SingleProcess))
            {
                if (!mmkv.IsExpirationEnabled)
                {
                    mmkv.EnableAutoKeyExpire(Mmkv.ExpireNever);
                }
                Assert.That(mmkv.IsExpirationEnabled, Is.True);
                mmkv.Clear();
                mmkv.Set("test", 123, 1);
                Assert.That(mmkv.GetInt32("test"), Is.EqualTo(123));
                System.Threading.Thread.Sleep(2000);
                Assert.That(mmkv.ContainsKey("test"), Is.False);
                Assert.Catch(() => mmkv.GetInt32("test"));
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

        [Test]
        public void RemoveAll()
        {
            using (var mmkv = Mmkv.Default(MmkvMode.SingleProcess))
            {
                mmkv.Clear();
                mmkv.Set("1", 1);
                mmkv.Set("2", 2);
                mmkv.Set("3", 3);
                mmkv.Set("4", 4);
                mmkv.Set("5", 5);
                mmkv.RemoveAll(new string[] { "1", "2" });
                Assert.Multiple(() =>
                {
                    Assert.That(mmkv.ContainsKey("1"), Is.False);
                    Assert.That(mmkv.ContainsKey("2"), Is.False);
                    Assert.That(mmkv.ContainsKey("3"), Is.True);
                    Assert.That(mmkv.ContainsKey("4"), Is.True);
                    Assert.That(mmkv.ContainsKey("5"), Is.True);
                });
                mmkv.RemoveAll(new List<string> { "3", "4" });
                Assert.Multiple(() =>
                {
                    Assert.That(mmkv.ContainsKey("3"), Is.False);
                    Assert.That(mmkv.ContainsKey("4"), Is.False);
                    Assert.That(mmkv.ContainsKey("5"), Is.True);
                });
            }
        }

        [Test]
        public void StringArray()
        {
            using (var mmkv = Mmkv.Default(MmkvMode.SingleProcess))
            {
                mmkv.Clear();
                mmkv.Set("strings", new String[] { "a", "b", "c" });
                Assert.Multiple(() =>
                {
                    Assert.That(mmkv.GetStringArray("strings"), Is.EquivalentTo(new String[] { "a", "b", "c" }));
                    Assert.That(mmkv.GetStringArrayOrDefault("strings"), Is.EquivalentTo(new String[] { "a", "b", "c" }));
                    Assert.That(mmkv.TryGetStringArray("strings", out var strings), Is.True);
                    Assert.That(strings, Is.EquivalentTo(new String[] { "a", "b", "c" }));

                    Assert.Catch(() => mmkv.GetStringArray("strings1"));
                    Assert.That(mmkv.GetStringArrayOrDefault("strings1"), Is.Null);
                    Assert.That(mmkv.GetStringArrayOrDefault("strings1", new String[] { "" }), Is.EquivalentTo(new String[] { "" }));
                    Assert.That(mmkv.TryGetStringArray("strings1", out _), Is.False);
                });
            }
        }
    }
}