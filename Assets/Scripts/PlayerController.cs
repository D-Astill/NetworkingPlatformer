//S

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    #region Public Variables
    [Header("Player Variables")]
    [Space(5)]
    public int points;
    public int health = 50;
    public int stamina = 50;
    public int armor = 300;
    [Header("Movement Variables")]
    [Space(5)]
    public float jumpHeight = 10.0f;
    public float movementDampening = 0.1f;
    public float movementSpeed = 20.0f;
    public float raydistance = 2.0f;
    public LayerMask layerMask;
    #endregion
    #region Private Variables
    private bool isJumping = false;
    private float runTimer = 0;
    private bool isSpaceDown = false;
    private bool canJump = false;
    private Rigidbody2D rB;
    private MeshRenderer renderer;
    private Camera cam;
    private Vector2 hitNormal;
    #endregion


    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rB = GetComponent<Rigidbody2D>();
        renderer = GetComponent<MeshRenderer>();
        HandleNetwork();
    }

    void HandleNetwork()
    {
        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            JumpController();
            MovementControl();
        }
    }
    void Sprint()
    {

    }



    void FixedUpdate()
    {
        //proform a raycast to see if they are toutching the ground
        Bounds bounds = renderer.bounds;
        Vector3 size = bounds.size;
        Vector3 scale = transform.localScale;
        float height = (size.y * 0.5f) * scale.y;
        //create ray
        Vector3 origin = transform.position + Vector3.down * height;
        Vector3 direction = Vector3.down;
        Ray ray = new Ray(origin, direction);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, raydistance, ~layerMask.value);
        Debug.DrawLine(origin, origin + direction * raydistance , Color.blue);
        if (hit.collider != null)
        {
            //check if the ray hits something 
            isJumping = false;
        }
    }
    #region CustomFuctions
    void JumpController()
    {
        if (!isJumping)
        {
            Debug.Log("Is Jumping");
            float inputVert = Input.GetAxis("Jump");
            if (inputVert > 0 && canJump)
            {
                canJump = false; 
                rB.AddForce(Vector3.up * inputVert * jumpHeight , ForceMode2D.Impulse);
                isJumping = true;
            }
            if (inputVert == 0)
            {
                canJump = true;
            }
        }
       
    }

    void MovementControl()
    {
        //obtaining movement for left and right
        float inputHoriz = Input.GetAxis("Horizontal");
        rB.velocity = new Vector2(Mathf.Lerp(rB.velocity.x,0,movementDampening),rB.velocity.y);
        Debug.DrawLine(transform.position,transform.position + Vector3.right * rB.velocity.x, Color.cyan);
        if (inputHoriz > 0 && hitNormal.x != -1)//move right
        {
            rB.velocity = new Vector2(movementSpeed, rB.velocity.y);
        }
        if (inputHoriz < 0 && hitNormal.x != 1)//move left
        {
            rB.velocity = new Vector2(-movementSpeed, rB.velocity.y);
        }
    }
    void OnCollisionStay2D(Collision2D other)
    {
        //Obtains the contact normal of the collision
        hitNormal = other.contacts[0].normal;
        Vector3 point = other.contacts[0].point;
        //draw a line from the collision point
        //to the direction of the normals
        Debug.DrawLine(point, point + (Vector3)hitNormal * 5, Color.red);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Point")
        {
            points++;
            Destroy(other.gameObject);
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        hitNormal = Vector2.zero;
    }
    #endregion
}
