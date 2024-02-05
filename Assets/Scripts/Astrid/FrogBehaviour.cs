using logiked.Tool2D.animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBehaviour : MonoBehaviour
{

    internal Transform thisTransform;
    public AnimatorRenderer2D animator;

    // The movement speed of the object
    public float moveSpeed = 0.2f;

    // A minimum and maximum time delay for taking a decision, choosing a direction to move in
    public Vector2 decisionTime = new Vector2(1, 4);
    internal float decisionTimeCount = 0;

    // zero for staying same place
    internal Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.zero, Vector3.zero };
    internal int currentMoveDirection;

    void ChooseMoveDirection()
    {
        currentMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));
    }

    void Start()
    {
        // Cache the transform for quicker access
        thisTransform = this.transform;
        // Set a random time delay for taking a decision ( changing direction,or standing in place for a while )
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);

        animator = GetComponent<AnimatorRenderer2D>();

        // Choose a movement direction, or stay in place
        ChooseMoveDirection();

    }

    void Update()
    {
        // Move the object in the chosen direction at the set speed
        Vector3 direction = moveDirections[currentMoveDirection];
        float xDir = direction.x;

        thisTransform.position += direction * Time.deltaTime * moveSpeed;

        animator.SetStateValue("jump", 1, false);

        if (decisionTimeCount > 0) decisionTimeCount -= Time.deltaTime;
        else
        {
            // Choose a random time delay for taking a decision ( changing direction, or standing in place for a while )
            decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);

            // Choose a movement direction, or stay in place
            ChooseMoveDirection();
        }

    }
}



