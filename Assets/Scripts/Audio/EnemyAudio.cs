using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public static EnemyAudio Instance;
    [Header("Audio Sources")]
    public AudioSource voiceSource;
    public AudioSource footstepSource;

    [Header("Voice Clips")]
    public AudioClip[] roarClips;
    public AudioClip[] attackClips;
    public AudioClip[] dieClips;

    [Header("Footstep Clips")]
    public AudioClip[] footstepClips;

    [Header("Settings")]
    public float minRoarInterval = 5f;
    public float maxRoarInterval = 12f;

    private bool isDead;

    private void Start()
    {
        Instance=this;
        ScheduleNextRoar();
        footstepSource.volume = 0.3f;
    }

    //-------------------------------------------------------
    // ROAR
    //-------------------------------------------------------

    void ScheduleNextRoar()
    {
        if (isDead)
            return;

        float delay = Random.Range(minRoarInterval, maxRoarInterval);

        Invoke(nameof(PlayRandomRoar), delay);
    }

    void PlayRandomRoar()
    {
        if (isDead)
            return;

        if (roarClips.Length == 0)
            return;

        AudioClip clip =
            roarClips[Random.Range(0, roarClips.Length)];

        voiceSource.PlayOneShot(clip);

        ScheduleNextRoar();
    }

    //-------------------------------------------------------
    // FOOTSTEP
    //-------------------------------------------------------

    public void StartFootsteps()
    {
        if (isDead)
            return;

        if (footstepClips.Length == 0)
            return;

        if (footstepSource.isPlaying)
            return;

        footstepSource.clip =
            footstepClips[Random.Range(0, footstepClips.Length)];

        footstepSource.loop = true;
        footstepSource.Play();
    }

    public void StopFootsteps()
    {
        footstepSource.Stop();
    }

    //-------------------------------------------------------
    // ATTACK
    //-------------------------------------------------------

    public void PlayAttack()
    {
        if (isDead)
            return;

        if (attackClips.Length == 0)
            return;

        voiceSource.PlayOneShot(
            attackClips[Random.Range(0, attackClips.Length)]);
    }

    //-------------------------------------------------------
    // DIE
    //-------------------------------------------------------

    public void PlayDie()
    {
        if (isDead)
            return;

        isDead = true;

        CancelInvoke();

        StopFootsteps();

        if (dieClips.Length == 0)
            return;

        voiceSource.PlayOneShot(
            dieClips[Random.Range(0, dieClips.Length)]);
    }
}