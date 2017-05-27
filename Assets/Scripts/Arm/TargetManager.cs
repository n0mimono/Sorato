using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class TargetManager {
  public IObservable<DamageReceptor> targetChanged { private set; get; }
  Subject<DamageReceptor> targetSubject;

  public IObservable<Damage> OnDamage { private set; get; }
  Subject<Damage> dmgSubject;

  DamageReceptor target;

  bool isFirst;

  public TargetManager() {
    targetSubject = new Subject<DamageReceptor> ();
    targetChanged = targetSubject;

    dmgSubject = new Subject<Damage> ();
    OnDamage = dmgSubject;

    isFirst = true;
  }

  public void Update() {
    if (isFirst) {
      target = GameObject.FindObjectsOfType<DamageReceptor> ()
        .Where (r => r.owner == DamageReceptor.Owner.Enemy)
        .FirstOrDefault ();
      target.OnDamage
        .Subscribe (d => dmgSubject.OnNext (d));
      
      targetSubject.OnNext (target);

      isFirst = false;
    }
  }

}
