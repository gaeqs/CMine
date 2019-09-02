namespace CMineNew.Util{
    public static class ForceUtils{
        
        /// <summary>
        /// Calculates the movement force required for a entity to move at a constant velocity, given
        /// the current damping constant applied to it.
        ///
        /// This force is calculated this way:
        /// Knowing that we want the sum of all forces to be 0 at a certain speed 'v':
        /// Let 'Fv' be the force of movement that we want, 'd' the damping constant, and 'Fd' the damping force, then
        /// Fv - Fd = 0, Fv - d * v = 0, Fv = d * v.
        ///
        /// Remember that the damping constant must be less than 0.
        ///
        /// </summary>
        /// <param name="dampingConstant">The damping constant.</param>
        /// <param name="velocity">The velocity we want the entity to move.</param>
        /// <returns>The required force.</returns>
        public static float CalculateForceFromDamping(float dampingConstant, float velocity) {
            return dampingConstant * velocity;
        }
    }
}