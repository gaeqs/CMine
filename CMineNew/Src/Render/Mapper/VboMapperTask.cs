namespace CMineNew.Render.Mapper{
    public class VboMapperTask<TKey>{
        public VboMapperTask(VboMapperTaskType type, TKey key, float[] data, int offset) {
            Type = type;
            Key = key;
            Data = data;
            Offset = offset;
        }

        public VboMapperTaskType Type { get; }

        public TKey Key { get; }

        public float[] Data { get; }

        public int Offset { get; }
    }

    public enum VboMapperTaskType{
        Add,
        Edit,
        Remove
    }
}