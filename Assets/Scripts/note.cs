using UnityEngine;

public class note : MonoBehaviour
{
    private Rigidbody2D _rb;

    public float _speed;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.linearVelocityY = _speed * -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
