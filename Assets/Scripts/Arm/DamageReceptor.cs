using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DamageReceptor : MonoBehaviour {
  public IObservable<Damage> OnDamage { private set; get; }
  Subject<Damage> dmgSubject;

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

  void Awake() {
    dmgSubject = new Subject<Damage> ();
    OnDamage = dmgSubject;
  }

  public void Receive(DamageSource source) {
    dmgSubject.OnNext (new Damage () {
      dmg = 0
    });
  }

}

public struct Damage {
  public int dmg;
}
