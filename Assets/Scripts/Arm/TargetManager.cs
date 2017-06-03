using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class TargetManager {
  public IObservable<Candidate> targetChanged { private set; get; }
  Subject<Candidate> targetSubject;

  public IObservable<Damage> OnDamage { private set; get; }
  Subject<Damage> dmgSubject;

  public class Candidate {
    public GearBoard.Board board;
    public DamageReceptor receptor;
    public Vector3 scrPosition;

    public IObservable<ScreenPoint> OnPositionChanged { private set; get;}
    Subject<ScreenPoint> scrSbj;
    public IObservable<float> OnDistChanged { private set; get;}
    Subject<float> distSbj;
    public IObservable<bool> OnTargetChanged { private set; get;}
    Subject<bool> tgtSbj;

    public Candidate() {
      scrSbj = new Subject<ScreenPoint>();
      OnPositionChanged = scrSbj;

      distSbj = new Subject<float>();
      OnDistChanged = distSbj;

      tgtSbj = new Subject<bool>();
      OnTargetChanged = tgtSbj;

      OnPositionChanged.Subscribe(p => scrPosition = p.pos);
    }

    public void UpdateScreenPosition (ScreenPoint point) {
      scrSbj.OnNext (point);
    }

    public void UpdateDist (float dist) {
      distSbj.OnNext (dist);
    }

    public void SetTarget(bool isTarget) {
      tgtSbj.OnNext (isTarget);
    }
  }
  public List<Candidate> candidates { private set; get; }
  public Candidate target { private set; get; }

  public TargetManager() {
    targetSubject = new Subject<Candidate> ();
    targetChanged = targetSubject;

    dmgSubject = new Subject<Damage> ();
    OnDamage = dmgSubject;

    candidates = GameObject.FindObjectsOfType<DamageReceptor> ()
      .Where (r => r.owner == DamageReceptor.Owner.Enemy)
      .Select(r => new Candidate() {
        board = r.GetComponentInParent<GearBoard.Board>(),
        receptor = r
      })
      .ToList ();
    foreach (var c in candidates) {
      c.receptor.OnDamage
        .Where (_ => c == target)
        .Subscribe (d => dmgSubject.OnNext (d));
    }

    targetSubject.Subscribe (t => {
      foreach (var c in candidates) {
        bool isTarget = (c == t);
        c.SetTarget(isTarget);
      }
    });
  }

  public void Update(Vector3 center) {
    var scrTgt = CalcScreenTarget (center);

    var prev = target;

    float minDist = 100000f;
    foreach (var c in candidates) {
      float scrDist = Vector3.Distance (c.scrPosition, scrTgt);
      if (minDist > scrDist) {
        target = c;
        minDist = scrDist;
      }
    }

    if (target != prev) {
      targetSubject.OnNext (target);
    }
  }

  public void UpdateTgtDist(float dist) {
    if (target != null) {
      target.UpdateDist (dist);
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
