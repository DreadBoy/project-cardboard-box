using System;


class DoAfterTimeout
{
    float counter, timeout;
    Action action;
    bool done = false;
    public DoAfterTimeout(float seconds, Action action)
    {
        counter = 0;
        timeout = seconds;
        this.action = action;
    }

    public void Update(float deltaTime)
    {
        counter += deltaTime;
        if(counter >= timeout && !done)
        {
            done = true;
            action.Invoke();
        }
    }
}