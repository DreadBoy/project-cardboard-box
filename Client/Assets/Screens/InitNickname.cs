using UnityEngine;
using UnityEngine.UI;

public class InitNickname : ScreenBehaviour
{
    InputField input;
    Button confirm;

    public override void OnEnable()
    {
        base.OnEnable();
        input = GetComponentInChildren<InputField>();
        confirm = transform.Find("Panel/Confirm").GetComponent<Button>();
    }

    public override void Start()
    {
        base.Start();
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPreferences.Nickname)))
            GoForwardImmediately(transitionTo[0]);
        else
            rect.anchoredPosition = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();
        confirm.interactable = !string.IsNullOrEmpty(input.text);
        if (!string.IsNullOrEmpty(input.text))
            PlayerPrefs.SetString(PlayerPreferences.Nickname, input.text);
    }
}