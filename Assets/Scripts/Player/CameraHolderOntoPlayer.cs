using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHolderOntoPlayer : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
