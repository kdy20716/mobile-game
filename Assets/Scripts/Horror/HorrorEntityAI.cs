using System;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorEscape
{
    public class HorrorEntityAI : MonoBehaviour
    {
        public static event Action OnPlayerCaught;

        public enum AIState { Patrol, Search, Chase }
        public AIState CurrentState { get; private set; } = AIState.Patrol;

        [Header("Movement")]
        [SerializeField] private float patrolSpeed = 2.4f;
        [SerializeField] private float chaseSpeed = 5.2f;
        [SerializeField] private List<Transform> patrolPoints = new List<Transform>();

        [Header("Detection")]
        [SerializeField] private float sightRange = 14f;
        [SerializeField] private float sightAngle = 75f;
        [SerializeField] private float hearDistance = 8f;

        [Header("Visuals & Sound")]
        [SerializeField] private Light entityEyeLight;
        [SerializeField] private Transform entityModel;

        private CharacterController _cc;
        private Transform _playerTransform;
        private int _currentPatrolIndex = 0;
        private float _searchTimer = 0f;
        private Vector3 _lastKnownPlayerPos;

        public float DistanceToPlayer { get; private set; } = 999f;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        private void Start()
        {
            if (HorrorPlayerController.Instance != null)
            {
                _playerTransform = HorrorPlayerController.Instance.transform;
            }
        }

        private void Update()
        {
            if (_playerTransform == null)
            {
                if (HorrorPlayerController.Instance != null)
                    _playerTransform = HorrorPlayerController.Instance.transform;
                return;
            }

            DistanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // Jumpscare trigger
            if (DistanceToPlayer < 1.4f)
            {
                CatchPlayer();
                return;
            }

            CheckPlayerDetection();

            switch (CurrentState)
            {
                case AIState.Patrol:
                    HandlePatrol();
                    break;
                case AIState.Chase:
                    HandleChase();
                    break;
                case AIState.Search:
                    HandleSearch();
                    break;
            }
        }

        private void CheckPlayerDetection()
        {
            Vector3 toPlayer = _playerTransform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, toPlayer);

            // 1. Sight detection
            if (DistanceToPlayer < sightRange && angle < sightAngle)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 1.5f, toPlayer.normalized, out hit, sightRange))
                {
                    if (hit.collider.GetComponentInParent<HorrorPlayerController>() != null)
                    {
                        // Spotted player!
                        SetChaseState(_playerTransform.position);
                        return;
                    }
                }
            }

            // 2. Sound detection (sprinting or very close)
            bool playerRunning = HorrorPlayerController.Instance != null && HorrorPlayerController.Instance.IsRunning;
            float detectionRadius = playerRunning ? hearDistance * 1.5f : hearDistance * 0.5f;

            if (DistanceToPlayer < detectionRadius)
            {
                SetChaseState(_playerTransform.position);
                return;
            }

            // Lose player
            if (CurrentState == AIState.Chase && DistanceToPlayer > sightRange * 1.5f)
            {
                CurrentState = AIState.Search;
                _searchTimer = 4f;
            }
        }

        private void SetChaseState(Vector3 pos)
        {
            CurrentState = AIState.Chase;
            _lastKnownPlayerPos = pos;
            if (entityEyeLight != null) entityEyeLight.color = Color.red;
        }

        private void HandlePatrol()
        {
            if (entityEyeLight != null) entityEyeLight.color = new Color(0.8f, 0.4f, 0.1f);

            if (patrolPoints == null || patrolPoints.Count == 0) return;

            Transform targetPoint = patrolPoints[_currentPatrolIndex];
            MoveTowards(targetPoint.position, patrolSpeed);

            if (Vector3.Distance(transform.position, targetPoint.position) < 2f)
            {
                _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Count;
            }
        }

        private void HandleChase()
        {
            MoveTowards(_playerTransform.position, chaseSpeed);
        }

        private void HandleSearch()
        {
            _searchTimer -= Time.deltaTime;
            MoveTowards(_lastKnownPlayerPos, patrolSpeed);

            if (_searchTimer <= 0f)
            {
                CurrentState = AIState.Patrol;
            }
        }

        private void MoveTowards(Vector3 targetPos, float speed)
        {
            Vector3 dir = (targetPos - transform.position);
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 6f);
                _cc.Move(dir.normalized * speed * Time.deltaTime + Vector3.down * 9.8f * Time.deltaTime);
            }
        }

        private void CatchPlayer()
        {
            enabled = false;
            OnPlayerCaught?.Invoke();
        }

        public void SetupAI(List<Transform> waypoints, Light eyeLight)
        {
            patrolPoints = waypoints;
            entityEyeLight = eyeLight;
        }
    }
}
