using logiked.source.extentions;
using logiked.source.types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSas : MonoBehaviour
{
    [SerializeField]
    Logic_SimpleDoor door1;

    [SerializeField]
    Logic_SimpleDoor door2;
    [SerializeField]
    string nextLevelName;

    [SerializeField]
    GameObject world1CameraPath;


    [SerializeField]
    GameObject world2CameraPath;

    bool sasActivated;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(!sasActivated && collision.gameObject.tag == "Player")
        {
            sasActivated = true;
            SwitchStates();
        }
    }

    public void SwitchStates()
    {
        door1.Action();
        world1CameraPath.gameObject.SetActive(false);
        new GameTimer(1f, OpenSecondLevel);
    }


    void OpenSecondLevel()
    {
        if(door2 != null)
        door2.SetState(true);

        if(world2CameraPath != null)
        world2CameraPath.gameObject.SetActive(true);

        if (!nextLevelName.IsNullOrEmpty())
            SceneManager.LoadScene(nextLevelName);

    }

    private void Awake()
    {
        if(world2CameraPath != null)
        world2CameraPath.SetActive(false);
    }

}
