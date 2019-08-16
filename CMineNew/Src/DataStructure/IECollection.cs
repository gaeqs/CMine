using System;
using System.Collections.Generic;

namespace CMine.DataStructure{
    public interface IECollection<TE> : IEnumerable<TE>{
        bool Add(TE elem);

        bool Remove(TE elem);

        bool Contains(TE elem);

        bool IsEmpty();

        int Size();

        void Clear();

        TE[] ToArray();

        void ForEach(Action<TE> action);
    }
}