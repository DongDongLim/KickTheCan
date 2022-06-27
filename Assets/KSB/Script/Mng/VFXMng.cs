using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSB
{
    public class VFXMng : SingletonMini<VFXMng>
    {
        [SerializeField]
        private GameObject explosionVFX;

        [SerializeField]
        private GameObject dustVFX;

        protected override void OnAwake()
        {

        }

        public void OnDust(Transform transform)
        {
            Instantiate(dustVFX,new Vector3(transform.position.x,0f,transform.position.z),Quaternion.identity);
        }

        public void OnExplosion(Transform transform)
        {
            Instantiate(dustVFX, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity);
        }
    }


}
