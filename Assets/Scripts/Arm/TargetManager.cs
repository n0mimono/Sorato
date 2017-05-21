using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class TargetManager {
  public IObservable<DamageReceptor> targetChanged { private set; get; }
  Subject<DamageReceptor> targetSubject;

  bool isFirst;

  public TargetManager() {
    targetSubject = new Subject<DamageReceptor> ();
    targetChanged = targetSubject;

    isFirst = true;
  }

  public void Update() {
    if (isFirst) {
      var tgt = GameObject.FindObjectsOfType<DamageReceptor> ()
        .Where (r => r.owner == DamageReceptor.Owner.Enemy)
        .FirstOrDefault ();
      targetSubject.OnNext (tgt);

      isFirst = false;
    }
  }

}
