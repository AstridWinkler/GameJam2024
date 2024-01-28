using logiked.source.attributes;
using logiked.source.extentions;
using logiked.source.types;
using UnityEngine;

public class PlayerMovementHandler : MonoBehaviour
{

    protected Rigidbody2D rb;
    protected PlayerCollisionHandler playerCollisionHandler;
    private bool boost;
    private bool jumpingKey;
    private bool isUsingJumpingKey;



    [Header("Jumping")]

    [SerializeField] private float jumpVel;

    [SerializeField] [Min(0)] 
    private float jumpCooldown = 0.1f;
    private GameTimer jumpCooldownTimer;
  

    [Space(10)]
    [SerializeField] private bool useJumpInputBuffer = true;

    [ShowIf(nameof(useJumpInputBuffer), "==", true)]
    [SerializeField] [Min(0)] 
    private float jumpInputBufferDelay = 0.1f;
    private GameTimer jumpInputBufferPressTimer;
    private bool jumpInputBufferReleased;




    [Header("Walking")]

    [SerializeField] private float moveSpeed;
    [SerializeField] private float boostSpeed;

    [Space(10)]
    public bool customDeceleration = true;

    public float inputGroundAcceleration = 300f;
    [ShowIf(nameof(customDeceleration), "==", true)]
    public float inputGroundDeceleration = 300f;

    public float inputAirAcceleration = 200f;
    [ShowIf(nameof(customDeceleration), "==", true)]
    public float inputAirDeceleration = 200f;

    [Tooltip("Air drag, déceleration quand tu laches tes inputs en plein air")]
    public float inputAirNaturalDeceleration = 100f;//Quand tu laches tes inputs en plein air

    [SerializeField]
    PhysicsMaterial2D movementMat;

    [Header("Slope")]


    [Tooltip("Ajouter une force en Y dans les montés pour aider le joeur à les franchir")]
    [SerializeField]
    bool slopeHelper = false;


    //[Tooltip("Valeur max en degrées d'inclinaison du sol sur lequel le joeur recoit un boost de vélocité pour évoluer plus facilement")]
     float minMaxSlopeAngle => playerCollisionHandler.MaxSlopeAngle;

    [SerializeField]
    PhysicsMaterial2D slopeFreezeMat;

    // [SerializeField]
    // Vector2 minMaxSlopeAngle;


    [SerializeField]
    [Tooltip("Verouiller la chute à partir du moment où la velocité Y est inferieure à cette valeure")]
    public float lockSlideVelocityY = 1f;




    [Header("Fall")]
    [Tooltip("Similaire à GravityScale")]
    public float fallMultiplier = 2.5f;
    [Tooltip("Puissance de la diminution de saut quand tu laches la bare espace")]
    public float lowJumpMultiplier = 2f;



    [Header("Walls")]
    public float wallJumpMultiplierX = 0.9f;
    public float wallJumpMultiplierY = 1f;

    [SerializeField]
    private bool stuckToWalls = true;



    [Tooltip("Vitesse de descente des murs")]
    public float wallSlideFallMaxSpeed = 3f;
    [Tooltip("Controle aerien apres un saut mural")]
    [Range(0, 1)]
    [SerializeField]
    float wallJumpControll = 0.5f;



    enum LockAirControl { None, LeftWalJump, RightWallJump}
    [GreyedField]
    [SerializeField]
    private LockAirControl lockAirControl = LockAirControl.None;

