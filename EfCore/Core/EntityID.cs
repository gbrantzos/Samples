using System.Collections.Generic;

namespace EfCore.Core
{
    /// <summary>
    /// Generic class to represent an Entity ID, persistence agnostic.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class BaseEntityID<TKey> : ValueObject
    {
        public TKey ID { get; protected set; }

        public BaseEntityID() => ID = default;

        public override int GetHashCode() => ID.GetHashCode();
        public override string ToString() => $"{GetType().Name}, ID: {ID.ToString()}";

        /// <summary>
        /// Create <see cref="BaseEntityID{TKey}"/> from given value.
        /// </summary>
        /// <typeparam name="TEntityID"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TEntityID FromValue<TEntityID>(TKey key) where TEntityID : BaseEntityID<TKey>, new()
            => new TEntityID { ID = key };

        /// <summary>
        /// Create new <see cref="BaseEntityID{TKey}"/> instance, of type <typeparamref name="TEntityID"/>.
        /// </summary>
        /// <typeparam name="TEntityID"></typeparam>
        /// <returns></returns>
        public static TEntityID CreateNew<TEntityID>() where TEntityID : BaseEntityID<TKey>, new()
            => new TEntityID();

        protected override IEnumerable<object> GetAtomicValues() { yield return ID; }
    }

    /// <summary>
    /// <see cref="BaseEntityID{TKey}"/> for current project, using <see cref="int"/> for ID storage.
    /// </summary>
    public abstract class EntityID : BaseEntityID<int> { }
}
