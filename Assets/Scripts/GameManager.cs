using System.Collections;
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
      .Subscribe (_ => board.kinematics.WillBoost());
    ui.fireBtn.onClick.AsObservable ()
      .Subscribe (_ => board.shooter.WillShoot ());
    yield return null;

    ui.spdGauge.UpdateAsObservable ()
      .Subscribe (_ => {
        ui.spdGauge.fillAmount = board.kinematics.speedRate;
        ui.spdText.text = board.kinematics.engine.speed.ToString("F1") + " m/s";
        ui.heightText.text = board.transform.position.y.ToString("F1") + " m";
      });

    ui.dashGauge.UpdateAsObservable ()
      .Subscribe (_ => {
        ui.dashGauge.fillAmount = board.kinematics.booster.power; 
        ui.dashText.text = Mathf.RoundToInt(board.kinematics.booster.power * 100).ToString();
      });
    ui.fireGauge.UpdateAsObservable ()
      .Subscribe (_ => {
        ui.fireGauge.fillAmount = board.shooter.status.power;
        ui.fireText.text = board.shooter.status.count.ToString();
      });
    yield return null;

    ui.extra.onClick.AddListener (() => {
      var post = camera.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
      post.enabled = !post.enabled;
    });
    yield return null;

    board.kinematics.engine.SetState (Engine.State.Stop);
    board.kinematics.OnBoosted
      .Where (b => b)
      .Subscribe (_ => ui.chara.SetFace (CharaFace.Cool));
    yield return null;

    foreach (var dmg in board.damages) {
      dmg.OnDamage
        .Subscribe (d => camera.Shake ());
      dmg.OnDamage
        .Subscribe (d => ui.chara.SetFace(CharaFace.Ouch));
    }
    tgtMan.OnDamage
      .Subscribe (d => ui.chara.SetFace (CharaFace.Smile));
    yield return null;

    ui.baseChanged
      .Select (a => new Vector3 (-a.y, a.x, 0f))
      .Subscribe (a => camera.current.Rotate (a));
    this.LateUpdateAsObservable ()
      .Subscribe (_ => {
        camera.current.SetTargetPosition (board.transform.position); 
        camera.UpdateCamera ();

        var p = camera.ScreenPosition(board.shooter.target.position);
        ui.UpdateTargetPosition(p, board.shooter.distToTarget);
      });
    yield return null;

  }

}
