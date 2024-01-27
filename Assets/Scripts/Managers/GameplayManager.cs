
using logiked.source.extentions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameplayManager : BasicManager
{




	public Transform CamTargetPoint { get { return camTargetPoint; } }

    private List<LevelMod> levelMods = new List<LevelMod>();

    /*
    [SerializeField]
    GameWorldFile[] Worlds;
    */

    [SerializeField]
    AudioClip currentMusic;

    [SerializeField]
	Transform camTargetPoint;

    [SerializeField]
    Tilemap[] mainPaintMaps;
    public Tilemap[] MainPaintMaps { get { return mainPaintMaps; } }


    public LevelBlock LevelContent { get { return levelContent; } }
    [SerializeField, HideInInspector]
    LevelBlock levelContent;
    //Scene currentLevelScene ;
    bool isLoading;

   public bool IsDashing { get; private set; }



    [SerializeField]
	GameObject playerPref;
	[SerializeField]
	GameObject spawnPointPref;

	[SerializeField]
	GameObject respawnParticles;
    [SerializeField]
    GameObject respawnOrbsParticles;

    [SerializeField]
	Transform defaultSpawnPoint;

    [SerializeField]
    Stack<Transform> spawnPointInstances = new Stack<Transform>();

    [SerializeField]
 Transform currentSpawnPoint => spawnPointInstances.Count == 0 ? defaultSpawnPoint : spawnPointInstances.Peek();

    public Vector3 CurrentSpawnPointPosition { get => currentSpawnPoint.position; }
    public Vector3 DefaultSpawnPointPosition { get => defaultSpawnPoint.position; }

    public enum DieEnum {Explosion = 0, SimpleCorpse = 1 , Desintegration=2}
   public enum DieKiller {None = 0, Spikes = 1 }

    [SerializeField]
    GameObject[] diePref;
    [SerializeField]
    bool isPlaying;

    public GameObject CurrentPlayer { get { return currentPlayer; } }
    GameObject currentPlayer;

    public KataPlayerController CurrentPlayerCtr { get { return currentPlayerCtr; } }
    KataPlayerController currentPlayerCtr;

   public  Vector3 TheoricalPlayerPos { get { return  (currentPlayerCtr != null )? currentPlayerCtr.transform.position : theoricalPlayerPos; } }
    Vector3 theoricalPlayerPos;
    



    private void LoadLevel(string name)
    {
        UnloadCurrentLevel();

        StartCoroutine(Loadcene(name));

    }

    IEnumerator Loadcene(string name)
    {
        isLoading = true;
        var opt = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        opt.allowSceneActivation = true;


        while (!opt.isDone)
        {
            yield return null;
        }

        isLoading = false;
       // currentLevelScene = SceneManager.GetSceneByName(name);
        OnSceneLoad();
    }
          
    void PlayWorldMusic(AudioClip music)
    {
      if(levelContent.LinkedWorld.MainMusic != currentMusic){
            currentMusic = levelContent.LinkedWorld.MainMusic;
            var s = GetComponent<AudioSource>();
            s.Stop();
            s.loop = true;
            s.PlayOneShot(currentMusic);
        }
    }

    void OnSceneLoad()
    {
        levelContent = GameObject.FindObjectOfType<LevelBlock>();
        levelMods = levelContent.GetComponentsInChildren<LevelMod>().Where(m => m.enabled).ToList();

        if(levelContent.LinkedWorld.MainMusic != currentMusic)
        {
            PlayWorldMusic(levelContent.LinkedWorld.MainMusic);
        }

        Tilemap[] maps = GameObject.FindObjectsOfType<Tilemap>();
        foreach (var m in maps) m.color = new Color(m.color.r, m.color.g, m.color.b, 1f);

        if (levelContent == null) return;

        defaultSpawnPoint = levelContent.SpawnPoint;
        GameManager.CamController.SetCamera(levelContent.MainCamera.transform.parent);
        mainPaintMaps = levelContent.MainPaintMaps;

        foreach(var e in mainPaintMaps)
            e.CompressBounds();

        isPlaying = true;


        InstPlayer(true);


        ISceneObject[] sceneObjs = FindObjectsOfType<MonoBehaviour>(true).OfType<ISceneObject>().ToArray();

        foreach (var obj in sceneObjs)
        {
            obj.OnSceneLoaded();
        }


    }


    void ResetLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().path);
    }
    
    void UnloadCurrentLevel()
    {

        //  if (currentLevelScene.isLoaded)
        //      SceneManager.UnloadSceneAsync(currentLevelScene);

        foreach (Transform trf in GameManager.TempInstances)
        {
            if (trf != null)
            {
                Destroy(trf.gameObject);
            }
        }

        foreach (var l in levelMods)
        {
            l.ExitMod();
            Destroy(l);
        }
        levelMods.Clear();
        spawnPointInstances.Clear();


        levelContent = null;
        return;
    }


    /*
    public void CleanSpawnPoints()
    {
        foreach (var e in spawnPointInstances) Destroy(e);
    }
    */
    public void RemoveSpawnPoint()
    {

        if (spawnPointInstances.Count > 0)
        {

           // Destroy(spawnPointInstances.Last());
            if (currentSpawnPoint != null && currentSpawnPoint != defaultSpawnPoint)
                currentSpawnPoint.gameObject.GetComponent<Animator>().SetBool("die", true);
            spawnPointInstances.Pop();

            foreach (var e in levelMods) e.OnRemoveCheckpoint();
        }

    }

    public void LevelFinished()
    {
        if (!isPlaying)
            return;

        isPlaying = false;
        if (currentSpawnPoint != null)
            RemoveSpawnPoint();

            KillPlayer();

        var nextLvl = System.Text.RegularExpressions.Regex.Replace(SceneManager.GetActiveScene().name, "([0-9]+)$", (m) => { var n = int.Parse(m.Groups[1].Value); return (n + 1).ToString(); } );
        LoadLevel(nextLvl);
    }



    protected override void InitAction()
	{
		camTargetPoint = new GameObject("CamTargetPoint").transform;
        DontDestroyOnLoad(camTargetPoint);
    
        // currentLevelScene = SceneManager.GetActiveScene();
        // Debug.LogError(currentLevelScene.name);
        OnSceneLoad();
        //LoadLevel("Level1");



    }




    Tra_LoopPack instP;

	void InstPlayer(bool instant = false, Vector2 respawnPos = default(Vector2))
	{

        if (!isPlaying)
            return;

        bool dash = (respawnPos != Vector2.zero);
        float respawnTime = (dash) ? 0.2f : 0.3f;

        void SummonPlayer()
        {

            GameObject.Instantiate(respawnParticles, respawnPos, new Quaternion(), GameManager.TempInstances);
            
            var lev = levelContent;

            currentPlayer = GameObject.Instantiate(playerPref, respawnPos, new Quaternion(), GameManager.TempInstances);

            /*
            new Tra_LoopPack(() =>
            {
                if (lev != levelContent) return;
                currentPlayer = GameObject.Instantiate(playerPref, respawnPos, new Quaternion(), GameManager.TempInstances);
            }, 0.05f, GameStateController.Wait_MobClassic);*/
        }


      

        if (respawnPos == Vector2.zero) {
            if (currentSpawnPoint != null)
            respawnPos = currentSpawnPoint.position;
            else
            respawnPos = defaultSpawnPoint.position;
        }
 

        if (instant)
            SummonPlayer();
        else
        {
            if (!dash)
            {
                foreach (var e in levelMods) e.OnBackToCheckPoint();
            }
            else
            {
                IsDashing = true;
            }

                       


            if (!instP.ActiveAndPlaying())
            {
                float respawnTime2 = 1f;

                var dist = Mathf.Max( (Vector2.Distance(lastDiePos, respawnPos) / 12f).Floor(), 1);
                respawnTime2 = respawnTime * dist;

                var lev = levelContent;
                instP = new Tra_LoopPack(() => { if (lev == levelContent) SummonPlayer(); IsDashing = false; }, respawnTime2, GameStateController.Wait_MobClassic);
                ParticlePlayerMovement(lastDiePos, respawnPos, respawnTime2, dist, true);
            }
        }    
	}

    public void ParticlePlayerMovement(Vector2 start, Vector2 end /*, Color col*/, float time, int cycleCount = 2, bool moveCam=false)
    {
        List<GameObject> ballsAnim = new List<GameObject>();
        float rng = Random.Range(0, 1f);
        TrailRenderer trail;
        GameObject gb;


        for (int i = 0; i < 5; i++)
        {
            gb = respawnOrbsParticles.Inst(start);
            trail = gb.GetComponentInChildren<TrailRenderer>();
           // trail.time = time;            
           // trail.startColor = col;
           // trail.endColor = col;
            ballsAnim.Add(gb);
        }


        bool ParticleAnim(Tra_LoopPack p, params object[] objs)
        {
            if (defaultSpawnPoint == null) return false;
            int i = 0;

            Vector2 centerpos = Vector2.Lerp(start, end, p.percent);
            if (moveCam)
            {
                camTargetPoint.transform.position = centerpos;
                theoricalPlayerPos = centerpos;
            }

            foreach (var n in ballsAnim)
            {
                if (n == null) return false;
                n.transform.position = centerpos + ReMath.FloatToCircleVector(rng + (0.3f * p.percent* cycleCount) + ((float)i++) / (float)ballsAnim.Count) * Mathf.Sin(cycleCount*p.percent * Mathf.PI) * 2f;
            }


            if (p.percent == 1)
            {
                foreach (var n in ballsAnim)                
                    n.SafetyDestroyWithComponents();
                
            }
            return true;
        }



        instP = new Tra_LoopPack(ParticleAnim, time, GameStateController.Wait_MobClassic);

    }


    private int MaxActionAllowed = 3;

    public GameObject PlaceSpawnPoint(Vector2 pos)
	{
        if (spawnPointInstances.Count >= MaxActionAllowed)
            return null;

          //  RemoveSpawnPoint();
        spawnPointInstances.Push(GameObject.Instantiate(spawnPointPref, pos, new Quaternion(), GameManager.TempInstances).transform);
            foreach (var e in levelMods) e.OnSaveCheckpoint();
        return currentSpawnPoint.gameObject;
	}


	public void KillPlayer(Vector2 respawnPos = default(Vector2))
    {
        KillPlayer(DieEnum.Explosion, respawnPos: respawnPos);
    }


	bool trySpawn;
    Vector2 lastDiePos;

    public void KillPlayer(DieEnum dieMode, DieKiller killer = DieKiller.None, Vector2 respawnPos = default(Vector2) )
	{

   //     Debug.Log("player killed");
        if (currentPlayer != null && !trySpawn)
        {
            currentPlayerCtr.OnDie();
            trySpawn = true;
            GameObject corp = Instantiate(diePref[(int)dieMode], currentPlayer.transform.position, new Quaternion(), GameManager.TempInstances);
            var rigs = corp.GetComponentsInChildren<Rigidbody2D>();
   
            Vector2 playerForce = Vector2.zero;

            if (currentPlayer != null)
                playerForce = currentPlayerCtr.LastframeVelocity;

            switch (dieMode) {




                case DieEnum.Explosion:

                    foreach (Rigidbody2D rig in rigs)
                        rig.AddForce(playerForce * 0.5f, ForceMode2D.Impulse);

                    break;

                case DieEnum.SimpleCorpse:
                    switch (killer)
                    {
                        case DieKiller.None:
                            foreach (Rigidbody2D rig in rigs)
                                rig.AddForce(playerForce * 3f, ForceMode2D.Impulse);
                            break;

                        case DieKiller.Spikes:
                            foreach (Rigidbody2D rig in rigs)
                            {
                                rig.AddForce(playerForce.normalized * 50f, ForceMode2D.Impulse);
                            }
                                break;


                    }

                    break;

            }

            if (currentPlayer != null)
            {
                lastDiePos = currentPlayer.transform.position;
                Destroy(currentPlayer);
            }


			InstPlayer(false, respawnPos);
		}
	}







    void Update()
    {

        foreach (var e in levelMods) e.OnUpdate();

        trySpawn = false;
        if(currentPlayer  != null)
        {
            if (currentPlayerCtr == null)
                currentPlayerCtr = currentPlayer.GetComponent<KataPlayerController>();
        }

        if(!isLoading)
        if (Input.GetKeyDown(KeyCode.R))
            ResetLevel();

    }


    private void FixedUpdate()
    {
        foreach (var e in levelMods) e.OnFixedUpdate();

    }

 
}
