using System;

namespace ProjectCardboardBox
{
    /// <summary>
    /// Helper class for easier lerping
    /// Usage: LerpHelper<Vector3> lerpPosition = new LerpHelper<Vector3>(transform.position, positions[index], Vector3.Lerp)
    /// Don't forget to call Update()
    /// </summary>
    public class LerpHelper<T>
    {
        public T from, to;
        public float progress = 0;
        public float distance = 1;
        public float speed = 1;
        Func<T, T, float, T> lerpFunction;

        public LerpHelper(T from, T to, Func<T, T, float, T> lerpFunction)
        {
            this.from = from;
            this.to = to;
            this.lerpFunction = lerpFunction;
        }
        /// <summary>
        /// Constructor for Helper
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="lerpFunction">Lerp function, eg. Vector3.Lerp, Quaternion.Lerp</param>
        /// <param name="speed"></param>
        /// <param name="distance">Usefull for maintaining constant speed of moving object</param>
        public LerpHelper(T from, T to, Func<T, T, float, T> lerpFunction, float speed = 1, float distance = 1)
            : this(from, to, lerpFunction)
        {
            this.speed = speed;
            this.distance = distance;
        }

        public void Update(float deltaTime)
        {
            progress += deltaTime * speed / distance;
        }

        public T Lerp()
        {
            if (progress >= 1)
                return to;

            return lerpFunction(from, to, progress);
        }

        public bool IsDone()
        {
            return progress >= 1;
        }
    }
}
