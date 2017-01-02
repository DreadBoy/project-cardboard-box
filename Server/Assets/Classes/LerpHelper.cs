using System;

class LerpHelper<T>
{
    public T from, to;
    public float fraction = 0;
    public float speed = 1;
    Func<T, T, float, T> lerpFunction;

    public LerpHelper(T from, T to, Func<T, T, float, T> lerpFunction)
    {
        this.from = from;
        this.to = to;
        this.lerpFunction = lerpFunction;
    }
    public LerpHelper(T from, T to, Func<T, T, float, T> lerpFunction, float speed)
        :this(from, to, lerpFunction)
    {
        this.speed = speed;
    }

    public void Update(float deltaTime)
    {
        fraction += deltaTime * speed;
    }

    public T Lerp()
    {
        return lerpFunction(from, to, fraction);
    }

    public bool Done()
    {
        return fraction >= 1;
    }
}