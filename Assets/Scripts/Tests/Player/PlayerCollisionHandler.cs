using logiked.source.attributes;
using logiked.source.extentions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerCollisionHandler : MonoBehaviour
{
    public enum PlayerOn
    {
        Aloft, Ground, Sliding, RightWall, LeftWall, GroundRightWall, GroundLeftWall
    }

    public enum PlayerSliding
    {
        Left = 1, no = 0, Right = -1
    }

    #region Referneces
    private Collider2D col;
    public Collider2D Col  => col;
    private Transform tr;
    #endregion


    #region Propertices

    public LayerMask groundLayer;
    public PlayerOn playerOn;
    private Vector2 downOffset;
    private Vector2 slopeOffset;
    private Vector2 rightOffset, leftOffset;

    [Header("Walls")]
    [SerializeField]
    private float wallCheckerYOffset;
    [SerializeField]
    private float wallsColliderRadius = 0.15f;

    [Header("Ground")]
    [SerializeField]
    private float bottomColliderWidth = 0.15f;

    [SerializeField]
    private float bottomColliderHeight = 0.15f;
    [SerializeField]
    private float bottomColliderOffset = 0f;

    [Header("Slope")]
    [SerializeField]
    private bool useSlopeRacast = true;

    [ShowIf(nameof(useSlopeRacast), "==", true)]
    [SerializeField]
    private float slopeColliderSizeWidth = 0.15f;
    [ShowIfSame()]
    [SerializeField]
    private float slopeColliderSizeHeight = 0.15f;
  
    [ShowIfSame()]
    [SerializeField]
    private float slopeColliderOffset = 0f;


    [GreyedField]
    [ShowIfSame()]
    [SerializeField]
    private PlayerSliding sliding;
    public PlayerSliding Slinding => sliding;
    public bool IsSlinding => Slinding != PlayerSliding.no;


    [Tooltip("Valeur max en degrées d'inclinaison du sol sur lequel le joeur peut evoluer en marchant")]
    [RangeLogiked(0, 90)]
    [ShowIfSame()]
    [SerializeField]
    float maxSlopeAngle = 45;
    public float MaxSlopeAngle => maxSlopeAngle;


    private float currentFacedSlope;
    /// <summary>
    /// plus grand angle actuellement sous les pieds du joueur de degrées (normal en haut = 0 degs)
    /// </summary>
    public float CurrentFacedSlope
    {
        get { return currentFacedSlope; }
        set { currentFacedSlope = value; }
    }


    #endregion

    private void Awake()
    {
        tr = transform;
        col = GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {

        var colSize = col.bounds;
        var colOffset = col.offset + (Vector2)col.transform.localPosition;


        downOffset = Vector2.down * (colSize.size.y / 2) + colOffset + (Vector2)tr.position + Vector2.up * bottomColliderOffset;
        rightOffset = Vector2.right * (colSize.size.x / 2) + colOffset + (Vector2)tr.position + Vector2.up * wallCheckerYOffset;
        leftOffset = Vector2.left * (colSize.size.x / 2) + colOffset + (Vector2)tr.position + Vector2.up * wallCheckerYOffset;

        slopeOffset = Vector2.down * (colSize.size.y / 2) + colOffset + (Vector2)tr.position + Vector2.up * slopeColliderOffset;
        

        var midRectSize = new Vector2(slopeColliderSizeWidth / 2f, slopeColliderSizeHeight);

        Physics2D.queriesHitTriggers = false;

        if (useSlopeRacast)
        {
            var groundCast = Physics2D.BoxCastAll((Vector3)slopeOffset + Vector3.right * 0.05f * slopeColliderSizeWidth, new Vector3(0.1f * slopeColliderSizeWidth, slopeColliderSizeHeight, 0.1f), 0, Vector2.right, midRectSize.x * 0.9f, groundLayer);
            var groundCast2 = Physics2D.BoxCastAll((Vector3)slopeOffset + Vector3.right * 0.05f * slopeColliderSizeWidth, new Vector3(0.1f * slopeColliderSizeWidth, slopeColliderSizeHeight, 0.1f), 0, Vector2.left, midRectSize.x * 0.9f, groundLayer);
            CalculateGroundAngle(groundCast.Union(groundCast2).ToArray());
        }

        bool onGround = Physics2D.BoxCast(downOffset, new Vector2(bottomColliderWidth, bottomColliderHeight), 0, Vector2.down, 0, groundLayer).transform != null;
        Physics2D.queriesHitTriggers = true;



        if (currentFacedSlope.Abs() > MaxSlopeAngle)
        {

            sliding = currentFacedSlope.Sign() < 0 ? PlayerSliding.Left : PlayerSliding.Right;
            onGround = false;
        } else
            sliding =  PlayerSliding.no;



        Physics2D.queriesHitTriggers = false;


        bool onRightWall = Physics2D.OverlapCircle(rightOffset, wallsColliderRadius, groundLayer);
        bool onLeftWall = Physics2D.OverlapCircle(leftOffset, wallsColliderRadius, groundLayer);

        Physics2D.queriesHitTriggers = true;


        //Check ground
        if (onGround && playerOn != PlayerOn.Ground)
        {                

            //Player is on the ground
                playerOn = PlayerOn.Ground;

            //check ground right wall
            if (onRightWall && playerOn != PlayerOn.GroundRightWall)
            {
                //Plyaer is on the ground and on the rightWall
                playerOn = PlayerOn.GroundRightWall;
            }

            //check ground left wall
            if (onLeftWall && playerOn != PlayerOn.GroundLeftWall)
            {
                //Player is on the ground and on the left wall
                playerOn = PlayerOn.GroundLeftWall;
            }
        }
        else
        {
            //Player isn't on the ground
            //check rightWall
            if (onRightWall && playerOn != PlayerOn.RightWall)
            {
                //Player is on the right wall
                playerOn = PlayerOn.RightWall;
            }
            //check leftwall
            else if (onLeftWall && playerOn != PlayerOn.LeftWall)
            {
                //Player is on the left wall
                playerOn = PlayerOn.LeftWall;
            }

        }

        //check aloft
        if ((!onGround && !onLeftWall && !onRightWall))
        {
            //plyaer is nowhere
            playerOn = PlayerOn.Aloft;
        }





    }

    public bool OnSurface => OnGround() || OnWall();

    public bool OnGround(bool andNotSliding = false)
    {
        switch (playerOn)
        {
            case PlayerOn.Ground:
            case PlayerOn.GroundRightWall:
            case PlayerOn.GroundLeftWall:
                //Player is on the ground
                return !andNotSliding || sliding == PlayerSliding.no;
            default:
                //Player isn't on the ground
                return false;
        }

    }
    public bool OnAloft()
    {
        switch (playerOn)
        {
            case PlayerCollisionHandler.PlayerOn.Aloft:
                //Player is on the Aloft
                return true;
            default:
                //Player is on the Aloft
                return false;
        }

    }



    void CalculateGroundAngle(RaycastHit2D[] rays)
    {
       if (rays.Length == 0) return;

        currentFacedSlope = float.MaxValue;

        float actSlope;

        for (int i = 0; i < rays.Length; i++)
        {
            actSlope = Vector2.SignedAngle(rays[i].normal, Vector2.up);
            Debug.DrawRay(rays[i].point, rays[i].normal, Color.blue);

            if(!OnWall() && (actSlope.Abs() - 90f).Abs() < 5)
            {
                //Si on est face à un escalier

                currentFacedSlope = 45f * actSlope.Sign();
                return;
            }


            if (actSlope.Abs() < currentFacedSlope)
                currentFacedSlope = actSlope;

            //  print(Vector2.SignedAngle(collision.contacts[i].normal, Vector2.up));
            //   Debug.DrawRay(collision.contacts[i].point, collision.contacts[i].normal, Color.blue );

        }

        

    }

    /*
    private void OnCollisionStay2D(Collision2D collision)
    {
        currentFacedSlope = 0;

        float actSlope;

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            actSlope = Vector2.SignedAngle(collision.contacts[i].normal, Vector2.up);

            if (actSlope.Abs() > currentFacedSlope)
                currentFacedSlope = actSlope;

            //  print(Vector2.SignedAngle(collision.contacts[i].normal, Vector2.up));
            //   Debug.DrawRay(collision.contacts[i].point, collision.contacts[i].normal, Color.blue );

        }
    }*/

    private void LateUpdate()
    {
        currentFacedSlope = 0;
    }


    public bool OnRightWall()
    {
        return playerOn == PlayerOn.RightWall;
    }
    public bool OnLeftWall()
    {
        return playerOn == PlayerOn.LeftWall;
    }
    public bool OnGroundRightWall()
    {
        return playerOn == PlayerOn.GroundRightWall;
    }
    public bool OnGroundLeftWall()
    {
        return playerOn == PlayerOn.GroundLeftWall;
    }
    public bool OnWall()
    {
        return OnRightWall() || OnLeftWall();
    }

    public bool OnTouchingWall()
    {
        return OnTouchingLeftWall() || OnTouchingRightWall();
    }


    public bool OnTouchingLeftWall()
    {
        return OnGroundLeftWall() || OnLeftWall();
    }
    public bool OnTouchingRightWall()
    {
        return OnGroundRightWall() || OnRightWall();
    }


    public bool OnGroundWall()
    {
        return OnGroundRightWall() || OnGroundLeftWall();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;


        Gizmos.DrawWireCube(slopeOffset + Vector2.right* slopeColliderSizeWidth/4f, new Vector3(slopeColliderSizeWidth/2f, slopeColliderSizeHeight, 0.1f));
        Gizmos.DrawWireCube(slopeOffset - Vector2.right* slopeColliderSizeWidth/4f, new Vector3(slopeColliderSizeWidth/2f, slopeColliderSizeHeight, 0.1f));

        Gizmos.color = Color.black;

        Gizmos.DrawWireCube( (Vector3)slopeOffset + Vector3.right * 0.05f * slopeColliderSizeWidth,  new Vector3(0.1f * slopeColliderSizeWidth, slopeColliderSizeHeight, 0.1f));



        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(downOffset ,  new Vector3( bottomColliderWidth, bottomColliderHeight, 0.1f));


        Gizmos.DrawWireSphere(rightOffset , wallsColliderRadius);
        Gizmos.DrawWireSphere(leftOffset , wallsColliderRadius);
    }
}
