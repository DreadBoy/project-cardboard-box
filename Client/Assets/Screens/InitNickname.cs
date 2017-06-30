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

        // Special handling because this screen is first one
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPreferences.Nickname)))
            GoForwardImmediately(transitionTo[0]);
        else
            rect.anchoredPosition = Vector2.zero;

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPreferences.Nickname)))
            input.text = PlayerPrefs.GetString(PlayerPreferences.Nickname);
    }

    public override void Update()
    {
        base.Update();
        confirm.interactable = !string.IsNullOrEmpty(input.text);
    }

    public override void OnEnter(ScreenBehaviour from)
    {
        base.OnEnter(from);

        if (from == null)
            return;

        if (from.GetComponent<GameLobby>() == null)
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPreferences.Nickname)))
                GoForwardImmediately(transitionTo[0]);
    }

    public void Skip()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPreferences.Nickname)))
            input.text = PlayerPrefs.GetString(PlayerPreferences.Nickname);
        GoForward();
    }

    public void Confirm()
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            PlayerPrefs.SetString(PlayerPreferences.Nickname, input.text);
            FindObjectOfType<NetworkBehaviour>().SendNickname(input.text);
        }
        GoForward();
    }
}