using IdGen;

namespace Expenses.Domain
{
    public static class KeyGenerator
    {
        private static readonly IdGenerator idGenerator = new IdGenerator(0);

        public static long NewId() => idGenerator.CreateId();
    }
}
