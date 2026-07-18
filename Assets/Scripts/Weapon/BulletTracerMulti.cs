using Photon.Pun;
using UnityEngine;

public class BulletTracerMulti : MonoBehaviourPun
{
    public float speed = 80f;

    [Header("Lifetime")]
    public float lifeTime = 5f;

    private void Start()
    {
        // Only the owner is responsible for destroying the network object
        if (photonView.IsMine)
        {
            Invoke(nameof(DestroySelf), lifeTime);
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only owner handles collision
        if (!photonView.IsMine)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        if (photonView != null &&
            photonView.IsMine &&
            photonView.gameObject != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}