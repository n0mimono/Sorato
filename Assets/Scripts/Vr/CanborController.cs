using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;

public class CanborController {
  public IObservable<BooledVariable<float>> OnRot;
  public IObservable<Vector2> OnGesture;

  public void Build() {
    var triggered = false;

    var delta = Vector2.zero;

    Observable
      .EveryUpdate()
      .Subscribe(_ => {
        var pos = new Vector2(
          2f * Input.mousePosition.x / Screen.width - 1f,
          2f * Input.mousePosition.y / Screen.height - 1f
        );
        delta = pos;
      });

    Observable
      .EveryUpdate()
      .Select(_ => Input.GetAxis("Fire1") > 0.5)
      .Subscribe(b => triggered = b);

    var sbjGesture = new Subject<Vector2>();
    OnGesture = sbjGesture;

    Observable
      .EveryUpdate()
      .Where(_ => triggered)
      .Subscribe(_ => sbjGesture.OnNext(0.5f * delta));
    
    var rotSubject = new Subject<BooledVariable<float>>();
    OnRot = rotSubject;

    Observable
      .EveryUpdate()
      .Where(_ => !triggered)
      .Select(_ => {
        if (delta.magnitude < 0.2f) {
          return new BooledVariable<float> { b = false, item = 0f };
        } else {
          var ang = Vector2.Angle(Vector2.up, delta.normalized);
          ang = delta.x <= 0 ? ang : 360f - ang;
          return new BooledVariable<float> { b = true, item = ang };
        }
      }).Subscribe(v => rotSubject.OnNext(v));
  }

}
