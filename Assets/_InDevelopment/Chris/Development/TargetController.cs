using System.Collections;
using SharedData;
using UnityEngine;
using UnityEngine.UI;

public class TargetController : MonoBehaviour
{
    private const int TOP = 263;
    private const int BOTTOM = -263;

    [SerializeField] private Collider2D _playerBar;
    [SerializeField] private Scrollbar _progressBar;
    [SerializeField] private SharedBool _isPlaying;

    private bool _isScoring;
    private float _chargeSpeed = 0.25f;
    private float _chargeValue;

    private bool _isMoving;
    private float _speed;
    private float _targetY;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _isScoring = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isScoring = false;
    }
    
    void Update()
    {
        if (_isPlaying.Value)
        {

            if (!_isMoving)
            {
                _isMoving = true;
                StartCoroutine(HandleMovement());
            }
            HandleScoring();
            CheckWin();
        }
    }

    private void CheckWin()
    {
        if (!Mathf.Approximately(_progressBar.size, 1f)) return;
        Debug.Log("Win!");
        _isPlaying.Value = false;

    }
    private IEnumerator HandleMovement()
    {
        while (_isPlaying.Value)
        {
            _targetY = Random.Range(BOTTOM, TOP);
            _speed = Random.Range(100,500);
            yield return StartCoroutine(MoveToPosition());
            float time = Random.Range(50, 200) / 100f;
            yield return new WaitForSeconds(time);
        }
    }

    private IEnumerator MoveToPosition()
    {
        while (!Mathf.Approximately(transform.localPosition.y, _targetY) && _isPlaying.Value)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0, _targetY, 0), _speed * Time.deltaTime);
            yield return null;
        }
    }

    private void HandleScoring()
    {
        if(_isScoring)
            _chargeValue = Mathf.Min(1, _chargeSpeed * Time.deltaTime + _chargeValue);
        else
            _chargeValue = Mathf.Max(0, _chargeValue - _chargeSpeed * Time.deltaTime);
        _progressBar.size = _chargeValue;
    }

}
