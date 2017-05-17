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

    public virtual void OnEnter(ScreenBehaviour from)
    {

    }

    public virtual void OnLeave(ScreenBehaviour to)
    {

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
        OnEnter(transitionedFrom);
    }

    public void EnterFromRight()
    {
        if (screenPosition == ScreenPosition.Left)
            rect.anchoredPosition = positionRight;

        var from = rect.anchoredPosition;
        var to = new Vector2(0, 0);
        moving = new LerpHelper<Vector2>(from, to, Vector2.Lerp, speed, Vector2.Distance(from, to));
        screenPosition = ScreenPosition.Center;
        OnEnter(transitionedFrom);
    }

    public void Hide()
    {
        GetComponent<RectTransform>().anchoredPosition = positionLeft;
        screenPosition = ScreenPosition.Left;
        if (moving != null)
            moving = null;
    }

    public void Show()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        screenPosition = ScreenPosition.Center;
        if (moving != null)
            moving = null;
        OnEnter(transitionedFrom);
    }

    public void GoForward()
    {
        if (transitionTo.Count != 1)
            throw new System.Exception("More than one screen available for transition");
        GoForward(transitionTo[0]);
    }

    public void GoForward(int index)
    {
        if (index >= 0 && index < transitionTo.Count)
            GoForward(transitionTo[index]);
    }

    public void GoForward(ScreenBehaviour nextScreen)
    {
        ExitToLeft();
        nextScreen.transitionedFrom = this;
        nextScreen.EnterFromRight();
        OnLeave(nextScreen);
    }

    public void GoForwardImmediately(ScreenBehaviour nextScreen)
    {
        Hide();
        nextScreen.transitionedFrom = this;
        nextScreen.Show();
        OnLeave(nextScreen);
    }

    protected enum ScreenPosition
    {
        Left,
        Center,
        Right
    }
}