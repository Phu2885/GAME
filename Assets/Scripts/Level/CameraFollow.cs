using System;
using UnityEngine;

namespace Heretic.Level
{
    /// <summary>Tunable camera values. Serialized on <see cref="LevelOneBootstrap"/> so edits persist in the scene.</summary>
    [Serializable]
    public class CameraFollowSettings
    {
        [Min(1f)] public float OrthographicSize = 6f;
        [Min(0.01f), Tooltip("Smoothing time of the horizontal follow (lower = tighter).")]
        public float HorizontalSmoothSeconds = 0.15f;
        [Min(0.01f), Tooltip("Smoothing time of the vertical follow (higher = gentler).")]
        public float VerticalSmoothSeconds = 0.5f;
        [Tooltip("How far above the player's feet the camera aims.")]
        public float VerticalOffset = 2f;
    }

    /// <summary>
    /// Orthographic camera that follows the player, smoothed, clamped to the level bounds (story 005, brief MVP feature 2).
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        private const float CameraZ = -10f;

        private CameraFollowSettings _s;
        private Camera _camera;
        private Transform _target;
        private Rect _bounds;
        private float _velocityX;
        private float _velocityY;

        /// <summary>Sets up the follow and snaps the camera onto the target. Call right after AddComponent.</summary>
        /// <param name="settings">Tunable values (owned by the bootstrap).</param>
        /// <param name="target">What to follow (the player).</param>
        /// <param name="levelBounds">World rectangle the view must stay inside.</param>
        public void Initialize(CameraFollowSettings settings, Transform target, Rect levelBounds)
        {
            _s = settings;
            _target = target;
            _bounds = levelBounds;
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = _s.OrthographicSize;
            transform.position = new Vector3(ClampX(target.position.x), ClampY(target.position.y + _s.VerticalOffset), CameraZ);
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 position = transform.position;
            position.x = Mathf.SmoothDamp(position.x, ClampX(_target.position.x), ref _velocityX, _s.HorizontalSmoothSeconds);
            position.y = Mathf.SmoothDamp(position.y, ClampY(_target.position.y + _s.VerticalOffset), ref _velocityY, _s.VerticalSmoothSeconds);
            position.z = CameraZ;
            transform.position = position;
        }

        private float ClampX(float x)
        {
            float half = _camera.orthographicSize * _camera.aspect;
            return ClampAxis(x, _bounds.xMin + half, _bounds.xMax - half);
        }

        private float ClampY(float y)
        {
            float half = _camera.orthographicSize;
            return ClampAxis(y, _bounds.yMin + half, _bounds.yMax - half);
        }

        // A level smaller than the view on this axis keeps the camera centred.
        private static float ClampAxis(float value, float min, float max)
        {
            return min > max ? (min + max) * 0.5f : Mathf.Clamp(value, min, max);
        }
    }
}
