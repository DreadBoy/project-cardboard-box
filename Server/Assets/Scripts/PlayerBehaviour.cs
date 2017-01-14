using System.Collections.Generic;
using UnityEngine;

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

        if (command.type == Command.Type.MOVE && state == State.ingame)
        {
            MovePlayer(command.number1 + command.number2);
        }
        if (command.type == Command.Type.TURN && state == State.ingame)
        {
            RotatePlayer(command.number1 + command.number2);
        }
        if (command.type == Command.Type.READY && state == State.waiting)
        {
            state = State.ready;
        }
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

        var looking = (angle / 90) % 90;
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


        if (newx < 0 || newx >= game.gridSize)
            return false;
        if (newy < 0 || newy >= game.gridSize)
            return false;

        x = newx;
        y = newy;

        lerpPosition = new LerpHelper<Vector3>(transform.localPosition, grid.FromPlayerPosition(newx, newy), Vector3.Lerp);

        runningCommand = true;

        return true;
    }

    public void RotatePlayer(int quarter)
    {
        angle += quarter * 90;
        lerpRotation = new LerpHelper<Quaternion>(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 90 * quarter, 0), Quaternion.Lerp);

        runningCommand = true;
    }
}
