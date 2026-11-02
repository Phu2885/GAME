using System;
using Heretic.Combat;
using Heretic.Player;
using UnityEngine;

namespace Heretic.Boss
{
    /// <summary>Boss state machine states. See the transition table on <see cref="PaladinBoss"/>.</summary>
    public enum BossState { Idle, Telegraph, Attack, HeavyQte, Recover, Defeated }

    /// <summary>The attacks the boss can perform. Light attacks are dodgeable; Heavy starts the QTE.</summary>
    public enum BossAttack { GroundSlam, Lunge, Heavy }

    /// <summary>
    /// Designer-tunable boss values. Serialized on <see cref="BossArenaBootstrap"/> so edits persist in the scene.
    /// </summary>
    [Serializable]
    public class PaladinBossSettings
    {
        [Header("Body")]
        public Vector2 Size = new Vector2(1.6f, 2.2f);
        [Min(1)] public int MaxHealth = 100;

        [Header("Pattern (cycled in order)")]
        public BossAttack[] Pattern =
        {
            BossAttack.GroundSlam, BossAttack.Lunge, BossAttack.GroundSlam, BossAttack.Lunge, BossAttack.Heavy,
        };

        [Header("Timings (seconds, scaled time)")]
        [Min(0f)] public float IdleSeconds = 1.2f;
        [Min(0.1f)] public float SlamTelegraphSeconds = 0.8f;
        [Min(0.1f)] public float LungeTelegraphSeconds = 0.8f;
        [Min(0.1f)] public float HeavyTelegraphSeconds = 1.2f;
        [Min(0f)] public float RecoverSeconds = 1f;
        [Min(0f)] public float HeavyRecoverSeconds = 1.5f;

        [Header("Speeds (units/second)")]
        [Min(0.1f)] public float LungeSpeed = 16f;
        [Min(0.1f)] public float ShockwaveSpeed = 9f;

        [Header("Damage")]
        [Min(0)] public int SlamDamage = 1;
        [Min(0)] public int LungeDamage = 1;
        [Min(0)] public int HeavyFailDamage = 2;
        [Min(0)] public int CounterDamageToBoss = 25;

        [Header("Heavy attack")]
        [Range(0f, 1f)] public float ParryChance = 0.5f;

        [Header("Ground slam")]
        public Vector2 ShockwaveSize = new Vector2(0.8f, 0.7f);

        [Header("Telegraph visuals")]
        public Color SlamTelegraphColor = new Color(1f, 0.9f, 0.2f);
        public Color LungeTelegraphColor = new Color(1f, 0.5f, 0.1f);
        public Color HeavyTelegraphColor = new Color(1f, 0.1f, 0.1f);
        public Color ShockwaveColor = new Color(1f, 0.8f, 0.3f);
        [Min(0.1f)] public float FlashesPerSecond = 4f;
    }

    /// <summary>
    /// Paladin boss: simple state machine with two light attacks and one heavy attack.
    /// Implements: design/game-brief.md, MVP feature 4 (story-002-paladin-boss).
    ///
    /// Transitions:
    ///   Idle      -> Telegraph            (idle timer elapsed, next attack from the pattern)
    ///   Telegraph -> Attack               (GroundSlam / Lunge, telegraph timer elapsed)
    ///   Telegraph -> HeavyQte             (Heavy, telegraph timer elapsed, QTE triggered)
    ///   Attack    -> Recover              (slam waves gone / lunge reached the arena edge)
    ///   HeavyQte  -> Recover              (parry, counter that did not kill, or QTE failed)
    ///   HeavyQte  -> Defeated             (counter damage took health to 0)
    ///   Recover   -> Idle                 (recover timer elapsed)
    ///   any       -> Defeated             (health reached 0)
    ///
    /// Time choice: every state timer and all boss movement use scaled time (Time.deltaTime).
    /// Slow motion only runs during HeavyQte, where the boss waits for the QTE result and has no
    /// running timer, so nothing of the boss is stretched except what should be (the player's and
    /// boss's world motion). The QTE clock itself is real time and lives in story 001.
    /// The boss moves via transform in Update (no Rigidbody), so slow motion cannot make it choppy.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PaladinBoss : MonoBehaviour
    {
        private PaladinBossSettings _s;
        private SpriteRenderer _renderer;
        private Color _baseColor;
        private PlayerController _player;
        private PlayerHealth _playerHealth;
        private SlowMotionController _qte;
        private Transform[] _waves;
        private float _arenaLeft;
        private float _arenaRight;
        private float _groundY;
        private int _patternIndex;
        private float _timer;
        private float _stateTime;
        private float _facing = -1f;
        private float _lungeDirection;
        private Color _telegraphColor;

        /// <summary>Raised once when boss health reaches 0 (story 006 listens for the reward).</summary>
        public event Action BossDefeated;

        /// <summary>Current boss health.</summary>
        public int Health { get; private set; }

        /// <summary>Maximum boss health.</summary>
        public int MaxHealth => _s.MaxHealth;

        /// <summary>Current state of the state machine.</summary>
        public BossState State { get; private set; } = BossState.Idle;

        /// <summary>The attack being telegraphed or performed (meaningful outside Idle/Recover).</summary>
        public BossAttack CurrentAttack { get; private set; }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _baseColor = _renderer.color;
        }

