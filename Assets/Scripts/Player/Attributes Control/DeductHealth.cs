using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeductHealth : MonoBehaviour {

   [Header("Deduct Setting")]
    public float interval = 2f;
    public int deductAmount = 10;
    private Coroutine _dudectCoroutine;
    
    void Start()
    {

        if (PlayerAttributes.Instance != null) {

            _dudectCoroutine = StartCoroutine(DeductManaPeriodically());

        } else {

            Debug.Log("Can't find player attributes!");

        }

    }

    System.Collections.IEnumerator DeductManaPeriodically() {

        WaitForSeconds wait = new WaitForSeconds(interval);

        while (true) {

            yield return wait;

            PlayerAttributes.Instance.TakeDamage(deductAmount);

        }

    }

     void OnDisable() {
        
        if (_dudectCoroutine != null) {

            StopCoroutine(_dudectCoroutine);

        }

    }

}
