﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GearBoard;
using UniRx;
using UniRx.Triggers;

public class GameManager : MonoBehaviour {
  public Board board;

  ResourceManager resource;
  UiManager ui;
  TargetManager tgtMan;

  [HideInInspector][SerializeField] new
  CameraManager camera;

  IEnumerator Start() {
    resource = new ResourceManager ();  
    yield return StartCoroutine (resource.LoadSceneAsync ("SoratoUi"));

    ui = GameObject.FindObjectOfType<UiManager> ();
    yield return StartCoroutine(ui.Build());

    board.Build ();
    yield return null;

    camera = GameObject.FindObjectOfType<CameraManager> ();
    camera.Build ();
    yield return null;

    tgtMan = new TargetManager ();
    tgtMan.targetChanged
      .Subscribe (t => board.shooter.SetTarget (t));
    yield return null;

    camera.OnUpdated
      .Subscribe (_ => tgtMan.Update ());
    yield return null;

    ui.rotChanged
      .Subscribe(v => board.kinematics.rotater.SetRotate(v.b, v.item.eulerAngles.z));
    ui.engChanged
      .Subscribe(i => board.kinematics.engine.SetState((Engine.State)i));
    ui.dashBtn.onClick.AsObservable ()
      .Subscribe (_ => board.kinematics.booster.Consume ());
    ui.fireBtn.onClick.AsObservable ()
      .Subscribe (_ => board.shooter.WillShoot ());
    yield return null;

    ui.text.UpdateAsObservable ()
      .Subscribe (_ => ui.text.text = board.kinematics.ToString());
    ui.dashGauge.UpdateAsObservable ()
      .Subscribe (_ => ui.dashGauge.fillAmount = board.kinematics.booster.power); 
    ui.fireGauge.UpdateAsObservable ()
      .Subscribe (_ => {
        ui.fireGauge.fillAmount = board.shooter.status.power;
        ui.fireText.text = board.shooter.status.count.ToString();
      });
    yield return null;

    board.kinematics.engine.SetState (Engine.State.Stop);
    yield return null;

    foreach (var dmg in board.damages) {
      dmg.OnDamage
        .Subscribe (d => camera.Shake ());
    }
    yield return null;

    ui.baseChanged
      .Select (a => new Vector3 (-a.y, a.x, 0f))
      .Subscribe (a => camera.current.Rotate (a));
    this.LateUpdateAsObservable ()
      .Subscribe (_ => {
        camera.current.SetTargetPosition (board.transform.position); 
        camera.UpdateCamera ();
      });
    yield return null;

  }

}