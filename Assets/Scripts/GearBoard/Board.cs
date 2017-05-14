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
        new Engine(new Engine.Status() { speed = 50f, acceleration = 1f } ),
        new Rotater(new Rotater.Status() { speed = 30f, acceleration = 1f } ),
        transform
      );
    }

  }
}
