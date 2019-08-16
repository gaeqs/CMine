namespace CMine.DataStructure.List{
    public interface IEList<TE> : IECollection<TE>{
        TE Get(int index);

        TE Set(TE elem, int index);

        bool Add(TE elem, int index);

        TE RemoveFromIndex(int index);

        TE RemoveLast();
    }
}