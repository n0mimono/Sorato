using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace GearBoard {
  public class Board : MonoBehaviour {
    public GameObject characterRoot;
    public GameObject gearRoot;

    [Header("Options")]
    public Transform boostOrigin;
    public int killBurstNo;

    public Character character { private set; get; }

    public Kinematics kinematics { private set; get; }
    public Shooter shooter { private set; get; }

    public Status status { private set; get; }

    public DamageReceptor[] damages { private set; get; }

    public bool IsReady { private set; get; }

    void Start() {
      damages = GetComponentsInChildren<DamageReceptor> ();
    }

    public void Build(Status status) {
      this.status = status;

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

      foreach (var dmg in damages) {
        dmg.OnDamage
          .Subscribe (d => status.Damage(d.dmg));
      }
      status.OnDead.Subscribe (_ => {
        var burst = ObjectPool.GetInstance(PoolType.Kill, killBurstNo);
        burst.transform.SetParent(transform);
        burst.Activate(transform.position, transform.eulerAngles);
      });
      status.OnDead.Subscribe (_ => kinematics.Stop ());
      status.OnDead.Subscribe (_ => StartCoroutine (Sink ()));
      status.OnDead.Subscribe (_ => SoundEffect.Play (SE.Attack, 2, transform));

      var playerWind = SoundEffect.Play (SE.Move, 2, transform);
      var orgVolWing = playerWind.volume;
      var soundWind = this.UpdateAsObservable ()
        .Subscribe (_ => playerWind.volume = kinematics.speedRate * orgVolWing);
      status.OnDead.Subscribe (_ => soundWind.Dispose());

      var playerEngine = SoundEffect.Play (SE.Move, 0, transform);
      status.OnDead.Subscribe (_ => playerEngine.Stop());

      kinematics.OnBoosted
        .Subscribe(_ => SoundEffect.Play (SE.Move, 1, transform));

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

    IEnumerator Sink() {
      yield return new WaitForSeconds (4f);

      if (character != null) {
        gameObject.SetActive (false);
      }
    }

  }

  public class Status {
    public float maxHp;
    public float curHp;
    public float hp { get { return curHp / maxHp; } }
    public bool isAlive { get { return curHp > 0f; } }

    public IObservable<Status> OnDamaged { get { return damageSbj; } }
    Subject<Status> damageSbj = new Subject<Status>();

    public IObservable<int> OnDead { get { return deadSbj; } }
    Subject<int> deadSbj = new Subject<int>();

    public Status() {
    }

    public void Damage(float dmg) {
      if (curHp <= 0f) {
        return;
      }

      curHp -= dmg;
      damageSbj.OnNext (this);

      if (curHp <= 0f) {
        curHp = 0f;
        deadSbj.OnNext(1);
      }
    }
  }

}
