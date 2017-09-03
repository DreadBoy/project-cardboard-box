using System;

namespace ProjectCardboardBox
{
    public class DoAfterTimeout
    {
        float counter, timeout;
        System.Action action;
        bool done = false;
        public DoAfterTimeout(float seconds, System.Action action)
        {
            counter = 0;
            timeout = seconds;
            this.action = action;
        }

        public void Update(float deltaTime)
        {
            counter += deltaTime;
            if (counter >= timeout && !done)
            {
                done = true;
                action.Invoke();
            }
        }
    }
}
