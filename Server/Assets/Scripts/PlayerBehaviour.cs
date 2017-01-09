using Cardboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    public GameBehaviour game;
    public GridBehaviour grid;

    float speed = 2f;
    float rotationspeed = 0.5f;

    int x, y;
    public Vector3 position
    {
        get
        {
            return new Vector3(x, 0, y);
        }
        set
        {
            x = (int)value.x;
            y = (int)value.z;
        }
    }
    public bool IsOnSpot(int x, int y)
    {
        return this.x == x && this.y == y;
    }

    float offsetx, offsety;
    public Vector3 offset
    {
        get
        {
            return new Vector3(offsetx, 0, offsety);
        }
        private set { }
    }

    //multiple of 90
    int angle;
    public Quaternion rotation
    {
        get
        {
            return Quaternion.Euler(0, angle, 0);
        }
        private set { }
    }

    float offsetangle;
    public Quaternion offsetrotation
    {
        get
        {
            return Quaternion.Euler(0, offsetangle * 90, 0);
        }
        private set { }
    }

    int offsetSign;

    public enum State
    {
        waiting,
        ready,
        ingame
    }
    public State state = State.waiting;

    bool runningCommand = false;
    Queue<Command> commandQueue = new Queue<Command>();

    void Awake()
    {
        grid = FindObjectOfType<GridBehaviour>();
        game = FindObjectOfType<GameBehaviour>();
    }

    void Start()
    {
    }

    void UpdateOffset(float deltaTime)
    {
        if (offsetx * offsetSign < 0)
            offsetx += offsetSign * deltaTime * speed;

        if (offsety * offsetSign < 0)
        {
            offsety += offsetSign * deltaTime * speed;
            Debug.Log(offsety);
        }

        if (offsetangle * offsetSign > 0)
            offsetangle += offsetSign * deltaTime * rotationspeed;

        if (offsetx * offsetSign <= 0 && offsety * offsetSign <= 0 && offsetangle * offsetSign <= 0 && runningCommand == true)
            runningCommand = false;

        if (!runningCommand && commandQueue.Count > 0)
            ReceiveCommand(commandQueue.Dequeue());
    }

    void Update()
    {
        if (state == State.ingame)
        {
            UpdateOffset(Time.deltaTime);

            transform.position = grid.FromPlayerPosition(position, offset);
            transform.rotation = rotation * offsetrotation;
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

    public bool MovePlayer(int number)
    {
        int x = 0, y = 0;

        var looking = (angle / 90) % 90;
        //looking up
        if (looking == 0)
        {
            y = number;
            offsetSign = 1;
        }
        if (looking == 1)
        {
            x = number;
            offsetSign = 1;
        }
        if (looking == 2)
        {
            y = number;
            offsetSign = -1;
        }
        if (looking == 3)
        {
            x = number;
            offsetSign = -1;
        }

        var newx = this.x + x * offsetSign;
        var newy = this.y + y * offsetSign;

        if (newx < 0 || newx >= game.gridSize)
            return false;
        if (newy < 0 || newy >= game.gridSize)
            return false;

        this.x = newx;
        this.y = newy;

        offsetx = -x * offsetSign;
        offsety = -y * offsetSign;

        runningCommand = true;

        return true;
    }

    public void RotatePlayer(int quarter)
    {
        angle += quarter * 90;

        offsetangle = -quarter;

        offsetSign = 1;

        runningCommand = true;
    }
}
