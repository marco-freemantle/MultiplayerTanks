using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject prefab1;
    [SerializeField] private GameObject prefab2;

    private void OnDestroy()
    {
        Instantiate(prefab1, transform.position, Quaternion.identity);
        Instantiate(prefab2, transform.position, Quaternion.identity);
    }
}
