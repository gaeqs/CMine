namespace CMineNew.DataStructure.List{
    /// <summary>
    /// Represents a list.
    /// In a list each element is assigned to an index that defines an order.
    /// This implementations allows to add, set and remove elements by its index.
    /// </summary>
    /// <typeparam name="TE">The type of the elements stored in the list.</typeparam>
    public interface IEList<TE> : IECollection<TE>{
        
        /// <summary>
        /// Returns the element stored in the given index, or throws IndexOutOfBoundException if the index is out of bound.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The element.</returns>
        TE Get(int index);

        /// <summary>
        /// Sets the given element in the given index, or throws IndexOutOfBoundException if the index is out of bound.
        /// </summary>
        /// <param name="elem">The element to set.</param>
        /// <param name="index">The index.</param>
        /// <returns>The element previously stored in the given index.</returns>
        TE Set(TE elem, int index);

        /// <summary>
        /// Adds an element in the given index and shifts to the right all elements whose index is greater than
        /// the added one, or throws IndexOutOfBoundException if the index is out of bound.
        /// </summary>
        /// <param name="elem">The element to add.</param>
        /// <param name="index">The index.</param>
        /// <returns>Whether the internal structure of the list has been modified.</returns>
        bool Add(TE elem, int index);

        /// <summary>
        /// Removes the element at the given index, or throws IndexOutOfBoundException if the index is out of bound.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The removed element.</returns>
        TE RemoveFromIndex(int index);

        /// <summary>
        /// Removes the last element of the list, or throws IndexOutOfBoundException if the list is empty.
        /// </summary>
        /// <returns>The removed element.</returns>
        TE RemoveLast();
    }
}