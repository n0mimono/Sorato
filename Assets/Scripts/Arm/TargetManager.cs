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

  public DamageReceptor target { private set; get; }

  public class Candidate {
    public DamageReceptor receptor;
    public Vector3 scrPosition;

    public void UpdateScreenPosition (Vector3 pos) {
      scrPosition = pos;
    }
  }
  public List<Candidate> candidates { private set; get; }

  public TargetManager() {
    targetSubject = new Subject<DamageReceptor> ();
    targetChanged = targetSubject;

    dmgSubject = new Subject<Damage> ();
    OnDamage = dmgSubject;

    candidates = GameObject.FindObjectsOfType<DamageReceptor> ()
      .Where (r => r.owner == DamageReceptor.Owner.Enemy)
      .Select(r => new Candidate() { receptor = r })
      .ToList ();
    foreach (var c in candidates.Select(c => c.receptor)) {
      c.OnDamage
        .Where (_ => c == target)
        .Subscribe (d => dmgSubject.OnNext (d));
    }
  }

  public void Update(Vector3 center) {
    var scrTgt = CalcScreenTarget (center);

    var prev = target;

    float minDist = 100000f;
    foreach (var c in candidates) {
      float scrDist = Vector3.Distance (c.scrPosition, scrTgt);
      if (minDist > scrDist) {
        target = c.receptor;
        minDist = scrDist;
      }
    }

    if (target != prev) {
      targetSubject.OnNext (target);
    }
  }

  Vector3 CalcScreenTarget(Vector3 center) {
    var screen = new Vector3 (Screen.width , Screen.height, 0f);
    var offset = center - screen * 0.5f;

    var corner = Vector3.zero;
    if (offset.x >= 0f && offset.y >= 0f) {
      corner = new Vector3 (0f, 0f);
    } else if (offset.x <= 0f && offset.y >= 0f) {
      corner = new Vector3 (screen.x, 0f);
    } else if (offset.x <= 0f && offset.y <= 0f) {
      corner = new Vector3 (screen.x, screen.y);
    } else if (offset.x >= 0f && offset.y <= 0f) {
      corner = new Vector3 (0f, screen.y);
    }
    var scrTgt = (center + corner) * 0.5f;

    return scrTgt;
  }

}
