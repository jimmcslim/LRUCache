# LRUCache

## Overview

This is a solution to the Fone Dynamics Backend Development Coding Challenge.

It is an implementation of a cache that implements a Least Recently Used (LRU) eviction policy.
The policy is only enforced when the cache is updated via the `AddOrUpdate` method; i.e. there are 
no background threads periodically sweeping out data that hasn't been recently accessed. 

The solution `LRUCache.sln` contains two projects:

* `DataStructures.csproj`: this contains the `ICache` interface as specified by the 
 coding challenge, and a `Cache` class that implements this interface using the LRU policy.
 Both classes are in the `FoneDynamics.DataStructures` namespace. 
* `DataStructures.Tests.csproj`: the unit test suite for this project.
Code coverage is measured at **100%** using JetBrains dotCover.

Both projects use .NET Framework 4.7.1 as their target framework.

## `ICache<TKey, TValue>` Interface

The following interface is implemented:

````
public interface ICache<TKey, TValue>
{
    /// <summary>
    /// Adds the value to the cache against the specified key.
    /// If the key already exists, its value is updated.
    /// </summary>
    void AddOrUpdate(TKey key, TValue value);

    /// <summary>
    /// Attempts to get the value from the cache against the specified key
    /// and returns true if the key exists in the cache.
    /// </summary>
    bool TryGetValue(TKey key, out TValue value);
}
````

Both calls on this interface make `key` the most recently used of all
the keys stored by the cache.