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

    public SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite shootingSprite;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (GameManager.Gameplay.CurrentPlayer != null)
            objectToShoot = GameManager.Gameplay.CurrentPlayer.transform;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
            spriteRenderer.sprite = shootingSprite;
            audioSource.PlayOneShot(audioSource.clip);

            Vector2 enemyPos = new Vector2(weaponMuzzle.position.x, weaponMuzzle.position.y); //where muzzle direction
            GameObject projectile = Instantiate(bullet, enemyPos, Quaternion.identity); //create bullet
            var direction = (enemyPos - (Vector2)objectToShoot.position).x.Sign(); //get the direction to the objectToShoot
            transform.localScale = new Vector3(direction * 1,1,1);
            projectile.GetComponent<Rigidbody2D>().velocity = Vector2.left * direction * shootingPower; //shoot bullet
  
            new GameTimer(1f, Pause);
        } else
        {
            spriteRenderer.sprite = defaultSprite;
            new GameTimer(0.5f, Action);
        }
    }

    void Pause()
    {
        spriteRenderer.sprite = defaultSprite;
        new GameTimer(3f, Action);
    }
    
}
