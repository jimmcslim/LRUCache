using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FoneDynamics.DataStructures.Tests
{
    [TestFixture]
    public class CacheTests
    {
        [Test]
        public void Can_Add_And_Retrieve_Values_By_Key()
        {
            var cache = new Cache<string, int>(10);
            cache.AddOrUpdate("one", 1);
            cache.AddOrUpdate("two", 2);

            var result = cache.TryGetValue("one", out var value);
            Assert.That(result, Is.True);
            Assert.That(value, Is.EqualTo(1));

            result = cache.TryGetValue("two", out value);
            Assert.That(result, Is.True);
            Assert.That(value, Is.EqualTo(2));
        }

        [Test]
        public void Cache_Can_Store_Null_Value_For_Key()
        {
            var cache = new Cache<string, string>(10);
            cache.AddOrUpdate("one", null);

            var result = cache.TryGetValue("one", out var value);
            Assert.That(result, Is.True);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void Can_Update_Existing_Values()
        {
            var cache = new Cache<string, int>(10);
            cache.AddOrUpdate("one", 1);
            cache.AddOrUpdate("one", 2);

            var result = cache.TryGetValue("one", out var value);
            Assert.That(result, Is.True);
            Assert.That(value, Is.EqualTo(2));
        }

        [Test]
        public void Indicates_If_Key_Doesnt_Exist()
        {
            var cache = new Cache<string, int>(10);
            var result = cache.TryGetValue("a key that doesn't exist", out var value);
            Assert.That(result, Is.False);
            Assert.That(value, Is.EqualTo(default(int)));
        }

        [Test]
        public void Least_Recently_Inserted_Key_Is_Removed_On_AddOrUpdate()
        {
            var cache = new Cache<string, string>(2);

            cache.AddOrUpdate("apple", "first");
            cache.AddOrUpdate("banana", "second");
            cache.AddOrUpdate("pear", "third");

            var result = cache.TryGetValue("apple", out var value);
            Assert.That(result, Is.False);
            Assert.That(value, Is.EqualTo(null));
        }

        [Test]
        public void Least_Recently_Accessed_Key_Is_Removed_on_AddOrUpdate()
        {
            var cache = new Cache<string, string>(2);

            cache.AddOrUpdate("apple", "first");
            cache.AddOrUpdate("banana", "second");

            cache.TryGetValue("apple", out var value);
            cache.TryGetValue("banana", out value);
            cache.TryGetValue("apple", out value);

            cache.AddOrUpdate("pear", "third");

            var result = cache.TryGetValue("banana", out value);
            Assert.That(result, Is.False);
            Assert.That(value, Is.EqualTo(null));
        }

        [Test]
        public void Rejects_Cache_Size_Of_Zero_Or_Negative()
        {
            var e = Assert.Throws<ArgumentOutOfRangeException>(() => { new Cache<string, string>(0); });
            Assert.That(e.Message, Is.EqualTo("maximumSize must be 1 or greater.\r\nParameter name: maximumSize\r\nActual value was 0."));
            e = Assert.Throws<ArgumentOutOfRangeException>(() => { new Cache<string, string>(-1); });
            Assert.That(e.Message, Is.EqualTo("maximumSize must be 1 or greater.\r\nParameter name: maximumSize\r\nActual value was -1."));
        }

        [Test]
        public void Rejects_Null_Key()
        {
            var cache = new Cache<string, string>(10);

            var e = Assert.Throws<ArgumentNullException>(() => cache.AddOrUpdate(null, "value"));
            Assert.That(e.Message, Is.EqualTo("key cannot be null.\r\nParameter name: key"));

            e = Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(null, out var value));
            Assert.That(e.Message, Is.EqualTo("key cannot be null.\r\nParameter name: key"));
        }

        // It is hard to write a unit test that 100% guarantees thread safety, however
        // this code sufficiently exercises the system under test with multiple tasks
        // that update the cache, sleep, and then attempt to retrieve from the cache.
        // The total number of cache misses is compared against the size of the cache
        // and the number of tasks that are run to ensure that we are seeing the 
        // correct behaviour.
        [Test]
        public async Task Cache_Is_Thread_Safe()
        {
            const int cacheSize = 80;
            const int numberOfTasks = 100;

            Assert.That(cacheSize, Is.LessThanOrEqualTo(numberOfTasks),
                "To check for expected cache misses, cache size must be less than the number of tasks");

            var cache = new Cache<int, string>(cacheSize);
            var tasks = new List<Task<bool>>();
            var random = new Random();

            for (var i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(UpdateCacheAndAttemptRetrieval(cache, i, random.Next(100, 1000)));
            }

            await Task.WhenAll(tasks);

            var numberOfCacheMisses = tasks.Count(x => x.Result == false);

            Assert.That(numberOfCacheMisses, Is.EqualTo(numberOfTasks - cacheSize), "Unexpected number of cache misses");
        }

        private async Task<bool> UpdateCacheAndAttemptRetrieval(ICache<int, string> cache, int key, int pauseInMilliseconds)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(pauseInMilliseconds));
            var expectedValue = key.ToString();
            Console.WriteLine($"Updating {key}...");
            cache.AddOrUpdate(key, expectedValue);
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            Console.WriteLine($"Retrieving key {key}...");
            var result = cache.TryGetValue(key, out var actualValue);
            if (result)
            {
                Console.WriteLine($"Cache hit on {key}.");
                Assert.That(actualValue, Is.EqualTo(expectedValue));
            }
            else
            {
                Console.WriteLine($"Cache missed on {key}.");
            }
            return result;
        }
    }
}