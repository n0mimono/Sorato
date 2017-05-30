using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GearBoard {
  public class Board : MonoBehaviour {
    public GameObject characterRoot;
    public GameObject gearRoot;

    [Header("Options")]
    public Transform boostOrigin;

    public Character character { private set; get; }

    public Kinematics kinematics { private set; get; }
    public Shooter shooter { private set; get; }

    public IEnumerable<DamageReceptor> damages {
      get {
        return GetComponentsInChildren<DamageReceptor> ();
      }
    }

    public bool IsReady { private set; get; }

    public void Build() {
      kinematics = new Kinematics (
        new Engine(new Engine.Status() {
          speed = 30f,
          acceleration = 1f
        }),
        new Rotater(new Rotater.Status() {
          speed = 30f,
          acceleration = 1f,
          zFactor = 2f
        }),
        new Booster(new Booster.Status() {
          speed = 3f,
          consumePerShot = 0.2f,
          recoverPerSec = 0.2f,
          deceleration = 1f,
          origin = boostOrigin
        }),
        transform
      );

      shooter = GetComponentInChildren<Shooter> ();
      if (shooter != null) {
        shooter.Build (new Shooter.Status () {
          gauge = 0,
          max = 20,
          recoverPerSec = 3f,
        });
      }

      character = characterRoot.GetComponentInChildren<Character> ();
      if (character != null) {
        kinematics.engine.OnStateChanged
          .Subscribe (s => character.SetSpeed (Mathf.Max (0, 2 - (int)s)));

        foreach (var dmg in damages) {
          dmg.OnDamage
            .Subscribe (d => character.InvokeDamage());
        }

        shooter.OnShooted
          .Where (b => b)
          .Subscribe (_ => character.InvokeFire ());
      }

      IsReady = true;
    }

    void Update() {
      if (IsReady) {
        UpdateState ();
      }
    }

    void UpdateState() {
      kinematics.Update (Time.deltaTime);
      shooter.UpdateState(Time.deltaTime);
    }

  }
}
