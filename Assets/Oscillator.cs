using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] float period = 2f;
    [SerializeField] Vector3 movementVector;

    Vector3 startPosition;
    Vector3 offset;

    [Range(0, 1)] [SerializeField] float movementFactor;


    // Use this for initialization
    void Start() {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {

        if(period <= Mathf.Epsilon) { return;}
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2f; //6.28
        float rawSinWave = Mathf.Sin(cycles * tau);
        movementFactor = rawSinWave / 2f + 0.5f;

        offset = movementVector * movementFactor;
        transform.position = startPosition + offset;
    }
}
