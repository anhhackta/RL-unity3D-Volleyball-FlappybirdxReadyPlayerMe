using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBehavior : MonoBehaviour
{
    private const float DISTANCE = 2f;
    private const int TOTAL_PIPE = 3;

    private Vector3 _startPosition;
    [SerializeField]
    private float _variationFactor = 0.4f;

    private void Awake()
    {
        _startPosition = transform.localPosition;
        RandomnizeY();

    }

    private void LateUpdate()
    {
        Transform transformRef = transform;
        transformRef.Translate(Vector3.left * Time.deltaTime);
        if (transformRef.localPosition.x < -DISTANCE)
            transformRef.Translate(DISTANCE * TOTAL_PIPE * Vector3.right);
    }

    public void Initialize()
    {
        transform.localPosition = _startPosition;
        RandomnizeY();
    }

    private void RandomnizeY()
    {
        float posY = Random.Range(-_variationFactor, _variationFactor);
        transform.Translate(Vector3.up * posY);
    }
}
