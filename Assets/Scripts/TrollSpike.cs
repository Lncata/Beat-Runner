using UnityEngine;
using System.Collections;

public class TrollSpike : MonoBehaviour
{
    public Transform player;

    public float triggerDistance = 3f;
    public float moveDistance = 2f;
    public float moveSpeed = 12f;

    public bool moveLeft = true;
    public bool returnToStart = false;
    public float waitBeforeReturn = 0.2f;

    private bool activated = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;

        float dir = moveLeft ? -1f : 1f;
        targetPosition = startPosition + new Vector3(moveDistance * dir, 0f, 0f);
    }

    void Update()
    {
        if (activated || player == null) return;

        float distanceX = player.position.x - transform.position.x;

        if (moveLeft)
        {
            if (distanceX >= -triggerDistance && distanceX <= 0.5f)
            {
                StartCoroutine(MoveSpike());
            }
        }
        else
        {
            if (distanceX >= -triggerDistance && distanceX <= 0.5f)
            {
                StartCoroutine(MoveSpike());
            }
        }
    }

    IEnumerator MoveSpike()
    {
        activated = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPosition;

        if (returnToStart)
        {
            yield return new WaitForSeconds(waitBeforeReturn);

            while (Vector3.Distance(transform.position, startPosition) > 0.02f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    startPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = startPosition;
        }
    }
}