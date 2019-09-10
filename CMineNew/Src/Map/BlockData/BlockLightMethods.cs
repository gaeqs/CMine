using System;
using System.Collections.Generic;
using System.Linq;
using CMineNew.DataStructure.List;
using CMineNew.DataStructure.Queue;

namespace CMineNew.Map.BlockData{
    public class BlockLightMethods{
        public static void ExpandFrom(Block from, BlockLightSource source, int light) {
            var queue = new Queue<Block>();
            var neighbours = from.Neighbours;
            for (var i = 0; i < neighbours.Length; i++) {
                var face = (BlockFace) i;
                if (!from.CanLightPassThrough(face)) continue;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var neighbour = neighbours[i];
                Expand(queue, neighbour, source, light, from, opposite);
            }

            UpdateRender(queue);
        }

        public static void Expand(Block to, BlockLightSource source, int light, Block from, BlockFace fromFace) {
            var queue = new Queue<Block>();
            Expand(queue, to, source, light, from, fromFace);
            UpdateRender(queue);
        }
        
        
        public static void RemoveLightSource(BlockLightSource source) {
            var queue = new Queue<Block>();
            var list = new ELinkedQueue<Block>();
            RemoveLight(list, source.Block, source);
            ExpandNearbyLights(queue, list);
            UpdateRender(queue);
        }

        private static void Expand(Queue<Block> updatedBlocks, Block to, BlockLightSource source,
            int light, Block from, BlockFace fromFace) {
            if (to == null || !to.CanLightBePassedFrom(fromFace, from)) return;
            var blockLight = to.BlockLight;
            if (blockLight.Light >= light) return;
            blockLight.Light = light;
            blockLight.Source = source;
            updatedBlocks?.Enqueue(to);

            var toLight = light - blockLight.LightPassReduction;
            var neighbours = to.Neighbours;

            for (var i = 0; i < neighbours.Length; i++) {
                var face = (BlockFace) i;
                if (!to.CanLightPassThrough(face)) continue;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var neighbour = neighbours[i];
                Expand(updatedBlocks, neighbour, source, toLight, to, opposite);
            }
        }

        private static void RemoveLight(ELinkedList<Block> removedBlocksList, Block block, BlockLightSource source) {
            var light = block.BlockLight;
            if (!source.Equals(light.Source)) return;
            light.Light = 0;
            light.Source = null;
            removedBlocksList.Add(block);

            var neighbours = block.Neighbours;
            for (var i = 0; i < neighbours.Length; i++) {
                var face = (BlockFace) i;
                if (!block.CanLightPassThrough(face)) continue;
                if(neighbours[i] == null) continue;
                RemoveLight(removedBlocksList, neighbours[i], source);
            }
        }

        private static void ExpandNearbyLights(Queue<Block> updateQueue, ELinkedList<Block> list) {
            var enumerator = (ELinkedList<Block>.ELinkedListEnumerator<Block>) list.GetEnumerator();
            while (enumerator.MoveNext()) {
                var elem = enumerator.Current;
                updateQueue.Enqueue(elem);

                if (elem == null || elem.Neighbours.All(n => n?.BlockLight.Source == null)) {
                    enumerator.Remove();
                }
            }

            enumerator.Reset();
            while (enumerator.MoveNext()) {
                var elem = enumerator.Current;
                if (elem == null) continue;
                var neighbours = elem.Neighbours;
                var light = 0;
                Block block = null;
                var face = BlockFace.Down;
                for (var i = 0; i < neighbours.Length; i++) {
                    var neighbour = neighbours[i];
                    if (neighbour == null) {
                        continue;
                    }
                    var bLight = neighbour.BlockLight;
                    var nLight = bLight.Light - bLight.LightPassReduction;
                    if (light >= nLight) continue;
                    light = nLight;
                    block = neighbour;
                    face = (BlockFace) i;
                }

                if (block == null) continue;
                Expand(null, elem, block.BlockLight.Source, light, block, face);
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