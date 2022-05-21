using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LRUCache
{
    public class LRUCache<T> : ILRUCache<T>
    {
        // A node of a double-linked list
        class Node
        {
            public string Key { get; set; }
            public T Value { get; set; }
            public Node Previous { get; set; }
            public Node Next { get; set; }

            public override string ToString() => $"Key {Key}";
        }

        private readonly ConcurrentDictionary<string, Node> _index;
        private readonly int _maxSize;
        private int _count;
        private Node _head;
        private Node _tail;

        public LRUCache(int maxSize = 30)
        {
            _count = 0;
            _maxSize = maxSize;
            _index = new ConcurrentDictionary<string, Node>();
        }

        public void Add(string key, T value)
        {
            // Check if we already have the value in cache
            if (_index.TryGetValue(key, out var _))
                return;

            // Create a new node 
            var node = new Node
            {
                Key = key,
                Value = value,
                Previous = null,
                Next = null,
            };


            // On an empty cache things are easy...
            if (_count == 0)
            {
                _head = node;
                _tail = node;
                _count = 1;
                _index.TryAdd(key, node);
                return;
            }

            // Put in front of head
            _head.Previous = node;
            node.Next = _head;

            // Adjust head and counter
            _head = node;
            _count++;

            // Add to lookup dictionary
            _index.TryAdd(key, node);

            // If we reached the maximum size, kickout last
            if (_count > _maxSize)
            {
                // Remove from lookup
                _index.Remove(_tail.Key, out var _);

                // Make sure we don't hold any reference
                _tail.Value = default;

                // Move tail one step behind and adjust
                _tail = _tail.Previous;
                _tail.Next = null;
                _count--;
            }
        }

        public T Get(string key)
        {
            if (!_index.TryGetValue(key, out var node))
                throw new KeyNotFoundException($"Key '{key}' not found in cache");

            // If we are not on head, we shall move item to head
            if (node != _head)
            {
                // Move to front
                node.Previous.Next = node.Next;
                if (node.Next is not null)
                    node.Next.Previous = node.Previous;

                // Link to old head
                node.Next = _head;

                // If picked item was tail, move a step back
                if (node == _tail)
                {
                    _tail = node.Previous;
                    _tail.Next = null;
                }

                // Make picked item head
                _head = node;
                node.Previous = null;
            }

            return node.Value;
        }

        public string DebugInfo()
        {
            if (_head is null)
                return "Empty";

            var node = _head;
            var sb = new StringBuilder();
            sb.Append($"HEAD: {_head} - TAIL: {_tail} > ");

            do
            {
                sb.Append($"{node.Key}");
                node = node.Next;
                if (node is null)
                    break;
                sb.Append(", ");
            } while (true);

            return sb.ToString();
        }
    }
}
