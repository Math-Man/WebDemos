using System;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(SphericalMotion))]
    public class SphericalMover : MonoBehaviour
    {
        private SphericalMotion _sm;
        public SphericalMotion GetMotionScript() => _sm;
        [SerializeField] public Vector3 forceTargetCartesian;
        [SerializeField] public SphericalCoords forceTargetSpherical;

        //Demo fields, just remove these fields when using in a project
        [SerializeField] private Transform targetMarkerSpherical;
        [SerializeField] private Transform targetMarkerCartesian;
        [SerializeField] private Transform radialCenterSpherical;
        [SerializeField] private Transform radialCenterCartesian;
        
        private void Awake()
        {
            _sm = GetComponent<SphericalMotion>();
        }

        public void MoveTowardsCartesianPosition(Vector3 position)
        {
            var spc = SphericalCoords.FromCartesian(position);

              var forceVector = SphericalCoords.GetSphericalDirectionToFromSphericalVector3(spc, _sm.sphericalPositionCartesian);
            _sm.ApplySphericalForce(forceVector.x, forceVector.y, forceVector.z, 0.02f);
        }
        
        public void MoveTowardsSphericalPosition(SphericalCoords spc)
        {
            var forceVector = SphericalCoords.GetSphericalDirectionToFromSphericalVector3(spc, _sm.sphericalPositionCartesian);
            _sm.ApplySphericalForce(forceVector.x, forceVector.y, forceVector.z, 0.02f);
        }
        
        private void Update()
        {
            // if (Input.GetKey(KeyCode.Space))
            // {
            //     MoveTowardsCartesianPosition(forceTargetCartesian);
            // }
            //
            // if (Input.GetKey(KeyCode.J))
            // {
            //     MoveTowardsSphericalPosition(forceTargetSpherical);
            // }

            
            //Demo visual stuff remove in real project
            targetMarkerCartesian.position = _sm.worldOffset + forceTargetCartesian;
            targetMarkerSpherical.position = _sm.worldOffset + forceTargetSpherical.ToCartesian();
            radialCenterCartesian.position = _sm.worldOffset;
            radialCenterSpherical.position = _sm.worldOffset;
            
            var scaleSph = forceTargetSpherical.GetRadialDistance() * 2;
            radialCenterSpherical.localScale = new Vector3(scaleSph,scaleSph,scaleSph);
            var scaleCart = forceTargetCartesian.magnitude*2;
            radialCenterCartesian.localScale = new Vector3(scaleCart,scaleCart,scaleCart);
        }

        private void OnDrawGizmos()
        {
            Vector3 worldOffset;
            if (_sm == null)
                worldOffset = GetComponent<SphericalMotion>().worldOffset;
            else
                worldOffset = _sm.worldOffset;
            
            var cartesianForceTarget = worldOffset + forceTargetCartesian;
            Gizmos.DrawWireSphere(cartesianForceTarget, 0.5f);
            Gizmos.DrawLine(transform.position, cartesianForceTarget);
            Gizmos.DrawLine(worldOffset, cartesianForceTarget);
            
            Gizmos.color = Color.red;
            var sphericalForceTarget = worldOffset + forceTargetSpherical.ToCartesian();
            Gizmos.DrawWireSphere(sphericalForceTarget, 0.5f);
            Gizmos.DrawLine(transform.position, sphericalForceTarget);
            Gizmos.DrawLine(worldOffset, sphericalForceTarget);
            
            Gizmos.color = Color.white; 
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(worldOffset, 3f);
        }
    }
}