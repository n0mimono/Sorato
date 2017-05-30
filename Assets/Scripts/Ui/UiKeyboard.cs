using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UiKeyboard {
  public IObservable<int> OnFire;
  public IObservable<int> OnDash;
  public IObservable<BooledVariable<float>> OnRot;
  public IObservable<Vector2> OnGesture;
  public IObservable<int> OnEngine;

  //  1  0 7
  //  2 -1 6
  //  3  4 5
  static readonly int[,] AngleCode = new int[,] {
    { 3,  4, 5 },
    { 2, -1, 6 },
    { 1,  0, 7 },
  };
  static readonly float[] Angles = new float[] {
    0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f,
  };

  public void Build() {
    var fireSubject = new Subject<int> ();
    OnFire = fireSubject;
    
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.F))
      .Subscribe (_ => fireSubject.OnNext(1));

    var dashSubject = new Subject<int> ();
    OnDash = dashSubject;

    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.R))
      .Subscribe (_ => dashSubject.OnNext(1));

    var rotSubject = new Subject<BooledVariable<float>> ();
    OnRot = rotSubject;

    Observable.EveryUpdate ()
      .Select (_ => ArrowsToAngleCode ())
      .Select (c => new BooledVariable<float> () { b = (c != -1), item = CodeToAngle (c) })
      .Subscribe (v => rotSubject.OnNext (v));

    var gesSubject = new Subject<Vector2> ();
    OnGesture = gesSubject;

    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.W))
      .Subscribe (_ => gesSubject.OnNext (Vector2.up));
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.Z))
      .Subscribe (_ => gesSubject.OnNext (Vector2.down));
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.A))
      .Subscribe (_ => gesSubject.OnNext (Vector2.left));
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.S))
      .Subscribe (_ => gesSubject.OnNext (Vector2.right));

    var engSubject = new Subject<int>();
    OnEngine = engSubject;

    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.Alpha1))
      .Subscribe (_ => engSubject.OnNext (4));
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.Alpha2))
      .Subscribe (_ => engSubject.OnNext (3));
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.Alpha3))
      .Subscribe (_ => engSubject.OnNext (2));
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.Alpha4))
      .Subscribe (_ => engSubject.OnNext (1));
    Observable.EveryUpdate ()
      .Where (_ => Input.GetKey (KeyCode.Alpha5))
      .Subscribe (_ => engSubject.OnNext (0));
    
  }

  public static int ArrowsToAngleCode() {
    var up = Input.GetKey (KeyCode.UpArrow) ? 1 : 0;
    var down = Input.GetKey (KeyCode.DownArrow) ? -1 : 0;
    var left = Input.GetKey (KeyCode.LeftArrow) ? -1 : 0;
    var right = Input.GetKey (KeyCode.RightArrow) ? 1 : 0;

    var upper = up + down; // -1, 0, 1
    var righter = left + right; // -1, 0, 1
    var code = AngleCode[upper + 1, righter + 1];

    return code;
  }

  public static float CodeToAngle(int code) {
    if (code != -1) {
      return Angles [code];
    } else {
      return 0f;
    }
  }

}
