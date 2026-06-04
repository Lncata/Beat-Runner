using UnityEngine;

public class MovingSpikeTrap : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("1 = derecha, -1 = izquierda")]
    public int direction = -1;

    public float shootDistance = 4f;
    public float shootSpeed = 12f;
    public float returnSpeed = 6f;

    [Header("Tiempos")]
    public float waitBeforeShoot = 0.4f;
    public float waitBeforeReturn = 0.25f;
    public float cooldown = 1.5f;

    [Header("Detección del Player")]
    public bool shootOnlyWhenPlayerNear = true;
    public float detectionDistance = 7f;
    public bool detectOnlyInFront = true;

    private Transform player;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float timer;
    private State currentState;

    private enum State
    {
        Waiting,
        Preparing,
        Shooting,
        StayingOut,
        Returning,
        Cooldown
    }

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.right * direction * shootDistance;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        currentState = State.Waiting;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Waiting:
                if (!shootOnlyWhenPlayerNear || PlayerDetected())
                {
                    timer = waitBeforeShoot;
                    currentState = State.Preparing;
                }
                break;

            case State.Preparing:
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    currentState = State.Shooting;
                }
                break;

            case State.Shooting:
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    shootSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    timer = waitBeforeReturn;
                    currentState = State.StayingOut;
                }
                break;

            case State.StayingOut:
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    currentState = State.Returning;
                }
                break;

            case State.Returning:
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    startPosition,
                    returnSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, startPosition) < 0.01f)
                {
                    timer = cooldown;
                    currentState = State.Cooldown;
                }
                break;

            case State.Cooldown:
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    currentState = State.Waiting;
                }
                break;
        }
    }

    private bool PlayerDetected()
    {
        if (player == null) return false;

        float distanceX = player.position.x - startPosition.x;

        if (Mathf.Abs(distanceX) > detectionDistance)
        {
            return false;
        }

        if (detectOnlyInFront)
        {
            if (direction == 1 && distanceX < 0)
            {
                return false;
            }

            if (direction == -1 && distanceX > 0)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 end = start + Vector3.right * direction * shootDistance;

        Gizmos.DrawLine(start, end);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, start + Vector3.right * direction * detectionDistance);
    }
}