#region Includes
using UnityEngine;
using ToolBox.Tags;
#endregion

namespace Autohand.GazeInteraction
{
    public class GazeInteractor : MonoBehaviour
    {
        #region Variables

        [Header("Configuration")]
        [SerializeField] private GameObject reticle;
        [SerializeField] private float _maxDetectionDistance;
        [SerializeField] private float _minDetectionDistance;
        [SerializeField] private float _timeToActivate = 1.0f;

        [Tooltip("Raycast will only hit selected layer")]
        [SerializeField] private LayerMask _layerMask;
        [Tooltip("Raycast will only hit selected tags")]
        [SerializeField] private Tag _tagMask;

        private Ray _ray;
        private RaycastHit _hit;
        private Vector3 GazePoint;

        private GazeReticle _reticle;
        private GazeInteractable _interactable;

        private float _enterStartTime;

        #endregion

        private void Start()
        {
            // var instance = ResourcesManager.GetPrefab(ResourcesManager.FILE_PREFAB_RETICLE);
            // var reticle = instance.GetComponent<GazeReticle>();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(reticle == null) { throw new System.Exception("Missing GazeReticle"); }
#endif

            _reticle = Instantiate(reticle.GetComponent<GazeReticle>());
            _reticle.SetInteractor(this);
        }
        private void Update()
        {
            _ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(_ray, out _hit, _maxDetectionDistance, _layerMask))
            {
              //raycast will stop when hitting objects outside tagMask
                if (!_hit.collider.transform.HasTag(_tagMask))
                {
                  return;
                }

                var distance = Vector3.Distance(transform.position, _hit.transform.position);
                if (distance < _minDetectionDistance)
                {
                    _reticle.Enable(false);
                    Reset();
                    return;
                }

                _reticle.SetTarget(_hit);
                _reticle.Enable(true);

                var interactable = _hit.collider.transform.GetComponent<GazeInteractable>();
                if(interactable == null)
                {
                    Reset();
                    return;
                }

                if (interactable != _interactable)
                {
                    Reset();

                    _enterStartTime = Time.time;

                    _interactable = interactable;
                    _interactable.GazeEnter(this, _hit.point);
                }

                _interactable.GazeStay(this, _hit.point);
                GazePoint = _hit.transform.position;

                if (_interactable.IsActivable && !_interactable.IsActivated)
                {

                    var timeToActivate = (_enterStartTime + _timeToActivate) - Time.time;
                    var progress = 1 - (timeToActivate / _timeToActivate);
                    progress = Mathf.Clamp(progress, 0, 1);
                    _reticle.SetProgress(progress);

                    if (progress == 1)
                    {

                        _reticle.Enable(false);
                        _interactable.Activate();
                    }
                }

                return;
            }

            _reticle.Enable(false);
            Reset();
        }

        private void Reset()
        {
            _reticle.SetProgress(0);

            if(_interactable == null) { return; }
            _interactable.GazeExit(this);
            _interactable = null;
        }

        public Vector3 getGazePoint()
        {
          return GazePoint;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * _maxDetectionDistance);
        }
#endif
    }
}
