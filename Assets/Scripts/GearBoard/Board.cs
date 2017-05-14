using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GearBoard {
  public class Board : MonoBehaviour {
    public GameObject characterRoot;
    public GameObject gearRoot;

    public Kinematics kinematics { private set; get; }

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
          speed = 2f,
          consumePerShot = 0.2f,
          recoverPerSec = 0.2f,
          deceleration = 1f
        }),
        transform
      );
    }

  }
}
