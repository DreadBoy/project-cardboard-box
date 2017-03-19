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
        game.OnCommandReceived(new List<Command>() { new Command(Action.GAMEOVER) });
        //gameUiBehaviour.OnHandReceived(new List<Chip>() {
        //    new Chip("Action:MOVE"),
        //    new Chip("Number:5"),
        //    new Chip("Action:TURN"),
        //    new Chip("Number:1"),
        //    new Chip("Action:TURN"),
        //    new Chip("Number:3"),
        //    new Chip("Action:MOVE"),
        //    new Chip("Number:9")
        //});

    }

    void Update()
    {

    }

    public void SendTestCommands()
    {
        networkBehaviour.SendCommands(new Command[] { new Command("MOVE:2"), new Command("TURN:2") });
    }
}
