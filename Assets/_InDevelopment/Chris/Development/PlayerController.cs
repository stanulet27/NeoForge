using SharedData;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SharedBool _focusIsPlaying;
    [SerializeField] private Collider2D _top;
    [SerializeField] private Collider2D _bottom;
    
    private float _speed = 0;
    private float rateOfChange = 2.5f;
    
    private bool _atTop;
    private bool _atBottom;
    
    void Update()
    {
        if (_focusIsPlaying.Value)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.Space) && !_atTop)
        {
            _speed +=rateOfChange * 1.0001f;
        }
        else if (!Input.GetKey(KeyCode.Space) && !_atBottom)
        {
            _speed -= rateOfChange - .9999f;
        }
        transform.position += new Vector3(0, _speed * Time.deltaTime, 0);   
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == _top)
        {
            _speed = 0;
            _atTop = true;
            transform.localPosition = new Vector3(0, 263, 0);
        }
        else if(other == _bottom)
        {
            transform.localPosition = new Vector3(0, -263, 0);
            _atBottom = true;
            _speed = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other == _top) _atTop = false;
        if(other == _bottom) _atBottom = false;
    }
}
