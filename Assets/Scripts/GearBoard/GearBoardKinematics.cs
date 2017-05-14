using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GearBoard {
  public class Engine {
    public class Status {
      public float speed;
      public float acceleration;
    }
    private Status status;

    public enum State {
      TopForward = 0,
      SecondForward,
      Stop,
      SecondBack,
      TopBack
    }
    float[] stateRates = new float[] {
      1f,
      0.5f,
      0f,
      -0.25f,
      -0.5f
    };

    public State state { private set; get; }
    public float speed { private set; get; }

    public Engine(Status status) {
      this.status = status;
    }

    public void SetState(State state) {
      this.state = state;
    }

    public void Update(float dt) {
      speed = Mathf.Lerp (
        speed,
        stateRates [(int)state] * status.speed,
        status.acceleration * dt
      );
    }
  }

  public class Rotater {
    public class Status {
      public float speed;
      public float acceleration;
    }
    private Status status;

    public Rotater(Status status) {
      this.status = status;
    }

    public Vector3 rotSpeed {
      get {
        return rotBase * status.speed;
      }
    }
    public Vector3 rotBase { private set; get; }
    public Vector3 rotTarget { private set; get; }

    public void SetRotate(bool isActive, float packedZ) {
      if (isActive) {
        this.rotTarget = new Vector3 (
          Mathf.Cos(packedZ * Mathf.Deg2Rad),
          -Mathf.Sin(packedZ * Mathf.Deg2Rad),
          0f
        );
      } else {
        this.rotTarget = Vector3.zero;
      }
    }

    public void Update(float dt) {
      rotBase = Vector3.Lerp (
        rotBase,
        rotTarget,
        status.acceleration * dt
      );

    }
  }

  public class Kinematics {
    public Engine engine { private set; get; }
    public Rotater rotater { private set; get; }

    Transform trans;

    public Kinematics(Engine engine, Rotater rotater, Transform tran) {
      this.engine = engine;
      this.rotater = rotater;
      this.trans = tran;
    }

    public void Update(float dt) {
      engine.Update (dt);
      rotater.Update (dt);

      Apply (dt);
    }

    void Apply(float dt) {
      trans.localEulerAngles = new Vector3 (
        rotater.rotSpeed.x,
        trans.localEulerAngles.y + rotater.rotSpeed.y * dt,
        0f
      );
      trans.localPosition += engine.speed * trans.forward * dt;
    }
  }

}
