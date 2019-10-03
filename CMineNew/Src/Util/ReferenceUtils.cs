using System;
using CMineNew.Map.BlockData;

namespace CMineNew.Util{
    public static class ReferenceUtils{
        
        public static readonly WeakReference<Block> EmptyWeakBlockReference = new WeakReference<Block>(null);
        
    }
}