using UnityEngine;

namespace Source.MobGenerator
{
    public class MobGenerator : MonoBehaviour
    {
        public MobData.MobType[] genList;
        [Header("생성 위치(상대적)")]
        public Vector2[] genPos;

        public virtual void Trigger()
        {
            for (int i = 0; i < genList.Length; i++)
            {
                MobData.instance.GetMob(genList[i],(Vector2)transform.position + genPos[i], Quaternion.identity); 
            }
        }
    }
}
