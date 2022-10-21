using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionListener : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] AudioManager audioManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ball")
        {
            float volumeMultiplier = collision.rigidbody.velocity.magnitude / 2;
            StartCoroutine(audioManager.PlayBallCollision(volumeMultiplier));
        }
    }

}
