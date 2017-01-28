using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    public GameBehaviour game;
    public UIBehaviour uiBehaviour;
    public GameUIBehaviour gameUiBehaviour;
    public NetworkBehaviour networkBehaviour;

    void Start()
    {
        if (game == null)
            game = FindObjectOfType<GameBehaviour>();
        if (uiBehaviour == null)
            uiBehaviour = FindObjectOfType<UIBehaviour>();
        if (networkBehaviour == null)
            networkBehaviour = FindObjectOfType<NetworkBehaviour>();
        if (gameUiBehaviour == null)
            gameUiBehaviour = FindObjectOfType<GameUIBehaviour>();

        game.state = GameBehaviour.State.game;
        uiBehaviour.ChangeState(game.state);
        gameUiBehaviour.OnHandReceived(new List<Chip>() {
            new Chip("Action:MOVE"),
            new Chip("Number:1"),
            new Chip("Action:MOVE"),
            new Chip("Number:1"),
            new Chip("Action:MOVE"),
            new Chip("Number:1"),
            new Chip("Action:MOVE")
        });

    }

    void Update()
    {

    }

    public void SendTestCommands()
    {
        networkBehaviour.SendCommands(new Command[] { new Command("MOVE:2"), new Command("TURN:2") });
    }
}
