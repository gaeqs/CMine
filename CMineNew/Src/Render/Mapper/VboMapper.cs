using CMineNew.Render.Object;

namespace CMineNew.Render.Mapper{
    public interface VboMapper<TKey>{
        VertexBufferObject Vbo { get; set; }

        VertexArrayObject Vao { get; set; }

        int ElementSize { get; }

        int Amount { get; }


        int Updates { get; }

        bool OnBackground { get; set; }

        int GetPointer(TKey key);

        void AddTask(VboMapperTask<TKey> task);

        void FlushQueue();

        bool ContainsKey(TKey key);
    }
    
    public delegate void OnResize(VertexArrayObject obj, VertexBufferObject oldBuffer, VertexBufferObject newBuffer);
}