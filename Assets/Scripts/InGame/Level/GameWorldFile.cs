using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
[CreateAssetMenu(fileName = "GameLevelInfo", menuName = "MrKata/GameLevel", order = 1)]
public class GameWorldFile : ScriptableObject
{

    public enum LevelMods { TimeControl = 1, CloneChasing= 2 }

    [BitMaskAttribute(typeof(LevelMods)), SerializeField]
    private LevelMods defaultMods;
    public LevelMods DefaultMods => defaultMods;



    /*
    [SerializeField]
    private  firstScene;
    [SerializeField]
    public Scene FirstScene => firstScene;
    */

    [SerializeField]
    private AudioClip mainMusic;
    public AudioClip MainMusic => mainMusic;



}
