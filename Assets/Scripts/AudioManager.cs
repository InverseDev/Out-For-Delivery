using UnityEngine;
using System.Collections;

public class CarSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource engineIdleSource;
    [SerializeField] private AudioSource engineAcceleratingSource;
    [SerializeField] private AudioSource brakingSource;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float pitchModifier = 1f;
    [SerializeField] private float idleStopDelay = 1f;
    private CarController carController;
    private float targetPitch;
    private Coroutine idleCoroutine;
    private bool brakeSoundPlayed = false;

    private void Start()
    {
        carController = GetComponent<CarController>();
    }

    private void Update()
    {
        float currentSpeed = carController.GetCurrentSpeed();

        if (Mathf.Abs(currentSpeed) < 1f)
        {
            if (!engineIdleSource.isPlaying)
            {
                engineIdleSource.Play();
            }
        }
        else
        {
            if (idleCoroutine != null)
            {
                StopCoroutine(idleCoroutine);
            }
            idleCoroutine = StartCoroutine(StopIdleSoundWithDelay());
            if (!engineAcceleratingSource.isPlaying)
            {
                engineAcceleratingSource.Play();
            }
            targetPitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentSpeed) / carController.MaxSpeed);
            engineAcceleratingSource.pitch = Mathf.Lerp(engineAcceleratingSource.pitch, targetPitch, Time.deltaTime * pitchModifier);
        }

        if (carController.IsBraking())
        {
            if (!brakeSoundPlayed)
            {
                brakingSource.Play();
                brakeSoundPlayed = true;
            }
        }
        else
        {
            brakingSource.Stop();
            brakeSoundPlayed = false;
        }

    }

    private IEnumerator StopIdleSoundWithDelay()
    {
        yield return new WaitForSeconds(idleStopDelay);
       
        if (Mathf.Abs(carController.GetCurrentSpeed()) < 1f && engineIdleSource.isPlaying)
        {
            engineIdleSource.Stop();
        }
    }
}