using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public float wait;

    public bool CanShoot {
      get {
        return gauge >= 1f;
      }
    }

    public void Update(float dt) {
      wait -= dt;
      if (wait <= 0f) {
        gauge = Mathf.Min (gauge + recoverPerSec * dt, max);
      }
    }

    public void Shoot() {
      gauge -= 1f;
      wait = 0.5f;
    }
  }
  public Status status { private set; get; }

  public DamageReceptor target { private set; get; }
  public float distToTarget {
    get {
      return Vector3.Distance (transform.position, target.position);
    }
  }

  public void Build(Status status) {
    slots = slotOrigins
      .Select (o => new Slot () {
        origin = o
      })
      .ToList ();

    this.status = status;
  }

  public void SetTarget(DamageReceptor target) {
    this.target = target;
  }

  public void UpdateState(float dt) {
    status.Update (dt);
  }

  public void WillShoot() {
    if (status.CanShoot) {
      Shoot ();
    }
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
