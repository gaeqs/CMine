using CMine.DataStructure.List;

namespace CMine.DataStructure.Queue{
    public interface IEQueue<TE> : IEList<TE>{
        
        void Push(TE elem);

        TE Pop();

        TE Peek();

        TE Element();
        
    }
}