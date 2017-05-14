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

    ui.rotChanged
      .Subscribe(v => board.kinematics.rotater.SetRotate(v.b, v.item.eulerAngles.z));
    ui.engChanged
      .Subscribe(i => board.kinematics.engine.SetState((Engine.State)i));
    yield return null;

    ui.baseChanged
      .Select (a => new Vector3 (-a.y, a.x, 0f))
      .Subscribe (a => camera.current.Rotate (a));
    board.LateUpdateAsObservable ()
      .Subscribe (_ => camera.current.SetTargetPosition (board.transform.position));
    camera.LateUpdateAsObservable ()
      .Subscribe (_ => camera.UpdateCamera ());
    yield return null;

    board.kinematics.engine.SetState (Engine.State.Stop);
    yield return null;

    board.UpdateAsObservable ()
      .Subscribe (_ => board.kinematics.Update (Time.deltaTime));
    yield return null;
  }

}
