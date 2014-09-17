using System.Runtime.Remoting.Channels;
using UnityEngine;
using System.Collections;
using UnityEngineInternal;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Small,
        Normal,
        Big
    }

    public float SunlightFadeSpeed;

    public float SmallSize;
    public float BigSize;
    public float ScaleSpeed;
    public float SmallEyeSize;
    public float NormalEyeSize;

    public Transform MushroomPlayerStart;
    public Transform ParkPlayerStart;
    public Transform MushroomForestPortal;

    public GameObject TheEnd;

    private PlayerState _currentState;

    public PlayerState CurrentState
    {
        get { return _currentState; }
        set { ChangeState(value); }
    }

    private bool _changingState;
    private float _desiredSize;

    private GameManager _manager;
    private TerrainCollider _terrainCollider;
    private OVRPlayerController _ovrPlayerController;
    private OVRCameraController _ovrCameraController;
    private Light _sunLight;
    private float _startIntensity;
    private AudioManager _audioManager;

    public void Start()
    {
        CurrentState = PlayerState.Normal;
        _changingState = false;

        _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        _terrainCollider = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainCollider>();
        _sunLight = GameObject.FindGameObjectWithTag("SunLight").GetComponent<Light>();
        _startIntensity = _sunLight.intensity;
        _ovrPlayerController = GetComponent<OVRPlayerController>();
        _ovrCameraController = GetComponentInChildren<OVRCameraController>();

        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
	
	public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            CurrentState = PlayerState.Big;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CurrentState = PlayerState.Small;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            CurrentState = PlayerState.Normal;
	}

    private void ChangeState(PlayerState newState)
    {
        if (_changingState || newState == _currentState)
            return;

        _changingState = true;

        Debug.Log("Changing to " + newState);

        switch (newState)
        {
            case PlayerState.Small:
                _desiredSize = SmallSize;
                StartCoroutine("Shrink");
                break;

            case PlayerState.Normal:
                switch (_currentState)
                {
                    case PlayerState.Small:
                        _desiredSize = 1.0f;
                        StartCoroutine("Grow");
                        break;
                    case PlayerState.Big:
                        _desiredSize = 1.0f;
                        StartCoroutine("Shrink");
                        break;
                }
                break;

            case PlayerState.Big:
                _desiredSize = BigSize;
                StartCoroutine("Grow");
                break;
        }

        _currentState = newState;
    }

    private IEnumerator Shrink()
    {
        var scale = transform.localScale;
        while (scale.x > _desiredSize)
        {
            scale.x -= ScaleSpeed * Time.deltaTime;
            scale.y -= ScaleSpeed * Time.deltaTime;
            scale.z -= ScaleSpeed * Time.deltaTime;

            if (_desiredSize < 1.0f)
            {
                if (_ovrCameraController != null && _ovrCameraController.EyeCenterPosition.y > SmallEyeSize)
                    _ovrCameraController.EyeCenterPosition.y -= ScaleSpeed * Time.deltaTime;
            }

            transform.localScale = scale;

            yield return null;
        }
        _changingState = false;
    }
    private IEnumerator Grow()
    {
        var scale = transform.localScale;
        while (scale.x < _desiredSize)
        {
            scale.x += ScaleSpeed * Time.deltaTime;
            scale.y += ScaleSpeed * Time.deltaTime;
            scale.z += ScaleSpeed * Time.deltaTime;

            if (_ovrCameraController != null && _ovrCameraController.EyeCenterPosition.y < NormalEyeSize)
                _ovrCameraController.EyeCenterPosition.y += ScaleSpeed * Time.deltaTime;

            transform.localScale = scale;

            yield return null;
        }
        _changingState = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("RabbitHolePortal"))
        {
            Debug.Log("Transporting!");

            StartCoroutine("DisablePark");
            _terrainCollider.enabled = false;
            _ovrPlayerController.GravityModifier = 0.1f;
            _audioManager.StartPlaying(AudioManager.AudioTrack.RabbitHole);
        }

        else if (other.tag.Equals("MushroomForestPortal"))
        {
            _manager.EnableMushroomForest();
            _audioManager.StartPlaying(AudioManager.AudioTrack.MushroomForest);

            transform.position = MushroomPlayerStart.position;
            transform.rotation = MushroomPlayerStart.rotation;

            transform.LookAt(MushroomForestPortal);
        }

        else if (other.tag.Equals("ParkPortal"))
        {
            _manager.EnablePark();
            _sunLight.intensity = _startIntensity;
            _terrainCollider.enabled = true;

            _audioManager.StartPlaying(AudioManager.AudioTrack.ParkBig);

            transform.position = ParkPlayerStart.position;
            transform.rotation = ParkPlayerStart.rotation;

            ChangeState(PlayerState.Big);

            TheEnd.SetActive(true);
        }

        else if (other.tag.Equals("Rabbit"))
        {
            collider.GetComponent<RabbitController>().IsIdle = false;
        }
    }

    private IEnumerator DisablePark()
    {
        yield return new WaitForSeconds(1.0f);

        while (_sunLight.intensity > 0.0f)
        {

            _sunLight.intensity -= SunlightFadeSpeed * Time.deltaTime;

            yield return null;
        }

        _manager.DisablePark();
    }
}
