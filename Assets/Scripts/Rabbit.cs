using UnityEngine;
using System.Collections;
using UnityEngineInternal;

public class Rabbit : MonoBehaviour
{


    private Animator _animator;

    [HideInInspector]
    public float ClipNormalizedTime;

    [HideInInspector]
    public bool IsIdle;

    public void Start()
	{
	    _animator = GetComponent<Animator>();
        IsIdle = _animator.GetBool("IsIdle");
	}
	
	public void Update()
	{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetIdle(true);
        }

        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetIdle(false);
        }

	    var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        ClipNormalizedTime = Mathf.Repeat(stateInfo.normalizedTime, 1.0f);

        //Debug.Log("NormTimme: " + ClipNormalizedTime);
	}

    public void SetIdle(bool newIsIdle)
    {
        if (newIsIdle == IsIdle)
            return;

        if (!IsIdle)
            StartCoroutine(ChangeState(newIsIdle));

        else
        {
            IsIdle = newIsIdle;
            _animator.SetBool("IsIdle", IsIdle);
        }
    }

    private IEnumerator ChangeState(bool newIsIdle)
    {
        while (ClipNormalizedTime <= 0.95f)
        {
            yield return null;
        }

        IsIdle = newIsIdle;
        _animator.SetBool("IsIdle", IsIdle);
    }

}