        /// <summary>Wires the boss to the arena, the player and the QTE controller, then starts a fresh fight.</summary>
        /// <param name="settings">Tunable values (owned by the bootstrap so they persist in the scene).</param>
        /// <param name="player">Real player (story 003) that takes the boss's damage; must also have a <see cref="PlayerHealth"/>.</param>
        /// <param name="qte">Story 001 controller whose heavy-attack API and result events are used.</param>
        /// <param name="arenaLeft">World X of the left arena wall (inner face).</param>
        /// <param name="arenaRight">World X of the right arena wall (inner face).</param>
        /// <param name="groundY">World Y of the ground surface.</param>
        public void Initialize(PaladinBossSettings settings, PlayerController player, SlowMotionController qte, float arenaLeft, float arenaRight, float groundY)
        {
            _s = settings;
            transform.localScale = new Vector3(_s.Size.x, _s.Size.y, 1f);
            _player = player;
            _playerHealth = player.GetComponent<PlayerHealth>();
            _qte = qte;
            _arenaLeft = arenaLeft;
            _arenaRight = arenaRight;
            _groundY = groundY;

            _waves = new Transform[2];
            for (int i = 0; i < _waves.Length; i++)
            {
                var wave = new GameObject("Shockwave");
                var waveRenderer = wave.AddComponent<SpriteRenderer>();
                waveRenderer.sprite = _renderer.sprite;
                waveRenderer.color = _s.ShockwaveColor;
                waveRenderer.sortingOrder = _renderer.sortingOrder + 1;
                wave.transform.localScale = new Vector3(_s.ShockwaveSize.x, _s.ShockwaveSize.y, 1f);
                wave.SetActive(false);
                _waves[i] = wave.transform;
            }

            _qte.ParrySucceeded += HandleParry;
            _qte.CounterSucceeded += HandleCounter;
            _qte.QteFailed += HandleFailed;

            ResetFight();
        }

        /// <summary>Restores full health, returns the boss to its start position and restarts the pattern.</summary>
        public void ResetFight()
        {
            HideWaves();
            Health = _s.MaxHealth;
            _patternIndex = 0;
            _renderer.color = _baseColor;
            transform.position = new Vector3(_arenaRight - 3f, _groundY + _s.Size.y * 0.5f, 0f);
            EnterState(BossState.Idle, _s.IdleSeconds);
            enabled = true;
        }

        /// <summary>Freezes the boss (fight over). <see cref="ResetFight"/> resumes it.</summary>
        public void StopFight()
        {
            HideWaves();
            enabled = false;
        }

        /// <summary>Applies damage to the boss; reaching 0 health defeats it. Ignored once defeated.</summary>
        /// <param name="amount">Health to remove.</param>
        public void TakeDamage(int amount)
        {
            if (State == BossState.Defeated) return;
            Health = Mathf.Max(0, Health - amount);
            if (Health > 0) return;

            HideWaves();
            _renderer.color = Color.gray;
            EnterState(BossState.Defeated, 0f);
            BossDefeated?.Invoke();
        }

        private void OnDestroy()
        {
            if (_qte == null) return;
            _qte.ParrySucceeded -= HandleParry;
            _qte.CounterSucceeded -= HandleCounter;
            _qte.QteFailed -= HandleFailed;
        }

        private void Update()
        {
            if (_player == null || _qte == null) return;
            _timer -= Time.deltaTime;
            _stateTime += Time.deltaTime;

            switch (State)
            {
                case BossState.Idle:
                    if (_timer <= 0f && _s.Pattern.Length > 0) BeginTelegraph(_s.Pattern[_patternIndex++ % _s.Pattern.Length]);
                    break;
                case BossState.Telegraph:
                    UpdateTelegraph();
                    break;
                case BossState.Attack:
                    if (CurrentAttack == BossAttack.GroundSlam) UpdateSlam();
                    else UpdateLunge();
                    break;
                case BossState.Recover:
                    if (_timer <= 0f) EnterState(BossState.Idle, _s.IdleSeconds);
                    break;
            }
        }

        private void EnterState(BossState state, float timer)
        {
            State = state;
            _timer = timer;
            _stateTime = 0f;
            if (state != BossState.Telegraph && state != BossState.Defeated) _renderer.color = _baseColor;
        }

        private void BeginTelegraph(BossAttack attack)
        {
            CurrentAttack = attack;
            switch (attack)
            {
                case BossAttack.GroundSlam:
                    _telegraphColor = _s.SlamTelegraphColor;
                    EnterState(BossState.Telegraph, _s.SlamTelegraphSeconds);
                    break;
                case BossAttack.Lunge:
                    _telegraphColor = _s.LungeTelegraphColor;
                    EnterState(BossState.Telegraph, _s.LungeTelegraphSeconds);
                    break;
                default:
                    _telegraphColor = _s.HeavyTelegraphColor;
                    EnterState(BossState.Telegraph, _s.HeavyTelegraphSeconds);
                    break;
            }
        }

