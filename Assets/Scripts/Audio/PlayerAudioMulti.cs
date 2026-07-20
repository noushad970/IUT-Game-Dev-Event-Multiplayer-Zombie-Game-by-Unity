using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerAudioMulti : MonoBehaviourPun
{
    [Header("Audio Sources")]
    public AudioSource footstepSource;
    public AudioSource weaponSource;
    public AudioSource voiceSource;

    [Header("Footsteps")]
    public AudioClip[] footstepClips;

    [Header("Weapons")]
    public AudioClip rifleShot;
    public AudioClip pistolShot;
    public AudioClip reloadClip;

    [Header("Voice")]
    public AudioClip jumpClip;
    public AudioClip hurtClip;
    public AudioClip deathClip;

    bool walking;

    //-----------------------------------------------------

    

    

    //-----------------------------------------------------

    public void PlayRifleShot()
    {
        if (!photonView.IsMine)
            return;
        Debug.Log("Playing rifle shot");
        photonView.RPC(nameof(RPC_PlayRifleShot), RpcTarget.All);
    }

    public void PlayPistolShot()
    {
        if (!photonView.IsMine)
            return;

        Debug.Log("Playing pistol shot");
        photonView.RPC(nameof(RPC_PlayPistolShot), RpcTarget.All);
    }

    public void PlayReload()
    {
        if (!photonView.IsMine)
            return;
        Debug.Log("Playing reload");
        photonView.RPC(nameof(RPC_PlayReload), RpcTarget.All);
    }

    public void PlayJump()
    {
        if (!photonView.IsMine)
            return;
        Debug.Log("Playing jump");
        photonView.RPC(nameof(RPC_PlayJump), RpcTarget.All);
    }

    public void PlayHurt()
    {
        if (!photonView.IsMine)
            return;
        Debug.Log("Playing hurt");
        photonView.RPC(nameof(RPC_PlayHurt), RpcTarget.All);
    }

    public void PlayDeath()
    {
        if (!photonView.IsMine)
            return;
        Debug.Log("Playing death");
        photonView.RPC(nameof(RPC_PlayDeath), RpcTarget.All);
    }

    //-----------------------------------------------------
    private Coroutine footstepRoutine;

    public float footstepDelay = 0.3f;

    public void StartFootsteps()
    {
        if (!photonView.IsMine)
            return;

        if (walking)
            return;

        walking = true;

        photonView.RPC(nameof(RPC_StartFootsteps), RpcTarget.All);
    }

    public void StopFootsteps()
    {
        if (!photonView.IsMine)
            return;

        if (!walking)
            return;

        walking = false;

        photonView.RPC(nameof(RPC_StopFootsteps), RpcTarget.All);
    }

    [PunRPC]
    void RPC_StartFootsteps()
    {
        if (footstepRoutine != null)
            StopCoroutine(footstepRoutine);

        footstepRoutine = StartCoroutine(FootstepRoutine());
    }

    [PunRPC]
    void RPC_StopFootsteps()
    {
        if (footstepRoutine != null)
        {
            StopCoroutine(footstepRoutine);
            footstepRoutine = null;
        }

        footstepSource.Stop();
    }

    IEnumerator FootstepRoutine()
    {
        while (walking)
        {
            if (footstepClips.Length > 0)
            {
                footstepSource.pitch = Random.Range(0.95f, 1.05f);

                footstepSource.PlayOneShot(
                    footstepClips[Random.Range(0, footstepClips.Length)]);
            }

            yield return new WaitForSeconds(footstepDelay);
        }

        footstepRoutine = null;
    }
    

    [PunRPC]
    void RPC_PlayRifleShot()
    {
        weaponSource.PlayOneShot(rifleShot);
    }

    [PunRPC]
    void RPC_PlayPistolShot()
    {
        weaponSource.PlayOneShot(pistolShot);
    }

    [PunRPC]
    void RPC_PlayReload()
    {
        weaponSource.PlayOneShot(reloadClip);
    }

    [PunRPC]
    void RPC_PlayJump()
    {
        voiceSource.PlayOneShot(jumpClip);
    }

    [PunRPC]
    void RPC_PlayHurt()
    {
        voiceSource.PlayOneShot(hurtClip);
    }

    [PunRPC]
    void RPC_PlayDeath()
    {
        voiceSource.PlayOneShot(deathClip);
    }
}