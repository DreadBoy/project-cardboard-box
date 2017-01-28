using ProjectCardboardBox;
using Enum = System.Enum;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerBehaviour : MonoBehaviour
{

    public GameBehaviour game;
    public GridBehaviour grid;

    float speed = 2f;
    float rotationspeed = 0.5f;

    int x, y;
    public bool IsOnSpot(int x, int y)
    {
        return this.x == x && this.y == y;
    }
    int angle = 0;

    public enum State
    {
        waiting,
        ready,
        ingame
    }
    public State state = State.waiting;

    bool runningCommand = false;
    Queue<Command> commandQueue = new Queue<Command>();
    LerpHelper<Vector3> lerpPosition = null;
    LerpHelper<Quaternion> lerpRotation = null;

    public SmartEvent<ChipsArgs> chipsEvent = new SmartEvent<ChipsArgs>();

    void Awake()
    {
        grid = FindObjectOfType<GridBehaviour>();
        game = FindObjectOfType<GameBehaviour>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (state == State.ingame)
        {
            if (!runningCommand && commandQueue.Count > 0)
                ReceiveCommand(commandQueue.Dequeue());

            if (lerpPosition != null)
            {
                lerpPosition.Update(Time.deltaTime);
                transform.localPosition = lerpPosition.Lerp();
                if (lerpPosition.IsDone())
                {
                    lerpPosition = null;
                    runningCommand = false;
                }
            }

            if (lerpRotation != null)
            {
                lerpRotation.Update(Time.deltaTime);
                transform.localRotation = lerpRotation.Lerp();
                if (lerpRotation.IsDone())
                {
                    lerpRotation = null;
                    runningCommand = false;
                }
            }
        }
    }


    public void ReceiveCommand(Command command)
    {
        if (runningCommand)
        {
            commandQueue.Enqueue(command);
            return;
        }

        if (command.type == Action.MOVE && state == State.ingame)
        {
            MovePlayer(command.number);
        }
        else if (command.type == Action.TURN && state == State.ingame)
        {
            RotatePlayer(command.number);
        }
        else if (command.type == Action.READY && state == State.waiting)
        {
            state = State.ready;
        }
        else if(command.type == Action.REQUESTCHIPS)
        {
            //NOTE You can generate chips based on current situation
            chipsEvent.RaiseEvent(new ChipsArgs(GenerateChips(command.number), this));
        }
    }

    List<Chip> GenerateChips(int number)
    {
        List<Chip> chips = new List<Chip>();
        int numNumber = 0, numAction = 0;
        for (int i = 0; i < number; i++)
        {
            var type = Random.Range(0, 2);
            if (type == 0)
            {
                // 1/3 change to generate action
                var action = (Action)Random.Range(0, Enum.GetValues(typeof(Action)).Cast<int>().Max() + 1);
                chips.Add(new Chip(Chip.Type.Action, action.ToString()));
                numNumber++;
            }
            else if (type == 1 ||type == 2)
            {
                // 2/3 change to generate number
                chips.Add(new Chip(Chip.Type.Number, Random.Range(0, 10).ToString()));
                numAction++;
            }
        }
        if(numNumber == 0)
        {
            var first = chips.Where(c => c.type == Chip.Type.Action).First();
            var index = chips.IndexOf(first);
            chips[index] = new Chip(Chip.Type.Number, Random.Range(0, 10).ToString());
        }
        if(numAction == 0)
        {
            var first = chips.Where(c => c.type == Chip.Type.Number).First();
            var index = chips.IndexOf(first);
            chips[index] = new Chip(Chip.Type.Action, ((Action)Random.Range(0, Enum.GetValues(typeof(Action)).Cast<int>().Max() + 1)).ToString());
        }
        return chips;
    }


    public void SpawnPlayer(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.localPosition = grid.FromPlayerPosition(x, y);
        transform.localRotation = Quaternion.Euler(0, 90 * angle, 0);
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }

    public bool MovePlayer(int number)
    {
        int newx = x, newy = y;

        var looking = (angle % 360) / 90;
        //looking up
        if (looking == 0)
        {
            newy = y + number;
            newx = x;
        }
        if (looking == 1)
        {
            newx = x + number;
            newy = y;
        }
        if (looking == 2)
        {
            newy = y - number;
            newx = x;
        }
        if (looking == 3)
        {
            newx = x - number;
            newy = y;
        }


        if (newx < 0)
            newx = 0;
        if (newx >= game.gridSize)
            newx = game.gridSize - 1;
        if (newy < 0)
            newy = 0;
        if (newy >= game.gridSize)
            newy = game.gridSize - 1;

        x = newx;
        y = newy;

        lerpPosition = new LerpHelper<Vector3>(transform.localPosition, grid.FromPlayerPosition(newx, newy), Vector3.Lerp, speed);

        runningCommand = true;

        return true;
    }

    public void RotatePlayer(int quarter)
    {
        angle += quarter * 90;
        angle %= 360;
        lerpRotation = new LerpHelper<Quaternion>(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 90 * quarter, 0), Quaternion.Lerp, rotationspeed);

        runningCommand = true;
    }
}
