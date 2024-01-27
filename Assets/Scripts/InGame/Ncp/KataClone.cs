using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelCloneChasing;

public class KataClone : PlayerBase
{
    [SerializeField]
    GameObject diePrefab;
    Collider2D col;

    PlayerMovementKey currentPoint;
   // public float StartTime { get; set; }
    public int StartIndex { get; private set; }// => (int)(StartTime * 60); }
    public bool IsDead { get; private set; }

    bool isDashing;

    public void Init(int startIndex)
    {
        StartIndex = startIndex;
    }

    protected override void Awake()
    {
        diePrefab.Inst(transform.position);
        col = GetComponent<Collider2D>();
        base.Awake();
    }

    public void UpdatePosFixed(PlayerMovementKey point)
    {
        UpdatePosFixed(point, point,0);
    }

    public void UpdatePosFixed(PlayerMovementKey point, PlayerMovementKey NextPoint, float p)
    {
        if (IsDead) return;


        if (point.idlAnim != currentPoint.idlAnim)
            anim.SetParameter("idl", point.idlAnim, true);



        if (!isDashing && point.isDashing)
        {
            // GameManager.Gameplay.ParticlePlayerMovement(point.pos, NextPoint.pos, Color.magenta, 0.3f);
            rend.transform.localScale = new Vector3(1f, 0.1f, 1f);
        }
        else if (isDashing && !point.isDashing)
        {
            rend.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        isDashing = point.isDashing; 

        col.enabled = !point.isDashing;
        currentPoint = point;
        transform.position = Vector3.Lerp(point.pos, NextPoint.pos, p);
        rend.flipX = point.flipX;
        actionKeyPressed = point.actionKeyPressed;
    }


    public void Kill()
    {
        gameObject.SafetyDestroyWithComponents();
        IsDead = true;
        diePrefab.Inst(transform.position);
    }

    public override void UpdatePlayer()
    {
      
    }
}
