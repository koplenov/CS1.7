using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalTimer : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}