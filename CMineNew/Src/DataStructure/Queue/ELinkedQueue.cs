using CMineNew.DataStructure.List;

namespace CMineNew.DataStructure.Queue{
    public class ELinkedQueue<TE> : ELinkedList<TE>, IEQueue<TE>{
        public void Push(TE elem) {
            Add(elem);
        }

        public TE Pop() {
            return RemoveFirst();
        }

        public TE Peek() {
            return _size == 0 ? default : _first._value;
        }

        public TE Element() {
            return First;
        }
    }
}