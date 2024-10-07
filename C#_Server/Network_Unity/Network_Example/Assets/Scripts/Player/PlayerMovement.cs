using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 target = mainCam.ScreenToWorldPoint(Input.mousePosition);

            StopAllCoroutines();
            StartCoroutine(MoveTo(target));
        }
    }

    private IEnumerator MoveTo(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);
            yield return null;
        }
    }
}
