﻿using ProjectCardboardBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enum = System.Enum;

public class PlayerBehaviour : MonoBehaviour
{
    public GameBehaviour game;
    public GridBehaviour grid;
    public PodiumBehaviour podium;
    HintBehaviour hintBehaviour;
    NicknameBehaviour nicknameBehaviour;
    public GameObject DeathFire;

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

    public Animator animator;
    public Vector3 velocity = Vector3.zero;

    Material colour;

    void Awake()
    {
        grid = FindObjectOfType<GridBehaviour>();
        game = FindObjectOfType<GameBehaviour>();
        podium = FindObjectOfType<PodiumBehaviour>();
        hintBehaviour = GetComponent<HintBehaviour>();
        nicknameBehaviour = GetComponent<NicknameBehaviour>();
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
                nicknameBehaviour.UpdatePosition(transform.position, this);
                if (lerpBounce.IsDone())
                    StopBounce();
            }

            if (distance > 0)
            {
                transform.position += transform.forward * Time.deltaTime * speed;
                nicknameBehaviour.UpdatePosition(transform.position, this);
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
        var hints = hint.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries).Select(s => new Command(s)).ToArray();
        hintBehaviour.DisplayHint(transform.position, angle, grid, hints, colour);
    }

    public void ReceiveColour(string col)
    {
        if (ColouredMaterial.Instance.materials.ContainsKey(col))
        {
            colour = ColouredMaterial.Instance.materials[col];
            hintBehaviour.UpdateColour(colour);
        }
    }

    public void ReceiveNickname(string nickname)
    {
        gameObject.name = nickname;
        nicknameBehaviour.UpdatePosition(transform.position, this);
        nicknameBehaviour.UpdateNickname(nickname);
    }

    public void CancelAndClearCommands()
    {
        commandQueue.Clear();
        StopMoving();
        StopRotating();
    }

    public void SpawnedInLobby()
    {
        hintBehaviour.DisplayCircle(transform.position, colour);
    }

    public void SpawnPlayer(Vector3 position)
    {
        transform.position = position;
        transform.localRotation = Quaternion.Euler(0, 90 * angle, 0);
        hintBehaviour.DisplayCircle(transform.position, colour);
        nicknameBehaviour.UpdatePosition(transform.position, this);
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);
        hintBehaviour.DestroyHint();
        nicknameBehaviour.DestroyNickname();
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

        hintBehaviour.DestroyHint();

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
        hintBehaviour.DestroyHint();
        hintBehaviour.DisplayCircle(transform.position, colour);
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

        animator.SetBool("Bouncing", true);
        hintBehaviour.DestroyHint();
        runningCommand = true;
    }

    void StopBounce()
    {
        lerpBounce = null;
        runningCommand = false;
        velocity = Vector3.zero;
        animator.SetBool("Bouncing", false);
        hintBehaviour.DisplayCircle(transform.position, colour);

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

        hintBehaviour.DestroyHint();

        runningCommand = true;
    }

    void StopRotating()
    {
        lerpRotation = null;
        runningCommand = false;
        velocity = Vector3.zero;
        animator.SetBool("Moving", false);

        hintBehaviour.DestroyHint();
        hintBehaviour.DisplayCircle(transform.position, colour);
    }


    void SnapToGrid()
    {
        transform.position = grid.SnapToGrid(transform.position);
    }

    void Die()
    {
        animator.SetTrigger("Dead");
        state = State.dead;
        game.PlayerDied(this);
        if (DeathFire == null)
            Debug.LogError("You forgot to assign death fire");
        else
        {
            var fire = Instantiate(DeathFire);
            fire.transform.SetParent(transform.parent);
            fire.transform.position = transform.position;
            StartCoroutine(RemoveFireAfterDelay(4, fire));
        }
        StartCoroutine(RemovePlayerFromBoardAfterDelay(3));
    }

    IEnumerator RemovePlayerFromBoardAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);

        state = State.ending;
        grid.RemovePlayerFromGrid(this);
        podium.AddPlayerToPodium(this);

        hintBehaviour.DestroyHint();
        nicknameBehaviour.UpdatePosition(transform.position, this);
    }

    IEnumerator RemoveFireAfterDelay(float time, GameObject fire)
    {
        yield return new WaitForSeconds(time);
        Destroy(fire);
    }

    public void YouWon()
    {
        state = State.ending;
        grid.RemovePlayerFromGrid(this);
        podium.AddPlayerToPodium(this, true);
        hintBehaviour.DestroyHint();
        nicknameBehaviour.UpdatePosition(transform.position, this);
    }
}
