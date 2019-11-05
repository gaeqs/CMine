namespace CMineNew.Color{
    public struct Rgba32I{
        //R, G, B, A
        private uint _value;

        public Rgba32I(byte r, byte g, byte b, byte a) {
            _value = ((uint) r << 24) | ((uint) g << 16) | ((uint) b << 8) | a;
        }

        public uint Value => _value;

        public float ValueF {
            get {
                unsafe {
                    var v = _value;
                    return *(float*) &v;
                }
            }
        }

        public byte R => (byte) (_value >> 24);

        public byte G => (byte) ((_value >> 16) & 0xFF);

        public byte B => (byte) ((_value >> 8) & 0xFF);

        public byte A => (byte) (_value & 0xFF);
        
        public override string ToString() {
            return _value.ToString("X");
        }
    }
}