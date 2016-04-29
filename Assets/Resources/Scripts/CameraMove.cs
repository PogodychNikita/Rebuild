using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{
    private float _cameraDistanceMax = 1000f;
    private float _cameraDistanceMin = 200f;
    private float _cameraDistance = 600f;
    private float _scrollSpeed = 500f;
    private float _mouseSensitivity = 2000f;

    public bool EnterUi;

    void Update()
    {
        if (!EnterUi)
        {
            if (Input.GetMouseButton(2))
                transform.position += new Vector3(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * _mouseSensitivity, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _mouseSensitivity, 0);

            _cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
            _cameraDistance = Mathf.Clamp(_cameraDistance, _cameraDistanceMin, _cameraDistanceMax);

            GetComponent<Camera>().orthographicSize = _cameraDistance;
        }
    }
}