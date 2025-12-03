using System;
using UnityEngine;

public class SideScroll : MonoBehaviour
{
    private Transform _player;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        var cameraPosition = transform.position;
        cameraPosition.x = MathF.Max(cameraPosition.x, _player.position.x);

        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        var cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }
}