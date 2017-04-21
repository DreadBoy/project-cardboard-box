using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScreenBehaviour : MonoBehaviour
{
    protected RectTransform rect;
    LerpHelper<Vector2> moving;
    ScreenPosition screenPosition = ScreenPosition.Center;
    protected Vector2 positionLeft = new Vector2(-1280, 0);
    protected Vector2 positionRight = new Vector2(1280, 0);
    protected float speed = 1280 * 4;

    public List<ScreenBehaviour> transitionTo = new List<ScreenBehaviour>();
    public ScreenBehaviour transitionedFrom;

    public virtual void OnEnable()
    {
        rect = GetComponent<RectTransform>();
        rect.anchoredPosition = positionRight;
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        if (moving != null)
        {
            moving.Update(Time.deltaTime);
            rect.anchoredPosition = moving.Lerp();
            if (moving.IsDone())
                moving = null;
        }
    }

    public void ExitToLeft()
    {
        var from = rect.anchoredPosition;
        var to = positionLeft;
        moving = new LerpHelper<Vector2>(from, to, Vector2.Lerp, speed, Vector2.Distance(from, to));
        screenPosition = ScreenPosition.Left;
    }

    public void ExitToRight()
    {
        var from = rect.anchoredPosition;
        var to = positionRight;
        moving = new LerpHelper<Vector2>(from, to, Vector2.Lerp, speed, Vector2.Distance(from, to));
        screenPosition = ScreenPosition.Left;
    }

    public void EnterFromLeft()
    {
        if (screenPosition == ScreenPosition.Right)
            rect.anchoredPosition = positionLeft;

        var from = rect.anchoredPosition;
        var to = new Vector2(0, 0);
        moving = new LerpHelper<Vector2>(from, to, Vector2.Lerp, speed, Vector2.Distance(from, to));
        screenPosition = ScreenPosition.Center;
    }

    public void EnterFromRight()
    {
        if (screenPosition == ScreenPosition.Left)
            rect.anchoredPosition = positionRight;

        var from = rect.anchoredPosition;
        var to = new Vector2(0, 0);
        moving = new LerpHelper<Vector2>(from, to, Vector2.Lerp, speed, Vector2.Distance(from, to));
        screenPosition = ScreenPosition.Center;
    }

    public void GoForward()
    {
        if (transitionTo.Count != 1)
            throw new System.Exception("More than one screen available for transition");
        GoForward(transitionTo[0]);
    }

    public void GoForward(int index)
    {
        if (index > 0 && index < transitionTo.Count)
            GoForward(transitionTo[index]);
    }

    public void GoForward(ScreenBehaviour nextScreen)
    {
        ExitToLeft();
        nextScreen.EnterFromRight();
        nextScreen.transitionedFrom = this;
    }

    protected enum ScreenPosition
    {
        Left,
        Center,
        Right
    }
}