    protected virtual void Awake()
    {
        playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float moveKeys)
    {

        moveKeys = moveKeys.Sign(true);

        bool freezePlayer = false;
        bool boost = this.boost;




        //Walls Handle


        if (playerCollisionHandler.OnTouchingWall())
        {

            if ( (playerCollisionHandler.OnLeftWall() && moveKeys <= 0 && stuckToWalls) || (moveKeys < 0 && playerCollisionHandler.OnTouchingLeftWall())) 
            {
                moveKeys = -0.25f;
                boost = false;
            }

            if ( (playerCollisionHandler.OnRightWall() && moveKeys >= 0 && stuckToWalls) || (moveKeys > 0 && playerCollisionHandler.OnTouchingRightWall()) )
            {
                moveKeys = 0.25f;
                boost = false;
            }
        }




        //Sliding Handle



        if (playerCollisionHandler.Slinding != PlayerCollisionHandler.PlayerSliding.no)
        {
            if (playerCollisionHandler.Slinding == PlayerCollisionHandler.PlayerSliding.Left)
                moveKeys = moveKeys.Clamp(-1f, 0);
            else
                moveKeys = moveKeys.Clamp(0,1);
        }



        //Air control Handle


        float lockAirControlXDirection = 0;

        switch (lockAirControl)//Faire que tu ne puisses pas revenir sur un jumpwall
        {
            case LockAirControl.RightWallJump://Si tu essaies de revenir sur le mur, on reduit de fou le temps des controles
                lockAirControlXDirection = 1;
                break;

            case LockAirControl.LeftWalJump:
                lockAirControlXDirection = -1;
                break;
        }

 



        //Acceleration & deceleraton compute

        float newMoveSpped = 0;

        if (moveKeys != 0)
        {
            newMoveSpped = (boost ? boostSpeed : moveSpeed);
        }

        Vector2 targetVelocity = new Vector2(newMoveSpped * moveKeys, rb.velocity.y);

        //Deceleration 

        if (!customDeceleration)
        {
            inputAirDeceleration = inputAirAcceleration;
            inputGroundDeceleration = inputGroundAcceleration;
        }

        var accelerationDirection = (targetVelocity.x - rb.velocity.x).Sign(true);
        bool isDecelerating = rb.velocity.x.Abs() > 0.1f && accelerationDirection != rb.velocity.x.Sign(true);

        float inputSpeed = isDecelerating ? inputGroundDeceleration : inputGroundAcceleration;

        if (playerCollisionHandler.OnAloft())
        {
            inputSpeed = isDecelerating ? ((moveKeys == 0) ? inputAirNaturalDeceleration : inputAirDeceleration) : inputAirAcceleration;
        }



        Debug.DrawRay(transform.position, targetVelocity.Mult(Vector2.right), Color.red);




        //Slope & stairs managamenet


        var FacingSlope = false;


        if (slopeHelper && playerCollisionHandler.OnGround()  && !playerCollisionHandler.OnGroundWall() && !jumpCooldownTimer.IsDefinedAndActive())
        {
            var groundAng = playerCollisionHandler.CurrentFacedSlope.Abs();
             FacingSlope = groundAng < minMaxSlopeAngle && groundAng > 5f;

          //  Debug.Log(playerCollisionHandler.CurrentFacedSlope.Abs());


            if (FacingSlope)
            {
                targetVelocity.y = 0;
                targetVelocity = targetVelocity.normalized.Rotate(-playerCollisionHandler.CurrentFacedSlope);
                targetVelocity.x *= newMoveSpped;
                targetVelocity.y *= moveSpeed;

                Debug.DrawRay(transform.position, targetVelocity.normalized, Color.green);

                //fonctionne:
                //rb.velocity = new Vector2(rb.velocity.x, targetVelocity.y);

                if (rb.velocity.y <= 0 && rb.velocity.y.Abs() < lockSlideVelocityY && moveKeys == 0)
                {
                    freezePlayer = true;
                }

            }
        }



        //Physic mat apply

        if (freezePlayer)
        {
            rb.sharedMaterial = slopeFreezeMat;
            targetVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;
            rb.Sleep();
            //  Debug.Log("stoping");
        }else
            rb.sharedMaterial = movementMat;




        //Wall jump air drag

        //Si tu essaies d'aller dans le sens inverse que le mur que tu viens de sauter, ben c'est l'air control qui gere
        if (lockAirControlXDirection != 0 && accelerationDirection == lockAirControlXDirection) inputSpeed *= wallJumpControll;


        //Velocity apply

        if (FacingSlope)
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, targetVelocity.x, inputSpeed * Time.deltaTime), targetVelocity.y);
        else
            rb.velocity = Vector2.MoveTowards(rb.velocity, targetVelocity, inputSpeed * Time.deltaTime);


    }


    public void JumpPressDown()
    {
        JumpPressBuffered(false);
    }

    private void JumpPressBuffered(bool isInputBuffer)
    {
        





        //Input buffer
        jumpInputBufferPressTimer.AutoStop();

        if (useJumpInputBuffer && !isInputBuffer && jumpInputBufferDelay > 0)
        {

            if (playerCollisionHandler.OnAloft())
            {
                jumpInputBufferPressTimer = new GameTimer(jumpInputBufferDelay);
                jumpInputBufferReleased = false;

                return;
            }
        }



        //Cooldown buffer

        if (jumpCooldown > 0)
        {
            if (jumpCooldownTimer.IsDefinedAndActive()) return;

            jumpCooldownTimer = new GameTimer(jumpCooldown);
        }



        //jump key pressed
        if (playerCollisionHandler.OnWall())
        {

            lockAirControl = playerCollisionHandler.OnRightWall() ? LockAirControl.RightWallJump : LockAirControl.LeftWalJump;
            float jumpDir = playerCollisionHandler.OnRightWall() ? -1 : 1;

            Vector2 jumpDirection = new Vector2(jumpVel * wallJumpMultiplierX * jumpDir, jumpVel * wallJumpMultiplierY);
            rb.velocity = jumpDirection;
            jumpingKey = true;

        }
        else if (playerCollisionHandler.OnGround(true))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVel);
            jumpingKey = true;
        }


    }




    public void JumpPressUp()
    {

        if (useJumpInputBuffer)
        {
            if (jumpInputBufferPressTimer.IsDefinedAndActive())
                jumpInputBufferReleased = true;
        }


        if ( jumpingKey)
        {
            jumpingKey = false;
         //   rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }


    private void LateUpdate()
    {

    }

    protected virtual void Update()
    {


        //JumpInput buffer
        if (useJumpInputBuffer)
        {
            if (!playerCollisionHandler.OnAloft() && jumpInputBufferPressTimer.IsDefinedAndActive())
            {
                JumpPressBuffered(true);
                jumpInputBufferPressTimer.AutoStop();
            }

            if (jumpInputBufferReleased && jumpInputBufferPressTimer.IsNullOrInactive())
            {
                JumpPressUp();
                jumpInputBufferReleased = false;                
            }
        }





        //Jump velocity

        var jumpingVel = (!playerCollisionHandler.OnGround() && rb.velocity.y > 0f) ;
        //isJumping &= jumpingVel;


        if (!jumpingVel || rb.velocity.y < 0)
            lockAirControl = LockAirControl.None;

        if (rb.velocity.y < 0)
        {
                rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        

            if (playerCollisionHandler.OnWall())
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideFallMaxSpeed));
            }
         

        } if (rb.velocity.y > 0 && !jumpingKey)
            {
                rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }


        
       
        

    }


    public void SetBoost(bool boost)
    {
        this.boost = boost;
    }

}
