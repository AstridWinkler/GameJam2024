using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimeControl : LevelMod
{
    float currentTimeValue=1f;
    Animator mainCameraAnim;

    public override void OnBackToCheckPoint()
    {
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnRemoveCheckpoint()
    {
    }

    public override void OnSaveCheckpoint()
    {
    }

    private void Start()
    {
     //   mainCameraAnim= GameManager.CamController.
    }

    public override void OnUpdate()
    {

        currentTimeValue += Input.mouseScrollDelta.y*0.1f;
        currentTimeValue = Mathf.Clamp01(currentTimeValue);
        float t = currentTimeValue * 0.6f+0.2f;
        if(t != Time.time)
            SetTimescale(t);
        GameManager.CamController.Anim.SetFloat("TimeScroll",1f- currentTimeValue);

        //  TimeScroll += Input.mouseScrollDelta.y;
    }

    public override void ExitMod()
    {
        currentTimeValue = 1f;
        SetTimescale(0.8f);
    }

    void SetTimescale(float newTime)
    {
        Time.timeScale = newTime;
        Time.fixedDeltaTime = newTime*0.01875f;

    }

}
