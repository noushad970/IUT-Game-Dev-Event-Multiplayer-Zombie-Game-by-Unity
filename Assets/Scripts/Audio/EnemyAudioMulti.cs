using Photon.Pun;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class EnemyAudioMulti : MonoBehaviourPun
{
    public AudioSource footstepSource;
    public AudioSource voiceSource;

    [Header("Footsteps")]
    public AudioClip[] footstepClips;
    public float footstepDelay = 0.3f;

    [Header("Zombie")]
    public AudioClip attackClip;
    public AudioClip deathClip;

    private bool walking;
    private Coroutine footstepRoutine;

    //-----------------------------------------------------

    private void Awake()
    {
        voiceSource = GetComponent<AudioSource>();

        Transform[] childs = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in childs)
        {
            if (child.name == "Footstep")
            {
                footstepSource = child.GetComponent<AudioSource>();
                break;
            }
        }

        if (footstepSource == null)
        {
            Debug.LogWarning("Footstep AudioSource not found.");
        }
    }

    //-----------------------------------------------------

    public void StartWalk()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (walking)
            return;

        walking = true;

        photonView.RPC(nameof(RPC_StartWalk), RpcTarget.All);
    }

    public void StopWalk()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!walking)
            return;

        walking = false;

        photonView.RPC(nameof(RPC_StopWalk), RpcTarget.All);
    }

    public void PlayAttack()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC(nameof(RPC_PlayAttack), RpcTarget.All);
    }

    public void PlayDeath()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        photonView.RPC(nameof(RPC_PlayDeath), RpcTarget.All);
    }

    //-----------------------------------------------------
    // RPC
    //-----------------------------------------------------

    [PunRPC]
    void RPC_StartWalk()
    {
        if (footstepRoutine != null)
            StopCoroutine(footstepRoutine);

        footstepRoutine = StartCoroutine(FootstepRoutine());
    }

    [PunRPC]
    void RPC_StopWalk()
    {
        if (footstepRoutine != null)
        {
            StopCoroutine(footstepRoutine);
            footstepRoutine = null;
        }

        if (footstepSource != null)
            footstepSource.Stop();
    }

    [PunRPC]
    void RPC_PlayAttack()
    {
        if (voiceSource != null && attackClip != null)
            voiceSource.PlayOneShot(attackClip);
    }

    [PunRPC]
    void RPC_PlayDeath()
    {
        if (voiceSource != null && deathClip != null)
            voiceSource.PlayOneShot(deathClip);
    }

    //-----------------------------------------------------
    // Footsteps
    //-----------------------------------------------------

    IEnumerator FootstepRoutine()
    {
        while (walking)
        {
            if (footstepSource != null &&
                footstepClips != null &&
                footstepClips.Length > 0)
            {
                footstepSource.pitch = Random.Range(0.95f, 1.05f);

                footstepSource.PlayOneShot(
                    footstepClips[Random.Range(0, footstepClips.Length)]);
            }

            yield return new WaitForSeconds(footstepDelay);
        }

        footstepRoutine = null;
    }
}