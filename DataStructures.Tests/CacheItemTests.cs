using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace FoneDynamics.DataStructures.Tests
{
    [TestFixture]
    public class CacheItemTests
    {
        [Test]
        public void Encapsulates_Value_And_Node_With_Key()
        {
            var node = new LinkedListNode<string>("key1");
            var item = new CacheItem<string, int>(node, 11);
            Assert.That(item.Node, Is.SameAs(node));
            Assert.That(item.Value, Is.EqualTo(11));

            node = new LinkedListNode<string>("key2");
            item = new CacheItem<string, int>(node, 22);
            Assert.That(item.Node, Is.SameAs(node));
            Assert.That(item.Value, Is.EqualTo(22));
        }

        [Test]
        public void Node_Can_Be_Updated()
        {
            var node1 = new LinkedListNode<string>("key1");
            var item = new CacheItem<string, int>(node1, 11);
            Assert.That(item.Node, Is.SameAs(node1));

            var node2 = new LinkedListNode<string>("key2");
            item.Node = node2;
            Assert.That(item.Node, Is.SameAs(node2));
        }

        [Test]
        public void Value_Can_Be_Updated()
        {
            var item = new CacheItem<string, int>(new LinkedListNode<string>("key1"), 11);
            Assert.That(item.Value, Is.EqualTo(11));

            item.Value = 22;
            Assert.That(item.Value, Is.EqualTo(22));
        }

        [Test]
        public void Rejects_Null_Node_Argument()
        {
            var e = Assert.Throws<ArgumentNullException>(() => new CacheItem<string, string>(null, "value"));
            Assert.That(e.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: node"));
        }
    }
}
