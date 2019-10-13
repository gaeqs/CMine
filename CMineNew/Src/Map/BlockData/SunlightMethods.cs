using System.Collections.Generic;
using System.Linq;
using CMineNew.DataStructure.List;
using CMineNew.DataStructure.Queue;
using CMineNew.Geometry;

namespace CMineNew.Map.BlockData{
    public static class SunlightMethods{
        private const int Faces = 6;

        public static void ExpandFrom(Block from, Vector3i source, sbyte light) {
            if (light <= 0) return;
            var queue = new Queue<Block>();
            for (var i = 0; i < Faces; i++) {
                var face = (BlockFace) i;
                if (!from.CanLightPassThrough(face)) continue;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var neighbour = from.GetNeighbour((BlockFace) i);
                Expand(queue, neighbour, source, light, from, opposite);
            }

            UpdateRender(queue);
        }

        public static void Expand(Block to, Vector3i source, sbyte light, Block from, BlockFace fromFace) {
            var queue = new Queue<Block>();
            Expand(queue, to, source, light, from, fromFace);
            UpdateRender(queue);
        }


        public static void RemoveLightSource(Vector3i source, Block sourceBlock) {
            var queue = new Queue<Block>();
            var list = new ELinkedQueue<Block>();
            RemoveLight(list, sourceBlock, source);
            GetNearbyLights(queue, list);
            ExpandNearbyLights(list, null);
            UpdateRender(queue);
        }

        public static ELinkedList<Block> RemoveLightFromBlock(Block block, bool expand = true) {
            if (block.BlockLight.Sunlight == 0) return null;
            var queue = new Queue<Block>();
            var list = new ELinkedQueue<Block>();

            RemoveLight(list, block, block.BlockLight.SunlightSource);
            GetNearbyLights(queue, list);
            if (expand) {
                ExpandNearbyLights(list, null);
            }

            UpdateRender(queue);
            return list;
        }

        private static void Expand(Queue<Block> updatedBlocks, Block to, Vector3i source,
            sbyte light, Block from, BlockFace fromFace) {
            if (to == null || !to.CanLightBePassedFrom(fromFace, from)) return;
            var blockLight = to.BlockLight;
            if (blockLight.Sunlight >= light) return;
            blockLight.Sunlight = light;
            blockLight.SunlightSource = source;
            updatedBlocks?.Enqueue(to);

            var toLight = (sbyte) (light - to.StaticData.BlockLightPassReduction);

            for (var i = 0; i < Faces; i++) {
                var face = (BlockFace) i;
                if (!to.CanLightPassThrough(face)) continue;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var neighbour = to.GetNeighbour((BlockFace) i);
                if (neighbour == null) {
                    continue;
                }

                Expand(updatedBlocks, neighbour, source, toLight, to, opposite);
            }
        }

        private static void RemoveLight(ELinkedList<Block> removedBlocksList, Block block, Vector3i source) {
            var light = block.BlockLight;
            if (!source.Equals(light.SunlightSource)) return;

            var oldLight = Equals(block.Position, source) ? Block.MaxBlockLight : light.Sunlight;
            light.Sunlight = light.LinearSunlight;
            light.SunlightSource = block.Position;
            removedBlocksList.Add(block);

            for (var i = 0; i < Faces; i++) {
                var face = (BlockFace) i;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var neighbour = block.GetNeighbour((BlockFace) i);
                if (neighbour == null) {
                    continue;
                }

                if (!block.CanLightPassThrough(face) || !neighbour.CanLightBePassedFrom(opposite, block)) continue;
                if (oldLight <= neighbour.BlockLight.Sunlight) continue;
                RemoveLight(removedBlocksList, neighbour, source);
            }
        }

        private static void GetNearbyLights(Queue<Block> updateQueue, ELinkedList<Block> list) {
            var enumerator = (ELinkedList<Block>.ELinkedListEnumerator<Block>) list.GetEnumerator();
            while (enumerator.MoveNext()) {
                var elem = enumerator.Current;
                updateQueue.Enqueue(elem);

                if (elem == null ||
                    elem.NeighbourReferences.All(n => !n.TryGetTarget(out var v) || v.BlockLight.Sunlight <= 0)) {
                    enumerator.Remove();
                }
            }
        }

        public static void ExpandNearbyLights(ELinkedList<Block> list, Queue<Block> queue) {
            if (list == null) return;
            var enumerator = (ELinkedList<Block>.ELinkedListEnumerator<Block>) list.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext()) {
                var elem = enumerator.Current;
                if (elem == null) continue;
                var light = elem.CalculateSunlightFromNeighbours(out var face);
                if (light <= 0) continue;
                var block = elem.GetNeighbour(face);
                if (block != null) {
                    Expand(queue, elem, block.BlockLight.SunlightSource, light, block, face);
                }
            }
        }

        private static void UpdateRender(Queue<Block> queue) {
            while (queue.Count > 0) {
                var element = queue.Dequeue();
                element.TriggerLightChange();
            }
        }
    }
}