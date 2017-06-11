using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class Shooter : MonoBehaviour {
  public Transform[] slotOrigins;
  public int arrowNo;

  public class Slot {
    public Transform origin;
  }
  List<Slot> slots;
  int curSlot;

  public class Status {
    public float gauge;
    public float max;
    public float recoverPerSec;

    public int count { get { return Mathf.FloorToInt(gauge); } }
    public float power { get { return gauge / max; } } 
    public bool full { private set; get; }

    public float wait;

    public bool CanShoot {
      get {
        return gauge >= 1f;
      }
    }

    public void Update(float dt) {
      wait -= dt;
      if (wait <= 0f) {
        var rawGauge = gauge + recoverPerSec * dt;
        full = rawGauge >= max;
        gauge = Mathf.Min (rawGauge, max);
      }
    }

    public void Shoot() {
      gauge -= 1f;
      wait = 0.5f;
      full = false;
    }
  }
  public Status status { private set; get; }

  public DamageReceptor target { private set; get; }
  public float distToTarget {
    get {
      if (target != null) {
        return Vector3.Distance (transform.position, target.position);
      } else {
        return 0f;
      }
    }
  }

  public IObservable<bool> OnShooted { get { return shootSubject; } }
  Subject<bool> shootSubject = new Subject<bool>();

  public void Build(Status status) {
    slots = slotOrigins
      .Select (o => new Slot () {
        origin = o
      })
      .ToList ();

    this.status = status;

    shootSubject
      .Where (b => b)
      .Subscribe (_ => Shoot ());
  }

  public void SetTarget(DamageReceptor target) {
    this.target = target;
  }

  public void UpdateState(float dt) {
    status.Update (dt);
  }

  public void WillShoot() {
    shootSubject.OnNext (status.CanShoot);
  }

  public void Shoot() {
    status.Shoot ();
    var origin = slots [curSlot].origin;

    var arrow = ObjectPool.GetInstance (PoolType.Arrow, arrowNo)
      .GetComponent<Arrow> ();
    arrow.Build (new Arrow.Params () {
      target = target,
      origin = origin
    });
    arrow.Shoot ();

    curSlot = (curSlot + 1) % slots.Count;
  }

}
