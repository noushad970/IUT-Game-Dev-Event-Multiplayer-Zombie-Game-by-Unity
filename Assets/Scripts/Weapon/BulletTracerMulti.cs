using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletTracerMulti : MonoBehaviourPun
{
    public float speed = 80f;

    private void Start()
    {
        // Auto destroy after some time to prevent leaks
        if (photonView.IsMine)
        {
            Invoke(nameof(DestroySelf), 3f);
        }
    }

    private void Update()
    {
        // Move the tracer forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only process collision on the owner to avoid duplicate effects
        if (!photonView.IsMine) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Trigger blood effect on all clients
            // photonView.RPC("RPC_PlayBloodEffect", RpcTarget.All, transform.position);

            // Destroy bullet on all clients
            PhotonNetwork.Destroy(gameObject);
        }
    }



    private void DestroySelf()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}