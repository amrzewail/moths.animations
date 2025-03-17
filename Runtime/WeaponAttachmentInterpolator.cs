using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

namespace Anima
{

    [ExecuteAlways]
    [DefaultExecutionOrder(200)]
    public class WeaponAttachmentInterpolator : MonoBehaviour
    {

        [SerializeField] Transform _rig;
        [SerializeField] Transform _weaponRoot;
        [SerializeField] Transform _scabbardRoot;

        [Space]
        [Header("Targets")]
        [SerializeField] Transform _sheathTransform;
        [SerializeField] Transform _gripRTransform;
        [SerializeField] Transform _gripLTransform;

        [Space]
        [SerializeField] Vector3 rotationOffset = new Vector3(90, 180, 0);

        private Transform _weapon;
        private Transform _free;
        private Transform _sheath;
        private Transform _gripR;
        private Transform _gripL;

        private float[] values = new float[4];

        private void Awake()
        {
            _weapon = _rig.Find("weapon");
            _free = _rig.Find("weapon.free");
            _sheath = _rig.Find("weapon.sheath");
            _gripR = _rig.Find("weapon.grip.R/weapon.grip.point.R");
            _gripL = _rig.Find("weapon.grip.L/weapon.grip.point.L");
        }


        private void Start()
        {
            if (!_weaponRoot) return;


        }

        private void LateUpdate()
        {
            if (!_weapon)
            {
                Awake();
                return;
            }

            values[0] = Vector3.Distance(_weapon.position, _free.position);
            values[1] = Vector3.Distance(_weapon.position, _sheath.position);
            values[2] = Vector3.Distance(_weapon.position, _gripR.position);
            values[3] = Vector3.Distance(_weapon.position, _gripL.position);

            float length = 0;
            for (int i = 0; i < values.Length; i++)
            {
                length += Mathf.Pow(values[i], 2);
            }
            length = Mathf.Sqrt(length);

            //for(int i = 0; i < values.Length; i++)
            //{
            //    values[i] = 1f - values[i] / length;
            //    if (values[i] >= 0.9f) values[i] = 1f;
            //    else values[i] = -1;
            //}

            float maxValue = 0;
            int maxIndex = -2;

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 1f - values[i] / length;
                if (values[i] < 0.8f) values[i] = -1;
                if (values[i] >= maxValue)
                {
                    maxValue = values[i];
                    maxIndex = i;
                }
            }

            maxValue = 1;

            Vector3 center = (_sheath.position + _gripR.position + _gripL.position) / 3f;
            Vector3 ownCenter = (_sheathTransform.position + _gripRTransform.position + _gripLTransform.position) / 3f;

            Vector3 position = _sheathTransform.position;
            Quaternion rotation = _sheathTransform.rotation;

            switch (maxIndex)
            {
                case 0:

                    Vector3 direction = _free.position - _gripR.position;
                    Transform ownRelative = _gripRTransform;
                    if (Vector3.Distance(_free.position, _sheath.position) < direction.magnitude)
                    {
                        direction = _free.position - _sheath.position;
                        ownRelative = _sheathTransform;
                    }
                    if (Vector3.Distance(_free.position, _gripL.position) < direction.magnitude)
                    {
                        direction = _free.position - _gripL.position;
                        ownRelative = _gripLTransform;
                    }

                    position = Vector3.Lerp(_weapon.position, ownRelative.position + direction, maxValue);
                    rotation = Quaternion.Slerp(_weapon.rotation, _free.rotation, maxValue);
                    break;
                case 1:
                    if (_sheathTransform)
                    {
                        position = Vector3.Lerp(_weapon.position, _sheathTransform.position, maxValue);
                        rotation = Quaternion.Slerp(_weapon.rotation, _sheathTransform.rotation, maxValue);
                    }
                    break;
                case 2:
                    if (_gripRTransform)
                    {
                        position = Vector3.Lerp(_weapon.position, _gripRTransform.position, maxValue);
                        rotation = Quaternion.Slerp(_weapon.rotation, _gripRTransform.rotation, maxValue);
                    }
                    break;
                case 3:
                    if (_gripLTransform)
                    {
                        position = Vector3.Lerp(_weapon.position, _gripLTransform.position, maxValue);
                        rotation = Quaternion.Slerp(_weapon.rotation, _gripLTransform.rotation, maxValue);
                    }
                    break;
            }

            Quaternion rotOffset = Quaternion.Euler(rotationOffset);

            _weaponRoot.position = position;
            _weaponRoot.rotation = rotation;
            _weaponRoot.localRotation *= rotOffset;

            _scabbardRoot.position = _sheathTransform.position;
            _scabbardRoot.rotation = _sheathTransform.rotation;
            _scabbardRoot.localRotation *= rotOffset;
        }
    }
}