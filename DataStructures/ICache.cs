namespace FoneDynamics.DataStructures
{
    public interface ICache<TKey, TValue>
    {
        /// <summary>
        /// Adds the value to the cache against the specified key.
        /// If the key already exists, its value is updated.
        /// </summary>
        /// <param name="key">The key that should be added or updated in the cache.</param>
        /// <param name="value">The value that should be associated with the key in the cache.</param>
        void AddOrUpdate(TKey key, TValue value);

        /// <summary>
        /// Attempts to get the value from the cache against the specified key
        /// and returns true if the key exists in the cache.
        /// </summary>
        /// <param name="key">The key that should be used to lookup a value in the cache.</param>
        /// <param name="value">If there was a value in the cache for the key, will be populated
        /// with the value on return. Otherwise will be the default value of <see cref="TValue"/>.</param>
        /// <returns>true if the cache contained a value for the key, false if not.</returns>
        bool TryGetValue(TKey key, out TValue value);
    }
}