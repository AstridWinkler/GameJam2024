using logiked.source.extentions;
using logiked.source.types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoots : MonoBehaviour
{

    Transform objectToShoot;
    public Transform weaponMuzzle;
    public GameObject bullet; //prefab
    public float shootingPower = 20f; //force of projection
    float distToBody;
    public float MaxDist = 10;


    void Start()
    {
        if (GameManager.Gameplay.CurrentPlayer != null)
            objectToShoot = GameManager.Gameplay.CurrentPlayer.transform;

       var time = new GameTimer(0.5f, Action);
    }


    void Action()
    {
        if (objectToShoot == null)
        {
            Start();
            return;
        }

        distToBody = Vector3.Distance(transform.position, objectToShoot.position);

        if (distToBody <= MaxDist)
        {
            Vector2 enemyPos = new Vector2(weaponMuzzle.position.x, weaponMuzzle.position.y); //where muzzle direction
            GameObject projectile = Instantiate(bullet, enemyPos, Quaternion.identity); //create bullet
            float direction = (enemyPos - (Vector2)objectToShoot.position).x.Sign(); //get the direction to the objectToShoot
            projectile.GetComponent<Rigidbody2D>().velocity = Vector2.left * direction * shootingPower; //shoot bullet

            new GameTimer(1f, Pause);
        } else
        {
            new GameTimer(0.5f, Action);
        }
    }


    void Pause()
    {
        new GameTimer(3f, Action);
    }
    
}
