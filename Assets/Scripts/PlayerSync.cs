using UnityEngine;

public class PlayerSync : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource music;
    public float speed = 6f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (music.isPlaying)
        {
            float syncX = music.time * speed;
            transform.position = new Vector2(syncX, transform.position.y);
        }
    }
}
