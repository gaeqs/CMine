using CMineNew.DataStructure.List;

namespace CMineNew.DataStructure.Queue{
    
    /// <summary>
    /// Represents a queue os elements.
    /// All EQueues can be used as a list.
    /// </summary>
    /// <typeparam name="TE">The type of the elements stored in the queue.</typeparam>
    public interface IEQueue<TE> : IEList<TE>{
        
        /// <summary>
        /// Adds an elements to the queue.
        /// </summary>
        /// <param name="elem">The element.</param>
        void Push(TE elem);

        /// <summary>
        /// Removes and returns the first element of the queue.
        /// Throws an exception if the queue is empty.
        /// </summary>
        /// <returns>The element</returns>
        TE Pop();

        /// <summary>
        /// Returns but does not removes the first element of the queue.
        /// Instead of the method Element(), Peek() returns null if the queue is empty.
        /// </summary>
        /// <returns>The element, or null if the queue is empty.</returns>
        TE Peek();

        /// <summary>
        /// Returns but does not removes the first element of the queue.
        /// Instead of the method Peek(), Element() throws an exception if the queue is empty.
        /// </summary>
        /// <returns>The element, or null if the queue is empty.</returns>
        TE Element();
        
    }
}