using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SmartEvent<T> where T : EventArgs
{
    private readonly List<T> m_previousEventArgs = new List<T>();
    private EventHandler<T> m_event;

    public event EventHandler<T> Event
    {
        add
        {
            //Raise previous events for the caller
            foreach (T e in m_previousEventArgs)
            {
                value(this, e);
            }
            //Add the event handler
            m_event += value;
        }
        remove
        {
            m_event -= value;
        }
    }

    public void RaiseEvent(T e)
    {
        if (m_event != null)
        {
            m_event(this, e);
        }
        m_previousEventArgs.Add(e);
    }
}