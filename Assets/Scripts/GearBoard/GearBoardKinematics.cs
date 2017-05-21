using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GearBoard {
  public class Booster {
    public class Status {
      public float speed;
      public float consumePerShot;
      public float recoverPerSec;
      public float deceleration;
      public float wait;
    }
    private Status status;

    public Booster(Status status) {
      this.status = status;
    }

    public float power { private set; get; }
    public float speed { private set; get; }

    public bool Consume() {
      if (power < status.consumePerShot) {
        return false;
      } else {
        speed = status.speed;
        power -= status.consumePerShot;
        status.wait = 0.5f;
        return true;
      }
    }

    public void Update(float dt) {
      status.wait -= dt;
      if (status.wait <= 0f) {
        power = Mathf.Clamp01 (power + status.recoverPerSec * dt);
      }
      speed = Mathf.Lerp (speed, 1f, status.deceleration * dt);
    }
  }

  public class Engine {
    public class Status {
      public float speed;
      public float acceleration;
    }
    private Status status;
    private Booster booster;

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

    public void Update(float dt, float addSpd) {
      speed = Mathf.Lerp (
        speed,
        stateRates [(int)state] * status.speed * addSpd,
        status.acceleration * dt
      );
    }
  }

  public class Rotater {
    public class Status {
      public float speed;
      public float acceleration;
      public float zFactor;
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
          Mathf.Sin(packedZ * Mathf.Deg2Rad) * status.zFactor
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
    public Booster booster { private set; get; }

    Transform trans;

    public const float MaxHeight = 200f;
    public const float MinHeight = 2f;

    public Kinematics(Engine engine, Rotater rotater, Booster booster, Transform tran) {
      this.engine = engine;
      this.rotater = rotater;
      this.booster = booster;
      this.trans = tran;
    }

    public void Update(float dt) {
      engine.Update (dt, booster.speed);
      rotater.Update (dt);
      booster.Update (dt);

      Apply (dt);
    }

    void Apply(float dt) {
      Vector3 angles = new Vector3 (
        rotater.rotSpeed.x,
        trans.localEulerAngles.y + rotater.rotSpeed.y * dt,
        rotater.rotSpeed.z
      );
      trans.localEulerAngles = angles;

      Vector3 position = trans.localPosition + engine.speed * trans.forward * dt;
      position.y = Mathf.Clamp (position.y, MinHeight, MaxHeight);
      trans.localPosition = position;
    }

    public override string ToString () {
      return string.Format ("Height: {0:F1}\nSpeed: {1:F1}",
        trans.position.y,
        engine.speed
      );
    }
  }

}
