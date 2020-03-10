using System;
using IdGen;

namespace ContactManager.Utilities
{
    public static class KeyGenerator
    {
        private static readonly IdGenerator idGen = new IdGenerator(0);

        public static TKey CreateKey<TKey>() where TKey : struct
        {
            switch (typeof(TKey))
            {
                case var cls when cls == typeof(long):
                    return (TKey)(object)idGen.CreateId();
                default:
                    throw new NotSupportedException($"Cannot generate key of type '{typeof(TKey).Name}'");
            }
        }
    }
}
