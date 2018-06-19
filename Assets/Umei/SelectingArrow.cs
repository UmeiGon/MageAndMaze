using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingArrow : MonoBehaviour {
    Coroutine currentCoroutine;
    [SerializeField] float arrowMoveSpeed = 1.0f;
    [SerializeField] float arrowMoveLimit = 40.0f;
    private void OnEnable()
    {
        currentCoroutine=StartCoroutine(SelectingRoutine());
    }
    private void OnDisable()
    {
        StopCoroutine(currentCoroutine);
    }
    IEnumerator SelectingRoutine()
    {
        int moveDir=-1;
        float allMoveValue = 0;
        while (true)
        {
            float moveValue=arrowMoveSpeed*Time.deltaTime;
            transform.Translate(0, moveValue*moveDir, 0);
            allMoveValue += moveValue;
            if (allMoveValue>arrowMoveLimit)
            {
                allMoveValue = 0;
                moveDir *= -1;
            }
            yield return null;
        }
    }
}
