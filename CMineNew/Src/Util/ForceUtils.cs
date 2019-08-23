namespace CMineNew.Util{
    public static class ForceUtils{
        public static float CalculateForceFromDamping(float dampingConstant, float velocity) {
            return dampingConstant * velocity;
        }
    }
}