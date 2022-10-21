using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] Rigidbody[] nonPlayerRbs;
    [SerializeField] Slider forceSlider;
    [SerializeField] float forceMultiplier;
    [SerializeField] InterfaceManager interfaceManager;
    [SerializeField] GameObject yellowBall;
    [SerializeField] GameObject redBall;
    [SerializeField] AudioManager audioManager;
    bool charging;
    bool increasing;
    bool yellowBallHit;
    bool redBallHit;
    Vector3[] startPositions;
    Vector3[] lastMovePositions;
    Vector3 lastShotForce;


    public enum GameState
    {
        asleep,
        awaiting,
        moving
    }

    [SerializeField] GameState gameState;

    private void Start()
    {
        gameState = GameState.asleep;
        startPositions = new Vector3[3];
        lastMovePositions = new Vector3[3];
        startPositions[0] = transform.position;
        startPositions[1] = yellowBall.transform.position;
        startPositions[2] = redBall.transform.position;

        interfaceManager = FindObjectOfType<InterfaceManager>();
    }
    private void Update()
    {
        if (gameState == GameState.awaiting)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                charging = true;
                if (increasing)
                {
                    forceSlider.value += Time.deltaTime;
                    if (forceSlider.value >= 1f)
                    {
                        increasing = false;
                        forceSlider.value = 1f;
                    }
                }
                else
                {
                    forceSlider.value -= Time.deltaTime;
                    if (forceSlider.value <= 0f)
                    {
                        increasing = true;
                        forceSlider.value = 0f;
                    }
                }
            }
            else if (charging && !Input.GetKey(KeyCode.Space))
            {
                charging = false;
                redBallHit = false;
                yellowBallHit = false;
                StartCoroutine(Shoot(cam.transform.forward.normalized * forceSlider.value * forceMultiplier));
                interfaceManager.ShotBall();
            }
            else if (forceSlider.value != 0f)
            {
                increasing = true;
                forceSlider.value = 0f;
            }
        }
        else if (gameState == GameState.moving && rigidBody.velocity == Vector3.zero && nonPlayerRbs[0].velocity == Vector3.zero && nonPlayerRbs[1].velocity == Vector3.zero)
        {
            if (interfaceManager.points >= 3)
            {
                gameState = GameState.asleep;
                interfaceManager.Victory();
            }
            else
            {
                gameState = GameState.awaiting;
                interfaceManager.BallStopped();
                forceSlider.gameObject.SetActive(true);
            }
        }
        
    }

    public void ResetField()
    {
        transform.position = startPositions[0];
        yellowBall.transform.position = startPositions[1];
        redBall.transform.position = startPositions[2];
        gameState = GameState.awaiting;
    }

    IEnumerator Shoot(Vector3 force)
    {
        lastMovePositions[0] = this.transform.position;
        lastMovePositions[1] = yellowBall.transform.position;
        lastMovePositions[2] = redBall.transform.position;
        lastShotForce = force;
        forceSlider.gameObject.SetActive(false);
        rigidBody.AddForce(force);
        audioManager.PlayBallShot();
        yield return new WaitUntil(() => rigidBody.velocity != Vector3.zero);
        gameState = GameState.moving;
    }

    public void ReplayLastShot()
    {
        this.transform.position = lastMovePositions[0];
        yellowBall.transform.position = lastMovePositions[1];
        redBall.transform.position = lastMovePositions[2];
        StartCoroutine(Shoot(lastShotForce));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == redBall && !redBallHit)
        {
            redBallHit = true;
            if (yellowBallHit)
            {
                interfaceManager.Scored();
            }
        }
        else if (collision.gameObject == yellowBall && !yellowBallHit)
        {
            yellowBallHit = true;
            if (redBallHit)
            {
                interfaceManager.Scored();
            }
        }
    }
}