        private void UpdateTelegraph()
        {
            // The boss keeps tracking the player until the telegraph ends, then commits to a direction.
            _facing = _player.transform.position.x >= transform.position.x ? 1f : -1f;
            _renderer.color = Color.Lerp(_baseColor, _telegraphColor, Mathf.PingPong(_stateTime * _s.FlashesPerSecond * 2f, 1f));
            if (_timer > 0f) return;

            switch (CurrentAttack)
            {
                case BossAttack.GroundSlam:
                    SpawnShockwaves();
                    EnterState(BossState.Attack, 0f);
                    break;
                case BossAttack.Lunge:
                    _lungeDirection = _facing;
                    EnterState(BossState.Attack, 0f);
                    break;
                default:
                    EnterState(BossState.HeavyQte, 0f);
                    _renderer.color = _s.HeavyTelegraphColor;
                    _qte.TriggerHeavyAttack(UnityEngine.Random.value < _s.ParryChance ? QteKind.Parry : QteKind.Counter);
                    // No QTE started (e.g. the controller refused): do not wait for a result that never comes.
                    if (!_qte.IsActive) EnterState(BossState.Recover, _s.HeavyRecoverSeconds);
                    break;
            }
        }

        private void SpawnShockwaves()
        {
            Vector3 origin = transform.position;
            for (int i = 0; i < _waves.Length; i++)
            {
                float direction = i == 0 ? -1f : 1f;
                _waves[i].position = new Vector3(origin.x + direction * _s.Size.x * 0.5f, _groundY + _s.ShockwaveSize.y * 0.5f, 0f);
                _waves[i].gameObject.SetActive(true);
            }
        }

        private void UpdateSlam()
        {
            bool anyActive = false;
            for (int i = 0; i < _waves.Length; i++)
            {
                if (!_waves[i].gameObject.activeSelf) continue;

                float direction = i == 0 ? -1f : 1f;
                Vector3 position = _waves[i].position;
                position.x += direction * _s.ShockwaveSpeed * Time.deltaTime;
                _waves[i].position = position;

                if (position.x < _arenaLeft || position.x > _arenaRight)
                {
                    _waves[i].gameObject.SetActive(false);
                    continue;
                }

                anyActive = true;
                // The shockwave runs along the ground: only a grounded player is hit.
                if (_player.IsGrounded && Overlaps(position, _s.ShockwaveSize, _player.Bounds))
                {
                    _playerHealth.TakeDamage(_s.SlamDamage);
                }
            }

            if (!anyActive) EnterState(BossState.Recover, _s.RecoverSeconds);
        }

        private void UpdateLunge()
        {
            float halfWidth = _s.Size.x * 0.5f;
            // Stored in floats so the clamp and the edge test below use the very same rounded values.
            float leftLimit = _arenaLeft + halfWidth;
            float rightLimit = _arenaRight - halfWidth;
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x + _lungeDirection * _s.LungeSpeed * Time.deltaTime, leftLimit, rightLimit);
            transform.position = position;

            // The whole body hurts: it is jumped over (player bottom above the boss top) or dashed through (i-frames).
            if (Overlaps(position, _s.Size, _player.Bounds)) _playerHealth.TakeDamage(_s.LungeDamage);

            // A tolerance: an exact float comparison against the clamp limit can miss by one rounding step.
            const float EdgeTolerance = 0.001f;
            bool atEdge = position.x <= leftLimit + EdgeTolerance || position.x >= rightLimit - EdgeTolerance;
            if (atEdge) EnterState(BossState.Recover, _s.RecoverSeconds);
        }

        private void HandleParry()
        {
            if (State != BossState.HeavyQte) return;
            // Blocked: the player takes no damage.
            EnterState(BossState.Recover, _s.HeavyRecoverSeconds);
        }

        private void HandleCounter()
        {
            if (State != BossState.HeavyQte) return;
            // Blocked and the boss pays for it; TakeDamage moves to Defeated when health runs out.
            EnterState(BossState.Recover, _s.HeavyRecoverSeconds);
            TakeDamage(_s.CounterDamageToBoss);
        }

        private void HandleFailed()
        {
            if (State != BossState.HeavyQte) return;
            // The heavy attack cannot be dodged, so it ignores dash invulnerability.
            EnterState(BossState.Recover, _s.HeavyRecoverSeconds);
            _playerHealth.TakeDamage(_s.HeavyFailDamage, true);
        }

        private void HideWaves()
        {
            if (_waves == null) return;
            for (int i = 0; i < _waves.Length; i++) _waves[i].gameObject.SetActive(false);
        }

        private static bool Overlaps(Vector2 center, Vector2 size, Bounds other)
        {
            return Mathf.Abs(center.x - other.center.x) < size.x * 0.5f + other.extents.x
                && Mathf.Abs(center.y - other.center.y) < size.y * 0.5f + other.extents.y;
        }
    }
}
