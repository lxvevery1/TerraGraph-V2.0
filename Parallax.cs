using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxEffect;


    private float lenght;
    private float startPos;


    private void Awake()
    {
        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float distance = Input.mousePosition.x * parallaxEffect * 0.0015f;

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
