using System.Collections;
using System.Collections.Generic;
using UnityEngine;








public class TransformAnim : BasicManager
{


    static GameStateController gsc;

    public Coroutine StartCor(IEnumerator cor)
    {
    return    StartCoroutine(cor);
    }

    protected override void InitAction()
    {
        gsc = GameManager.StateGame;
    }



    public static bool WaitTestByActiveState(GameStateType waitFor)
    {
        if(gsc == null)
            gsc = GameManager.StateGame;
        switch (waitFor)
        {
            case GameStateType.animation:
                return gsc.AnimationsPaused;

            case GameStateType.control:
                return gsc.ControlsPaused;

            case GameStateType.loading:
                return gsc.InLoading;

            case GameStateType.mob:
                return gsc.MobsPaused;

            case GameStateType.pauseMenu:
                return gsc.InPauseMenu;

            case GameStateType.physics:
                return gsc.PhysicsPaused;
        }
        return false;
    }


}





public class Tra_LoopPack
{
    public delegate bool ToCall(Tra_LoopPack inf, params object[] obj);
    public ToCall toCall;
    public delegate void EndToCall(params object[] obj);
    public EndToCall toCall2;
    public delegate void EndToCall2();
    public EndToCall2 toCall3;


    public float w { get; private set; }
    public float wait { get; private set; }
    //public bool skipManager { get; private set; }
    public object[] objs { get; private set; }
    public float percent { get; private set; }
    public bool isRunning { get; private set; }
    bool endCall = false;

    Coroutine cor = null;

    public static Tra_LoopPack EndInvokeAuto(EndToCall2 endCallFunc, float time, bool waitMob = true)
    {
        return new Tra_LoopPack(endCallFunc, time, (waitMob) ? GameStateController.Wait_MobClassic : null);
    }



    public Tra_LoopPack(EndToCall2 endCallFunc, float _wait, params GameStateType[] waitFor)
    {
        objs = new object[0];
        endCall = true;
        toCall3 = endCallFunc;
        toCall2 = null;

        isRunning = true;
        wait = _wait;
        cor = GameManager.TransfAnim.StartCor(StartLoop(waitFor));
    }

    public Tra_LoopPack(EndToCall endCallFunc, object[] _objs, float _wait, params GameStateType[] waitFor)
    {
        endCall = true;
        toCall2 = endCallFunc;
        toCall3 = null;
        isRunning = true;
        wait = _wait;
        objs = _objs;

        cor = GameManager.TransfAnim.StartCor(StartLoop(waitFor));
    }

    public Tra_LoopPack(float _wait, params GameStateType[] waitFor)
    {
        endCall = false;
        toCall2 = null;
        toCall3 = null;
        objs = null;
        wait = _wait;
        isRunning = true;
        cor = GameManager.TransfAnim.StartCor(StartLoop(waitFor));
    }


    public Tra_LoopPack(float _wait, ToCall func, params object[] _objs)
    {
        objs = _objs;
        //	skipManager = _skipManager;
        wait = _wait;
        toCall = func;
        isRunning = true;
        cor = GameManager.TransfAnim.StartCor(StartLoop());
    }


    public Tra_LoopPack(ToCall func, float _wait, GameStateType[] waitFor, params object[] _objs)
    {
        objs = _objs;
        //	skipManager = _skipManager;
        wait = _wait;
        toCall = func;
        isRunning = true;

        cor = GameManager.TransfAnim.StartCor(StartLoop(waitFor));
    }







    public void StopAnim()
    {
        if (cor != null)
        {
            isRunning = false;
            GameManager.TransfAnim.StopCoroutine(cor);
            cor = null;
        }

    }


    public IEnumerator StartLoop(params GameStateType[] waitFor)
    {
        w = 0;
        percent = 0;

        while (w < wait)
        {
            w += Time.deltaTime;
            percent = w / wait;

            if (waitFor != null)
            {
                foreach (GameStateType w in waitFor)
                    while (TransformAnim.WaitTestByActiveState(w)) { yield return null; }
            }
            yield return null;

            if (toCall != null && !endCall && !toCall(this, objs))
            {

                StopAnim();
                yield break;
            }
        }

        percent = 1f;

        if (endCall)
        {
            if (toCall2 != null)
                toCall2(objs);
            else toCall3();
        }
        else if (toCall != null)
            toCall(this, objs);




        isRunning = false;


    }

    public static bool ChangePosition(Tra_LoopPack p, params object[] objs)
    {
        if (((Transform)objs[0]) != null)
        {
            ((Transform)objs[0]).position = Vector3.Lerp((Vector3)objs[1], (Vector3)objs[2], p.percent);
            return true;
        }
        else
            return false;
    }

    public static bool ChangePosRotToTransform(Tra_LoopPack p, params object[] objs)
    {
        if (((Transform)objs[0]) != null && ((Transform)objs[1]) != null)
        {
            ((Transform)objs[0]).position = Vector3.Lerp((Vector3)objs[2], ((Transform)objs[1]).position, p.percent);
            ((Transform)objs[0]).eulerAngles = Vector3.Lerp((Vector3)objs[3], ((Transform)objs[1]).eulerAngles, p.percent);

            return true;
        }
        else
            return false;
    }



    public static bool ChangeRotation(Tra_LoopPack p, params object[] objs)
    {
        if (((Transform)objs[0]) != null)
        {
            ((Transform)objs[0]).localEulerAngles = Vector3.Lerp((Vector3)objs[1], (Vector3)objs[2], p.percent);
            return true;
        }
        else
            return false;
    }




    public static bool ChangeLocalPosition(Tra_LoopPack p, params object[] objs)
    {

        if (objs[0] != null && (Transform)objs[0] != null && p != null)
        {
            ((Transform)objs[0]).localPosition = Vector3.Lerp((Vector3)objs[1], (Vector3)objs[2], p.percent);
            return true;
        }
        else
            return false;
    }



    public static bool RectTransformSize(Tra_LoopPack p, params object[] objs)
    {

        if (objs[0] != null)
        {
            ((RectTransform)objs[0]).sizeDelta = Vector2.Lerp((Vector2)objs[1], (Vector2)objs[2], p.percent);
            return true;
        }
        else
            return false;
    }



    public static bool ChangeSpriteColor(Tra_LoopPack p, params object[] objs)
    {
        if (objs[0] != null && ((SpriteRenderer)(objs[0])) != null)
        {
            ((SpriteRenderer)(objs[0])).color = Color.Lerp((Color)objs[1], (Color)objs[2], p.percent);
            return true;
        }
        else
        {
            return false;
        }
    }


    public static bool ChangeRendererColor(Tra_LoopPack p, params object[] objs)
    {
        if (objs[0] != null && ((Renderer)(objs[0])) != null)
        {
            ((Renderer)(objs[0])).material.color = Color.Lerp((Color)objs[1], (Color)objs[2], p.percent);
            return true;
        }
        else
            return false;
    }
}







public class WaitBlock
{
	public bool Finished;
	float waitTime;


	public WaitBlock(float time, params GameStateType[] waitFor)
	{
		Finished = false;
		waitTime = time;
		GameManager.TransfAnim.StartCoroutine(Wait(waitFor));
	}



		IEnumerator Wait(params GameStateType[] waitFor)
	{

		float counter = 0f;

		while (counter < waitTime)
		{
			foreach (GameStateType w in waitFor)
				if (TransformAnim.WaitTestByActiveState(w)) yield return null;
		

			counter += Time.deltaTime;
			yield return null; //Don't freeze Unity
		}

		Finished = true;
		yield break;
	}


}


