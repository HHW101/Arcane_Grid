using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardArrow : MonoBehaviour
{
    public void Init()
    {

    }
    public void End()
    {
        StartCoroutine(EndCoroutine());
    }
    IEnumerator EndCoroutine()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
