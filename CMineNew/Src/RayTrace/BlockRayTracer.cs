using System;
using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData;
using OpenTK;

namespace CMineNew.RayTrace{
    public class BlockRayTracer{
        private readonly World _world;

        private Vector3 _origin;
        private Vector3 _direction;
        private Vector3 _current;
        private float _maximumDistance, _maximumDistanceSquared;
        private Vector3i _currentBlock;
        private Block _result;
        private BlockFace _face;
        private bool _finished;
        private List<string> _bypassBlocks;

        public BlockRayTracer(World world, Vector3 origin, Vector3 direction, float maximumDistance) {
            _world = world;
            _origin = origin;
            _current = origin;
            _direction = direction.Normalized();
            _maximumDistance = maximumDistance;
            _maximumDistanceSquared = maximumDistance * maximumDistance;
            _currentBlock = new Vector3i(_current, true);
            _result = null;
            _finished = false;
            _face = BlockFace.Up;
            _bypassBlocks = new List<string> {"default:air", "default:water"};
        }

        public World World => _world;

        public Vector3 Origin => _origin;

        public Vector3 Direction => _direction;

        public Vector3 Current => _current;

        public float MaximumDistance {
            get => _maximumDistance;
            set {
                _maximumDistance = value;
                _maximumDistanceSquared = _maximumDistance * _maximumDistance;
            }
        }

        public float MaximumDistanceSquared => _maximumDistanceSquared;

        public Block Result => _result;

        public BlockFace Face => _face;

        public List<string> BypassBlocks {
            get => _bypassBlocks;
            set => _bypassBlocks = value;
        }

        public void Reset(Vector3 origin, Vector3 direction) {
            _origin = origin;
            _current = origin;
            _currentBlock = new Vector3i(_current, true);
            _direction = direction.Normalized();
            _result = null;
            _finished = false;
            _face = BlockFace.Up;
        }

        public void Run() {
            while (!_finished) {
                Step();
            }
        }

        public void Step() {
            var block = _world.GetBlock(_currentBlock);
            if (block != null && !_bypassBlocks.Contains(block.Id)) {
                if (block.Collides(_face, _current, _origin, _direction, out _face, out _current)) {
                    _result = block;
                    _finished = true;
                    return;
                }
            }

            var distanceX = GetDistance(_direction.X, _current.X, _currentBlock.X);
            var distanceY = GetDistance(_direction.Y, _current.Y, _currentBlock.Y);
            var distanceZ = GetDistance(_direction.Z, _current.Z, _currentBlock.Z);

            if (distanceX < distanceY) {
                if (distanceX < distanceZ) {
                    MoveToX(distanceX);
                }
                else {
                    MoveToZ(distanceZ);
                }
            }
            else {
                if (distanceY < distanceZ) {
                    MoveToY(distanceY);
                }
                else {
                    MoveToZ(distanceZ);
                }
            }

            if (_maximumDistanceSquared < (_current - _origin).LengthSquared) _finished = true;
        }

        private void MoveToX(float distance) {
            _current += _direction * distance;
            if (_direction.X < 0) {
                _face = BlockFace.East;
                _currentBlock.X--;
            }
            else {
                _currentBlock.X++;
                _face = BlockFace.West;
            }
        }

        private void MoveToY(float distance) {
            _current += _direction * distance;
            if (_direction.Y < 0) {
                _currentBlock.Y--;
                _face = BlockFace.Up;
            }
            else {
                _currentBlock.Y++;
                _face = BlockFace.Down;
            }
        }

        private void MoveToZ(float distance) {
            _current += _direction * distance;
            if (_direction.Z < 0) {
                _currentBlock.Z--;
                _face = BlockFace.South;
            }
            else {
                _currentBlock.Z++;
                _face = BlockFace.North;
            }
        }

        private static float GetDistance(float direction, float current, int currentBlock) {
            if (Math.Abs(direction) < 0.0001f) {
                return float.MaxValue;
            }

            var next = currentBlock + (direction > 0 ? 1 : 0);
            return (next - current) / direction;
        }
    }
}