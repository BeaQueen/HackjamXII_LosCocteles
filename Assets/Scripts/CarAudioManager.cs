using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class CarAudioProfile
    {
        public string carName;

        [Header("Car")]
        public GameObject carRoot;

        [Header("Unique Sounds")]
        public AudioClip startupClip;
        public AudioClip engineLoopClip;
        public AudioClip accelerationClip;
    }

    // =========================================================
    // CARS
    // =========================================================

    [Header("Cars")]
    [SerializeField] private CarAudioProfile[] cars;


    // =========================================================
    // GENERIC SOUNDS
    // =========================================================

    [Header("Generic Sounds")]
    [SerializeField] private AudioClip brakeClip;
    [SerializeField] private AudioClip skidClip;
    [SerializeField] private AudioClip crashClip;


    // =========================================================
    // INPUT
    // =========================================================

    [Header("Wheel Input")]
    [SerializeField] private InputActionReference brakeAction;
    [SerializeField] private InputActionReference steeringAction;


    // =========================================================
    // ENGINE
    // =========================================================

    [Header("Engine RPM")]

    [SerializeField] private float maxRevolutions = 10f;

    [SerializeField] private float idlePitch = 0.75f;

    [SerializeField] private float maxEnginePitch = 1.6f;

    [SerializeField] private float idleVolume = 0.35f;

    [SerializeField] private float maxEngineVolume = 0.85f;

    [SerializeField] private float engineSmoothness = 5f;


    // =========================================================
    // ACCELERATION
    // =========================================================

    [Header("Acceleration")]

    [SerializeField] private float accelerationVolume = 0.7f;


    // =========================================================
    // BRAKE
    // =========================================================

    [Header("Brake")]

    [SerializeField] private float brakeThreshold = 0.2f;

    [SerializeField] private float brakeVolume = 0.8f;


    // =========================================================
    // SKID
    // =========================================================

    [Header("Skid")]

    [SerializeField] private float skidSteeringThreshold = 0.3f;

    [SerializeField] private float minimumSkidRevolutions = 1f;

    [SerializeField] private float skidVolume = 0.8f;


    // =========================================================
    // CRASH
    // =========================================================

    [Header("Crash")]

    [SerializeField] private float crashSpeedDropThreshold = 7f;

    [SerializeField] private float crashCooldown = 0.5f;


    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    private AudioSource startupSource;
    private AudioSource engineSource;
    private AudioSource accelerationSource;
    private AudioSource brakeSource;
    private AudioSource skidSource;
    private AudioSource crashSource;


    // =========================================================
    // ACTIVE CAR
    // =========================================================

    private CarAudioProfile activeProfile;
    private Transform activeCar;
    private LogicaConductor conductor;


    // =========================================================
    // INTERNAL STATE
    // =========================================================

    private Vector3 previousPosition;
    private float previousSpeed;

    private float lastCrashTime = -100f;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        CreateAudioSources();
    }


    private void Start()
    {
        FindActiveCar();

        if (activeCar == null)
        {
            Debug.LogError(
                "CarAudioManager: No se ha encontrado ningún coche activo."
            );

            return;
        }

        FindConductor();

        if (conductor == null)
            return;

        previousPosition = activeCar.position;
        previousSpeed = 0f;

        StartCoroutine(StartCarAudio());
    }


    private void Update()
    {
        if (activeCar == null || conductor == null)
            return;

        UpdateEngine();
        UpdateAcceleration();
        UpdateBrake();
        UpdateSkid();
        UpdateCrashDetection();

        previousPosition = activeCar.position;
    }


    // =========================================================
    // FIND CAR
    // =========================================================

    private void FindActiveCar()
    {
        foreach (CarAudioProfile profile in cars)
        {
            if (
                profile.carRoot != null &&
                profile.carRoot.activeInHierarchy
            )
            {
                activeProfile = profile;
                activeCar = profile.carRoot.transform;

                Debug.Log(
                    "CarAudioManager: coche activo = "
                    + profile.carName
                );

                return;
            }
        }
    }


    private void FindConductor()
    {
        conductor = activeCar.GetComponent<LogicaConductor>();

        if (conductor == null)
        {
            conductor =
                activeCar.GetComponentInChildren<LogicaConductor>();
        }

        if (conductor == null)
        {
            conductor =
                activeCar.GetComponentInParent<LogicaConductor>();
        }

        if (conductor == null)
        {
            Debug.LogError(
                "CarAudioManager: No se ha encontrado LogicaConductor."
            );
        }
    }


    // =========================================================
    // STARTUP
    // =========================================================

    private IEnumerator StartCarAudio()
    {
        // ARRANQUE: NO LOOP
        if (
            activeProfile != null &&
            activeProfile.startupClip != null
        )
        {
            startupSource.clip =
                activeProfile.startupClip;

            startupSource.loop = false;
            startupSource.Play();

            yield return new WaitForSeconds(
                activeProfile.startupClip.length
            );
        }

        StartEngineLoop();
    }


    // =========================================================
    // ENGINE
    // =========================================================

    private void StartEngineLoop()
    {
        if (
            activeProfile == null ||
            activeProfile.engineLoopClip == null
        )
            return;

        engineSource.clip =
            activeProfile.engineLoopClip;

        // MOTOR: LOOP
        engineSource.loop = true;

        engineSource.pitch = idlePitch;
        engineSource.volume = idleVolume;

        engineSource.Play();
    }


    private void UpdateEngine()
    {
        if (!engineSource.isPlaying)
            return;

        float normalizedRPM =
            Mathf.Clamp01(
                conductor.revoluciones /
                maxRevolutions
            );

        float targetPitch =
            Mathf.Lerp(
                idlePitch,
                maxEnginePitch,
                normalizedRPM
            );

        float targetVolume =
            Mathf.Lerp(
                idleVolume,
                maxEngineVolume,
                normalizedRPM
            );

        engineSource.pitch =
            Mathf.Lerp(
                engineSource.pitch,
                targetPitch,
                Time.deltaTime * engineSmoothness
            );

        engineSource.volume =
            Mathf.Lerp(
                engineSource.volume,
                targetVolume,
                Time.deltaTime * engineSmoothness
            );
    }


    // =========================================================
    // ACCELERATION
    // =========================================================

    private void UpdateAcceleration()
    {
        bool accelerating =
            conductor.acelerando;

        if (accelerating)
        {
            StartAcceleration();
        }
        else
        {
            StopAcceleration();
        }
    }


    private void StartAcceleration()
    {
        if (
            activeProfile == null ||
            activeProfile.accelerationClip == null
        )
            return;

        if (accelerationSource.isPlaying)
            return;

        accelerationSource.clip =
            activeProfile.accelerationClip;

        // ACELERACIÓN: LOOP
        accelerationSource.loop = true;
        accelerationSource.volume = accelerationVolume;

        accelerationSource.Play();
    }


    private void StopAcceleration()
    {
        if (accelerationSource.isPlaying)
        {
            accelerationSource.Stop();
        }
    }


    // =========================================================
    // BRAKE
    // =========================================================

    private void UpdateBrake()
    {
        if (brakeAction == null)
            return;

        float rawBrake =
            brakeAction.action.ReadValue<float>();

        /*
         * En vuestro volante:
         *
         * 1 = pedal suelto
         * distinto de 1 = pedal pulsado
         */

        float brakeAmount =
            Mathf.Clamp01(1f - rawBrake);

        bool braking =
            brakeAmount > brakeThreshold;

        if (braking)
        {
            StartBrake();
        }
        else
        {
            StopBrake();
        }
    }


    private void StartBrake()
    {
        if (brakeClip == null)
            return;

        if (brakeSource.isPlaying)
            return;

        brakeSource.clip = brakeClip;

        // FRENO: LOOP
        brakeSource.loop = true;
        brakeSource.volume = brakeVolume;

        brakeSource.Play();
    }


    private void StopBrake()
    {
        if (brakeSource.isPlaying)
        {
            brakeSource.Stop();
        }
    }


    // =========================================================
    // SKID
    // =========================================================

    private void UpdateSkid()
    {
        if (steeringAction == null)
            return;

        Vector2 steering =
            steeringAction.action.ReadValue<Vector2>();

        float horizontalSteering =
            Mathf.Abs(steering.x);

        bool enoughSteering =
            horizontalSteering >
            skidSteeringThreshold;

        bool enoughSpeed =
            conductor.revoluciones >
            minimumSkidRevolutions;

        bool shouldSkid =
            enoughSteering &&
            enoughSpeed;

        if (shouldSkid)
        {
            StartSkid();
        }
        else
        {
            StopSkid();
        }
    }


    private void StartSkid()
    {
        if (skidClip == null)
            return;

        if (skidSource.isPlaying)
            return;

        skidSource.clip = skidClip;

        // DERRAPE: LOOP
        skidSource.loop = true;
        skidSource.volume = skidVolume;

        skidSource.Play();
    }


    private void StopSkid()
    {
        if (skidSource.isPlaying)
        {
            skidSource.Stop();
        }
    }


    // =========================================================
    // CRASH
    // =========================================================

    private void UpdateCrashDetection()
    {
        if (Time.deltaTime <= 0f)
            return;

        Vector3 movement =
            activeCar.position -
            previousPosition;

        float currentSpeed =
            movement.magnitude /
            Time.deltaTime;

        float speedDrop =
            previousSpeed -
            currentSpeed;

        bool bigSpeedDrop =
            speedDrop >
            crashSpeedDropThreshold;

        bool cooldownFinished =
            Time.time - lastCrashTime >
            crashCooldown;

        if (
            bigSpeedDrop &&
            cooldownFinished
        )
        {
            PlayCrash();

            lastCrashTime = Time.time;
        }

        previousSpeed = currentSpeed;
    }


    private void PlayCrash()
    {
        if (crashClip == null)
            return;

        // CHOQUE: ONE SHOT
        crashSource.loop = false;

        crashSource.PlayOneShot(
            crashClip
        );
    }


    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    private void CreateAudioSources()
    {
        startupSource =
            CreateAudioSource();

        engineSource =
            CreateAudioSource();

        accelerationSource =
            CreateAudioSource();

        brakeSource =
            CreateAudioSource();

        skidSource =
            CreateAudioSource();

        crashSource =
            CreateAudioSource();
    }


    private AudioSource CreateAudioSource()
    {
        AudioSource source =
            gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;

        // Sonido 2D porque es el coche del jugador
        source.spatialBlend = 0f;

        return source;
    }
}