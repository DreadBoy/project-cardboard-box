using ProjectCardboardBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enum = System.Enum;

public class PlayerBehaviour : MonoBehaviour
{
    public GameBehaviour game;
    public GridBehaviour grid;
    HintBehaviour hintBehaviour;

    float speed = 3.5f;
    float rotationspeed = 1.5f;

    /// <summary>
    /// Is in degrees and always between 0 and 360
    /// </summary>
    public int angle { get; set; }

    /// <summary>
    /// Tells how much player should move ahead
    /// Changed to > 0 when receiving command
    /// If 0, player shouldn't move at all
    /// </summary>
    float distance { get; set; }

    public enum State
    {
        waiting,
        ready,
        ingame,
        dead,
        ending
    }
    public State state = State.waiting;

    bool runningCommand = false;
    Queue<Command> commandQueue = new Queue<Command>();
    LerpHelper<Quaternion> lerpRotation = null;
    LerpHelper<Vector3> lerpBounce = null;

    public SmartEvent<EndTurnArgs> EndTurn = new SmartEvent<EndTurnArgs>();

    public Animator animator;
    public Vector3 velocity = Vector3.zero;

    Material colour;

    void Awake()
    {
        grid = FindObjectOfType<GridBehaviour>();
        game = FindObjectOfType<GameBehaviour>();
        hintBehaviour = GetComponent<HintBehaviour>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void OnEnable()
    {
        colour = ColouredMaterial.Instance.GetNewColour();
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

            if (lerpBounce != null)
            {
                lerpBounce.Update(Time.deltaTime);
                transform.position = lerpBounce.Lerp();
                if (lerpBounce.IsDone())
                    StopBounce();
            }

            if (distance > 0)
            {
                transform.position += transform.forward * Time.deltaTime * speed;
                distance -= Time.deltaTime * speed;
                if (distance <= 0)
                {
                    StopMoving();
                    SnapToGrid();
                    if (commandQueue.Count == 0)
                        EndTurn.RaiseEvent(new EndTurnArgs(this));
                }
            }

            if (lerpRotation != null)
            {
                lerpRotation.Update(Time.deltaTime);
                transform.localRotation = lerpRotation.Lerp();
                if (lerpRotation.IsDone())
                {
                    StopRotating();
                    if (commandQueue.Count == 0)
                        EndTurn.RaiseEvent(new EndTurnArgs(this));
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
        else if (command.type == Action.NOTREADY && state == State.ready)
        {
            state = State.waiting;
        }
        else if (command.type == Action.REQUESTCHIPS)
        {
            Debug.LogError("Cient can't request chip any more! Outdated client?");
        }
    }

    public void ReceiveHint(string hint)
    {
        Debug.Log("Received hint " + hint);
        var hints = hint.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).Select(s => new Command(s)).ToArray();
        hintBehaviour.DisplayHint(transform.position, angle, grid, hints, colour);
    }

    public void CancelAndClearCommands()
    {
        commandQueue.Clear();
        StopMoving();
        StopRotating();
    }


    public void SpawnPlayer(Vector3 position)
    {
        transform.position = position;
        transform.localRotation = Quaternion.Euler(0, 90 * angle, 0);
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
        hintBehaviour.DestroyHint();
    }

    public bool MovePlayer(int number)
    {
        var target = Vector3.zero;

        if (angle % 90 == 0 || angle % 90 == 180)
            target = transform.position + transform.forward * number * grid.offset.z;
        else if (angle % 90 == 90 || angle % 90 == 270)
            target = transform.position + transform.forward * number * grid.offset.x;
        else //we are still rotating??
            return false;

        target = grid.SnapToGrid(target);
        distance = Vector3.Distance(target, transform.position);
        if (distance == 0)
            return false;

        //TODO Some static value?
        animator.SetBool("Moving", true);

        velocity = transform.forward * speed;
        runningCommand = true;

        return true;
    }

    void StopMoving()
    {
        distance = 0;
        runningCommand = false;
        velocity = Vector3.zero;
        animator.SetBool("Moving", false);
    }

    void StopBounce()
    {
        lerpBounce = null;
        runningCommand = false;
        velocity = Vector3.zero;
        animator.SetBool("Bouncing", false);

        Vector3 pos;
        if (grid.TrySnapToGrid(transform.position, out pos))
            return;
        // We are outside of grid
        Die();
    }

    public void RotatePlayer(int quarter)
    {
        quarter %= 4;

        // don't turn at all
        if (quarter == 0)
            return;

        angle += quarter * 90;
        angle %= 360;
        lerpRotation = new LerpHelper<Quaternion>(transform.localRotation, transform.localRotation * Quaternion.Euler(0, 90 * quarter, 0), Quaternion.Lerp, rotationspeed, quarter);
        animator.SetBool("Moving", true);

        runningCommand = true;
    }

    void StopRotating()
    {
        lerpRotation = null;
        runningCommand = false;
        velocity = Vector3.zero;
        animator.SetBool("Moving", false);
    }


    void SnapToGrid()
    {
        transform.position = grid.SnapToGrid(transform.position);
    }

    public void BounceIntoDirection(Vector3 direction)
    {
        CancelAndClearCommands();

        // perpendicular to bounce direction
        //var spreadVector = direction;
        //spreadVector.x = (int)Mathf.Abs(spreadVector.x);
        //spreadVector.z = (int)Mathf.Abs(spreadVector.z);
        //spreadVector.y = spreadVector.x;
        //spreadVector.x = spreadVector.z;
        //spreadVector.z = spreadVector.y;

        var length = Random.Range(1, 3);
        //var spread = Random.Range(-2, 2);
        //spread = 0;

        var landing = direction * length/* + spreadVector * spread*/;
        grid.TrySnapToGrid(transform.position + landing, out landing);


        lerpBounce = new LerpHelper<Vector3>(
            transform.position,
            landing,
            Vector3.Lerp,
            speed * 3,
            Vector3.Distance(landing, transform.position));
        velocity = (landing - transform.position).normalized;
        runningCommand = true;
        animator.SetBool("Bouncing", true);
    }

    void Die()
    {
        animator.SetTrigger("Dead");
        state = State.dead;
        game.playerDied(this);
    }
}
