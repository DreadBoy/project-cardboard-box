using ProjectCardboardBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enum = System.Enum;

public class PlayerBehaviour : MonoBehaviour
{
    public GameBehaviour game;
    public GridBehaviour grid;

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
        ingame
    }
    public State state = State.waiting;

    bool runningCommand = false;
    Queue<Command> commandQueue = new Queue<Command>();
    LerpHelper<Quaternion> lerpRotation = null;
    LerpHelper<Vector3> lerpBounce = null;

    public SmartEvent<ChipsArgs> chipsEvent = new SmartEvent<ChipsArgs>();

    public Animator animator;
    public Vector3 velocity = Vector3.zero;

    void Awake()
    {
        grid = FindObjectOfType<GridBehaviour>();
        game = FindObjectOfType<GameBehaviour>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
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
                }
            }

            if (lerpRotation != null)
            {
                lerpRotation.Update(Time.deltaTime);
                transform.localRotation = lerpRotation.Lerp();
                if (lerpRotation.IsDone())
                {
                    StopRotating();
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
        else if (command.type == Action.REQUESTCHIPS)
        {
            //NOTE You can generate chips based on current situation
            chipsEvent.RaiseEvent(new ChipsArgs(GenerateChips(command.number), this));
        }
    }

    public void CancelAndClearCommands()
    {
        commandQueue.Clear();
        StopMoving();
        StopRotating();
    }

    List<Chip> GenerateChips(int number)
    {
        List<Chip> chips = new List<Chip>();
        int numNumber = 0, numAction = 0;
        for (int i = 0; i < number; i++)
        {
            var type = Random.Range(0, 3);
            if (new int[] { 0 }.Contains(type))
            {
                // some chance to generate action
                var action = (Action)Random.Range(0, Enum.GetValues(typeof(Action)).Cast<int>().Max() + 1);
                chips.Add(new Chip(Chip.Type.Action, action.ToString()));
                numAction++;
            }
            else if (new int[] { 1, 2 }.Contains(type))
            {
                // some chance to generate number
                chips.Add(new Chip(Chip.Type.Number, Random.Range(0, 10).ToString()));
                numNumber++;
            }
        }
        if (numNumber == 0)
        {
            var act = chips.Where(c => c.type == Chip.Type.Action).ToList();
            var first = act.First();
            var index = chips.IndexOf(first);
            chips[index] = new Chip(Chip.Type.Number, Random.Range(0, 10).ToString());
        }
        if (numAction == 0)
        {
            var nums = chips.Where(c => c.type == Chip.Type.Number).ToList();
            var first = nums.First();
            var index = chips.IndexOf(first);
            chips[index] = new Chip(Chip.Type.Action, ((Action)Random.Range(0, Enum.GetValues(typeof(Action)).Cast<int>().Max() + 1)).ToString());
        }
        return chips;
    }


    public void SpawnPlayer(Vector3 position)
    {
        transform.position = position;
        transform.localRotation = Quaternion.Euler(0, 90 * angle, 0);
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
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
    }
}
