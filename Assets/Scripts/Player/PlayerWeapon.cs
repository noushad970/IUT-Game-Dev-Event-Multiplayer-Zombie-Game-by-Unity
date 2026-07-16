using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerWeapon : MonoBehaviour
{
    public enum FireMode
    {
        Single = 0,
        Auto = 1
    }

    [Header("Weapon")]
    public FireMode fireMode = FireMode.Auto;
    public int singleDamage = 40;
    public int autoDamage = 15;
    public float singleFireRate = 0.4f;
    public float autoFireRate = 0.1f;
    public Transform firePoint;
    public Transform shootDirection;
    public float fireDistance = 100f;
    public LayerMask hitLayer;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject bulletTracerPrefab;
    public float tracerSpeed = 80f;
    public Animator anim;

    [Header("UI Buttons")]
    private Button fireButton;
    private Button autoModeButton;
    private Button singleModeButton;

    private bool isHoldingFire = false;
    private bool canFire = true;
    private Coroutine autoFireRoutine;

    public GameObject rifle, pistol;

    private void Start()
    {
        FindFireButton();
        FindModeButtons();        // ← New
        UpdateModeButtons();      // ← New: Set initial button states
    }

    private void Update()
    {
        // ==========================
        // PC Mouse Input
        // ==========================
        if (Mouse.current == null)
            return;

        // Left Mouse Button Pressed
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartFire();
        }

        // Left Mouse Button Released
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            StopFire();
        }
    }

    private void LateUpdate()
    {
        if (fireMode == FireMode.Auto)
        {
            rifle.SetActive(true);
            pistol.SetActive(false);
        }
        else
        {
            rifle.SetActive(false);
            pistol.SetActive(true);
        }
    }

    // =============================================
    // FIND UI REFERENCES
    // =============================================
    private void FindFireButton()
    {
        FireRef fireRef = FindFirstObjectByType<FireRef>();
        if (fireRef == null)
        {
            Debug.LogWarning("FireRef not found.");
            return;
        }

        fireButton = fireRef.GetComponent<Button>();
        EventTrigger trigger = fireButton.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = fireButton.gameObject.AddComponent<EventTrigger>();

        AddEvent(trigger, EventTriggerType.PointerDown, PointerDown);
        AddEvent(trigger, EventTriggerType.PointerUp, PointerUp);
        Debug.Log("Fire Button Initialized.");
    }

    private void FindModeButtons()
    {
        // Find Auto Mode Button
        AutoRef autoRef = FindFirstObjectByType<AutoRef>();
        if (autoRef != null)
        {
            autoModeButton = autoRef.GetComponent<Button>();
            if (autoModeButton != null)
                autoModeButton.onClick.AddListener(() => SetFireMode(FireMode.Auto));
        }
        else
        {
            Debug.LogWarning("AutoRef not found.");
        }

        // Find Single Mode Button
        SingleRef singleRef = FindFirstObjectByType<SingleRef>();
        if (singleRef != null)
        {
            singleModeButton = singleRef.GetComponent<Button>();
            if (singleModeButton != null)
                singleModeButton.onClick.AddListener(() => SetFireMode(FireMode.Single));
        }
        else
        {
            Debug.LogWarning("SingleRef not found.");
        }
    }

    private void UpdateModeButtons()
    {
        if (fireMode == FireMode.Auto)
        {
            if (autoModeButton != null) autoModeButton.gameObject.SetActive(false);
            if (singleModeButton != null) singleModeButton.gameObject.SetActive(true);
        }
        else // Single
        {
            if (autoModeButton != null) autoModeButton.gameObject.SetActive(true);
            if (singleModeButton != null) singleModeButton.gameObject.SetActive(false);
        }
    }

    void AddEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    //=========================================
    // UI EVENTS
    //=========================================
    void PointerDown(BaseEventData data)
    {
        StartFire();
    }

    void PointerUp(BaseEventData data)
    {
        StopFire();
    }

    //=========================================
    // START / STOP FIRE
    //=========================================
    void StartFire()
    {
        if (fireMode == FireMode.Single)
        {
            FireSingle();
        }
        else
        {
            if (isHoldingFire) return;
            isHoldingFire = true;
            autoFireRoutine = StartCoroutine(AutoFire());
        }
    }

    void StopFire()
    {
        isHoldingFire = false;
        if (autoFireRoutine != null)
        {
            StopCoroutine(autoFireRoutine);
            autoFireRoutine = null;
        }
    }

    //=========================================
    // AUTO FIRE
    //=========================================
    IEnumerator AutoFire()
    {
        while (isHoldingFire)
        {
            Fire(autoDamage);
            yield return new WaitForSeconds(autoFireRate);
        }
        autoFireRoutine = null;
    }

    //=========================================
    // SINGLE FIRE
    //=========================================
    async void FireSingle()
    {
        if (!canFire) return;
        canFire = false;
        Fire(singleDamage);
        await System.Threading.Tasks.Task.Delay((int)(singleFireRate * 1000));
        canFire = true;
    }

    //=========================================
    // FIRE
    //=========================================
    void Fire(int damage)
    {
        Debug.Log("Fire : Damage = " + damage);

        //-----------------------------
        // Muzzle Flash
        //-----------------------------
        if (muzzleFlash != null)
        {
            Quaternion flashRotation = firePoint.rotation * Quaternion.Euler(0f, -90f, 0f);
            ParticleSystem flash = Instantiate(muzzleFlash, firePoint.position, flashRotation);
            flash.gameObject.SetActive(true);
            flash.Play();
            Destroy(flash.gameObject, flash.main.duration + flash.main.startLifetime.constantMax);
        }

        //-----------------------------
        // Shoot Animation
        //-----------------------------
        if (anim != null)
            anim.Play("SingleShot");

        //-----------------------------
        // Raycast
        //-----------------------------
        Vector3 direction = shootDirection.forward;
        Ray ray = new Ray(firePoint.position, direction);
        Vector3 hitPoint = firePoint.position + direction * fireDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, fireDistance, hitLayer))
        {
            Debug.Log("Hit : " + hit.collider.name);
            hitPoint = hit.point;

            EnemyHealth zombie = hit.collider.GetComponent<EnemyHealth>();
            if (zombie == null)
                zombie = hit.collider.GetComponentInParent<EnemyHealth>();

            if (zombie != null)
            {
                zombie.TakeDamage(damage, gameObject);
            }
        }

        //-----------------------------
        // Bullet Tracer
        //-----------------------------
        if (bulletTracerPrefab != null)
        {
            GameObject tracer = Instantiate(bulletTracerPrefab, firePoint.position, firePoint.rotation);
            tracer.SetActive(true);
            StartCoroutine(MoveTracer(tracer, hitPoint));
        }
    }

    //=========================================
    // BULLET MOVEMENT
    //=========================================
    IEnumerator MoveTracer(GameObject tracer, Vector3 target)
    {
        while (tracer != null && Vector3.Distance(tracer.transform.position, target) > 0.05f)
        {
            tracer.transform.position = Vector3.MoveTowards(tracer.transform.position, target, tracerSpeed * Time.deltaTime);
            tracer.transform.forward = (target - tracer.transform.position).normalized;
            yield return null;
        }
        if (tracer != null)
            Destroy(tracer);
    }

    //=========================================
    // PUBLIC - MODE SWITCHING
    //=========================================
    public void SetFireMode(FireMode mode)
    {
        fireMode = mode;
        Debug.Log("Fire Mode Changed To : " + fireMode);
        UpdateModeButtons();     // ← Update button visibility
    }
}