using System;
using System.Collections.Generic;

namespace CMineNew.DataStructure{
    /// <summary>
    /// Represents a collection. This is the root interfaces for all "E" data structures.
    /// </summary>
    /// <typeparam name="TE">The type of the elements stored in the collection.</typeparam>
    public interface IECollection<TE> : IEnumerable<TE>{
        
        /// <summary>
        /// Adds an element to the collection.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>The the collection can't store the same element twice, returns whether the element was stored.
        /// Else returns whether the internal structure of the collection was modified.</returns>
        bool Add(TE elem);

        /// <summary>
        /// Removes an element from the collection.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>Whether the element was removed.</returns>
        bool Remove(TE elem);

        /// <summary>
        /// Returns whether an element is inside the collection.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>Whether an element is inside the collection.</returns>
        bool Contains(TE elem);

        /// <summary>
        /// Returns whether the collection is empty.
        /// </summary>
        /// <returns>Whether the collection is empty.</returns>
        bool IsEmpty();

        /// <summary>
        /// Returns the number of elements in the collection.
        /// </summary>
        /// <returns>The number of elements.</returns>
        int Size();

        /// <summary>
        /// Clears the element.
        /// </summary>
        void Clear();

        /// <summary>
        /// Creates a copy of the collection in form of an array.
        /// </summary>
        /// <returns></returns>
        TE[] ToArray();

        /// <summary>
        /// Iterates the collection and executes the given action for each element.
        /// </summary>
        /// <param name="action">The action for perform.</param>
        void ForEach(Action<TE> action);
    }
}