using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource rifleShot, pistolShot, grenadeSound, jumpSound, reloadSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Footstep Clips")]
    public AudioClip[] footstepClips;

    [Header("Settings")]
    public float stepInterval = 0.45f;
    public float minMoveSpeed = 0.1f;

    public AudioSource runSound;
    private float stepTimer;

    private bool isMoving;

    void Update()
    {
        // Change this according to your movement script

        if (isMoving)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval;
        }
    }

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0)
            return;

        AudioClip clip =
            footstepClips[Random.Range(0, footstepClips.Length)];

        runSound.PlayOneShot(clip);
    }

    public void PlayRifleShot()
    {
        rifleShot.Play();
    }
    public void PlayPistolShot()
    {
        pistolShot.Play();
    }
    public void PlayGrenadeSound()
    {
        grenadeSound.Play();
    }
    public void PlayRunSound()
    {
        runSound.Play();
    }
    public void PlayJumpSound()
    {
        jumpSound.Play();
    }
    public void PlayReloadSound()
    {
        reloadSound.Play();
    }
    public void setMovementCondition(bool val)
    {
        isMoving = val;
    }

}
