using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using GearBoard;
using UniRx;
using UniRx.Triggers;

public class GameManager : MonoBehaviour {
  public Board board;
  public BoardNpc[] npcs;

  UiManager ui;
  TargetManager tgtMan;

  [HideInInspector][SerializeField] new
  CameraManager camera;

  Subject<ResultInfo> gameOver;

  IEnumerator Start() {
    gameOver = new Subject<ResultInfo> ();

    yield return StartCoroutine (Build ());
    yield return StartCoroutine (StartGame ());
  }

  IEnumerator Build() {
    yield return StartCoroutine (SceneStack.LoadSceneAsync ("SoratoUi"));

    ui = GameObject.FindObjectOfType<UiManager> ();
    yield return StartCoroutine(ui.Build());

    board.Build (new Status () {
      maxHp = 20f,
      curHp = 20f,
    });
    foreach (var npc in npcs) {
      npc.Build ();
    }
    yield return null;

    camera = GameObject.FindObjectOfType<CameraManager> ();
    camera.Build ();
    yield return null;

    tgtMan = new TargetManager ();
    tgtMan.targetChanged
      .Subscribe (t => { 
        board.shooter.SetTarget (t.receptor);
      });
    foreach (var c in tgtMan.candidates) {
      var uiTarget = ui.CreateTarget ();
      c.OnPositionChanged.Subscribe (p => uiTarget.UpdateTargetPosition (p));
      c.OnDistChanged.Subscribe (d => uiTarget.UpdateTargetDist (d));
      c.OnTargetChanged.Subscribe (b => uiTarget.SetActiveTarget (b));
      c.board.status.OnDamaged.Subscribe (d => uiTarget.dmgImage.fillAmount = d.hp);
    }
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
        ui.dashFlash.SetActive(board.kinematics.booster.full);
      });
    ui.fireGauge.UpdateAsObservable ()
      .Subscribe (_ => {
        ui.fireGauge.fillAmount = board.shooter.status.power;
        ui.fireText.text = board.shooter.status.count.ToString();
        ui.fireFlash.SetActive(board.shooter.status.full);
      });
    yield return null;

    ui.UpdateAsObservable ()
      .Subscribe (_ => tgtMan.UpdateTgtDist (board.shooter.distToTarget));
    ui.extra.onClick.AddListener (() => {
      var post = camera.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>();
      post.enabled = !post.enabled;
    });
    yield return null;

    board.kinematics.engine.SetState (Engine.State.Stop);
    board.kinematics.OnBoosted
      .Where (b => b)
      .Subscribe (_ => ui.chara.SetFace (CharaFace.Cool));

    board.status.OnDamaged.Subscribe (s => ui.hpBar.fillAmount = s.hp);
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

    board.status.OnDead
      .Subscribe (_ => gameOver.OnNext(new ResultInfo() { win = false, target = board.transform } ));
    var primeNpcBoard = npcs.FirstOrDefault ().board;
    primeNpcBoard.status.OnDead
      .Subscribe (_ => gameOver.OnNext (new ResultInfo() { win = true, target = primeNpcBoard.transform }));
    gameOver.Take (1).Subscribe (r => StartCoroutine (Result (r)));
    yield return null;

    ui.baseChanged
      .Select (a => new Vector3 (-a.y, a.x, 0f))
      .Subscribe (a => camera.current.Rotate (a));
    this.LateUpdateAsObservable ()
      .Subscribe (_ => {
        camera.current.SetTargetPosition (board.transform.position); 
        camera.UpdateCamera ();

        foreach (var c in tgtMan.candidates) {
          c.UpdateScreenPosition(camera.ScreenPosition(c.receptor.position));
        }

        var bPos = camera.ScreenPosition(board.transform.position).pos;
        tgtMan.Update(bPos);
      });
    yield return null;

  }

  IEnumerator StartGame() {
    yield return StartCoroutine (SceneStack.Open ());

    ui.hpBar.GetComponent<UiAutoFill> ().StartFill ();
    yield return null;

    foreach (var npc in npcs) {
      npc.StartNpc ();
    }

    yield return null;
    SceneStack.SetActive (true);
  }

  IEnumerator Result(ResultInfo info) {
    yield return null;
    SceneStack.SetActive (false);

    yield return StartCoroutine (SceneStack.Close ());

    yield return null;
    SceneStack.MoveScene ("Title");
  }

}

public struct ResultInfo {
  public bool win;
  public Transform target;
}
