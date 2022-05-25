using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    public void SpawnEffect(GameObject effect, Vector3 pos, float length) => StartCoroutine(WaitAndDelete(length, pos, effect));

    private IEnumerator WaitAndDelete(float time, Vector3 position, GameObject obj)
    {
        GameObject spawned = Instantiate(obj, position, Quaternion.identity, transform);
        yield return new WaitForSeconds(time);
        Destroy(spawned);
    }
}
