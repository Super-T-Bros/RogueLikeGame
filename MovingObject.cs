using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;   //sec
    public LayerMask blockingLayer; //layer to check collision

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;       //component reference
    private float inverseMoveTime;   //make movement calculations efficient

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;    // use by multiplying instead of dividing - more efficient
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir); //finds end position

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer); // creates line between start and end and checks blocking layer
        boxCollider.enabled = true;

        //space in line was open
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true; // were able to move
        }
        return false; // unsuccessful move
    }

    protected IEnumerator SmoothMovement (Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;


        while (sqrRemainingDistance > float.Epsilon) //float.epsilon ~ 0
        {
            //moves point (newPosition) in a straight line towards target point (moves between rb2d and end) by a given distance ( inverseMoveTime * Time.deltaTime)
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null; // wait for frame before reevaluating loop condition
        }
    }

    //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword
    //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player)
    protected virtual void AttemptMove <T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;

        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    //don't know if we'll have to interact with player (as zombie) or with wall (as player)
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}