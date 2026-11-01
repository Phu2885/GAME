using System;
using UnityEngine;

namespace Heretic.Player
{
    /// <summary>Tunable animation values. Serialized on <see cref="PlayerTestBootstrap"/> so edits persist in the scene.</summary>
    [Serializable]
    public class PlayerAnimationSettings
    {
        [Min(0.1f)] public float IdleFps = 3f;
        [Min(0.1f)] public float RunFps = 10f;
        [Min(0.1f)] public float DeathFps = 6f;
        [Range(0f, 1f)] public float InvulnerableAlpha = 0.5f;
    }

    /// <summary>
    /// Code-driven frame switching for the 14-frame demon sheet (no AnimatorController).
    /// Frame order: 0-1 idle, 2-5 run, 6 jump, 7 dash, 8-10 attack, 11-13 death. Flips the sprite to follow facing.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        private enum Clip { Idle, Run, Jump, Dash, Attack, Death }

        private const int IdleFirst = 0;
        private const int RunFirst = 2;
        private const int JumpFrame = 6;
        private const int DashFrame = 7;
        private const int AttackFirst = 8;
        private const int DeathFirst = 11;

        private PlayerAnimationSettings _s;
        private Sprite[] _frames;
        private PlayerController _controller;
        private PlayerHealth _health;
        private SpriteRenderer _renderer;
        private Clip _clip = Clip.Idle;
        private float _clipTime;

        /// <summary>Sets up the animator. Call right after AddComponent.</summary>
        /// <param name="settings">Tunable values (owned by the bootstrap).</param>
        /// <param name="frames">The 14 sheet sprites in order (demon_0 ... demon_13).</param>
        /// <param name="controller">Source of the movement state.</param>
        /// <param name="health">Source of the dead / invulnerable state.</param>
        public void Initialize(PlayerAnimationSettings settings, Sprite[] frames, PlayerController controller, PlayerHealth health)
        {
            _s = settings;
            _frames = frames;
            _controller = controller;
            _health = health;
            _renderer = GetComponent<SpriteRenderer>();
        }

        // LateUpdate so the controller has already updated this frame's state.
        private void LateUpdate()
        {
            Clip clip = ChooseClip();
            if (clip != _clip)
            {
                _clip = clip;
                _clipTime = 0f;
            }
            else
            {
                _clipTime += Time.deltaTime;
            }

            _renderer.sprite = _frames[FrameFor(_clip)];
            _renderer.flipX = _controller.Facing < 0f;

            Color color = _renderer.color;
            color.a = !_health.IsDead && _health.IsInvulnerable ? _s.InvulnerableAlpha : 1f;
            _renderer.color = color;
        }

        private Clip ChooseClip()
        {
            if (_health.IsDead) return Clip.Death;
            if (_controller.IsDashing) return Clip.Dash;
            if (_controller.IsAttacking) return Clip.Attack;
            if (!_controller.IsGrounded) return Clip.Jump;
            return Mathf.Abs(_controller.MoveInput) > 0.01f ? Clip.Run : Clip.Idle;
        }

        private int FrameFor(Clip clip)
        {
            switch (clip)
            {
                case Clip.Run: return RunFirst + (int)(_clipTime * _s.RunFps) % 4;
                case Clip.Jump: return JumpFrame;
                case Clip.Dash: return DashFrame;
                // The strike frame (9) is shown exactly while the hit is active, so the slash arc matches the hit.
                case Clip.Attack: return AttackFirst + _controller.AttackPhase;
                case Clip.Death: return DeathFirst + Mathf.Min((int)(_clipTime * _s.DeathFps), 2);
                default: return IdleFirst + (int)(_clipTime * _s.IdleFps) % 2;
            }
        }
    }
}
