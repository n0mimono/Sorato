using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DamageReceptor : MonoBehaviour {
  public IObservable<Damage> OnDamage { get { return dmgSubject; } }
  Subject<Damage> dmgSubject = new Subject<Damage>();

  public Vector3 position {
    get {
      return transform.position;
    }
  }

  public enum Owner {
    None,
    Player,
    Enemy,
  }
  public Owner owner;

  public void Receive(DamageSource source) {
    dmgSubject.OnNext (new Damage () {
      dmg = 1f
    });
  }

}

public struct Damage {
  public float dmg;
}
