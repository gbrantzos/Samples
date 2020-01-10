using System;
using IdGen;

namespace Expenses.Domain
{
    public static class KeyGenerator
    {
        private static readonly DateTimeOffset DateStart2019 = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private static readonly IdGenerator idGenerator = new IdGenerator(0, DateStart2019);

        public static long NewId() => idGenerator.CreateId();
    }
}
