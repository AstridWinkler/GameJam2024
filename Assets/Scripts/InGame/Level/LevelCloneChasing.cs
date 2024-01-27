using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCloneChasing : LevelMod
{

    GameObject clonePrefab;
    List<PlayerMovementKey> playerMovements = new List<PlayerMovementKey>();
    List<KataClone> clones = new List<KataClone>();
    Tra_LoopPack dieTra;

    [SerializeField]
    float delaySummon = 0.75f; 
    float gameTime = 0; 
    int checkpointIndex = 0;
    float currentColdownSummon = 0f;
    float coldownAtCheckpoint = 0f;

    readonly float rate=60;


    public struct PlayerMovementKey
    {
        public readonly Vector3 pos;
        public readonly int idlAnim;
        public readonly bool flipX;
        public readonly bool isDashing;
        public readonly bool actionKeyPressed;
        public readonly float time;


        public PlayerMovementKey(Vector3 pos, int idlAnim, bool lookinDir, float time, bool keyPressed, bool isDashing)
        {
            this.pos = pos;
            this.idlAnim = idlAnim;
            this.flipX = lookinDir;
            this.time = time;
            this.actionKeyPressed = keyPressed;
            this.isDashing = isDashing;
        }
    }


    private void Start()
    {
        clonePrefab = GameManager.Resources.ClonePrefab;

               }






    public override void OnBackToCheckPoint()
    {
        int checkPointOffset = (playerMovements.Count - 1) - (int)(checkpointIndex);
        int currentOffset;
        gameTime = ((int)(playerMovements[Mathf.Min( checkpointIndex, playerMovements.Count-1)].time*60))/60f;

        dieTra = new Tra_LoopPack(0.3f, (p, m) =>
        {
            if (this == null) return false;


            foreach (var c in clones)
            {
                currentOffset = (playerMovements.Count - c.StartIndex) - (int)(checkPointOffset * p.percent);
                if (currentOffset >= 0)
                    c.UpdatePosFixed(playerMovements[currentOffset]);
                else
                {
                    c.UpdatePosFixed(playerMovements[0]);
                    c.Kill();
                }
            }


            clones.RemoveAll(b => b.IsDead);


            if (p.percent == 1)
            {
                // foreach (var c in clones)
                //    c.StartIndex -= checkPointOffset;
               currentColdownSummon = coldownAtCheckpoint;
                    playerMovements.RemoveRange( (int)checkpointIndex, playerMovements.Count -(checkpointIndex));
            }


            return true;
        }, GameStateController.Wait_MobClassic
        );
    }

    public override void OnFixedUpdate()
    {
        if (!enabled) return; 

        var currentPlayerCtr = GameManager.Gameplay.CurrentPlayerCtr;
        int needTocreate = 0;
        int count = 0;


        void UpChrono()
        {
            gameTime += Time.fixedDeltaTime;
            needTocreate = (int)(gameTime * rate) - playerMovements.Count;
        }

        if (currentPlayerCtr != null)
        {
            UpChrono();
            if (needTocreate >= 0)
            {
                Debug.LogError("obsolete");

                //     for (int i = 0; i < needTocreate; i++)                
                //          playerMovements.Add(new PlayerMovementKey(currentPlayerCtr.transform.position, currentPlayerCtr.AnimState, currentPlayerCtr.LookingDirection, gameTime, currentPlayerCtr.actionKeyPressed, GameManager.Gameplay.IsDashing));


                //    print(playerMovements.Count);
            }

            currentColdownSummon += Time.fixedDeltaTime;

            if (currentColdownSummon > delaySummon)
            {
                currentColdownSummon = 0;
                KataClone clone = clonePrefab.Inst(GameManager.Gameplay.DefaultSpawnPointPosition).GetComponent<KataClone>();
                clones.Add(clone);
                clone.Init(playerMovements.Count - 1);
            }
        }
        else if (GameManager.Gameplay.IsDashing)
        {
            UpChrono();

            for (int i = 0; i < needTocreate; i++)
            {
                //Obsolete
                Debug.LogError("obsolete");
                //  playerMovements.Add(new PlayerMovementKey(GameManager.Gameplay.TheoricalPlayerPos, playerMovements[playerMovements.Count - 1].idlAnim, playerMovements[playerMovements.Count - 1].flipX, gameTime, currentPlayerCtr.actionKeyPressed, GameManager.Gameplay.IsDashing));
            }

        }

        int selMov;

        foreach (var c in clones)
        {
            try {
                selMov = playerMovements.Count - c.StartIndex;
                count = 1;

                /*
                if (playerMovements[selMov].isDashing){//Find dash end position for animation
                 
                    while (count + selMov < playerMovements.Count)                    
                        if (!playerMovements[selMov + count++].isDashing) break;
                    
                    c.UpdatePosFixed(playerMovements[selMov], playerMovements[selMov + count], (gameTime * rate) - (int)(gameTime * rate));
                }
                else
                {*/
                    c.UpdatePosFixed(playerMovements[selMov], playerMovements[selMov + 1], (gameTime * rate) - (int)(gameTime * rate));
               // }
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                print(c.StartIndex + " < " + playerMovements.Count);

            }
        }
    }

    public override void ExitMod()
    {
        foreach (var c in clones)
            c.Kill();

        if (dieTra.ActiveAndPlaying()) dieTra.StopAnim();
        playerMovements.Clear();
        clones.Clear();

        checkpointIndex = 0;
        currentColdownSummon = 0;
        gameTime = 0;
    }

    public override void OnRemoveCheckpoint()
    {
        checkpointIndex = 0;
        coldownAtCheckpoint = 0;
    }


    public override void OnSaveCheckpoint()
    {
        checkpointIndex = playerMovements.Count-1;
        coldownAtCheckpoint = currentColdownSummon;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }





  
 











}
