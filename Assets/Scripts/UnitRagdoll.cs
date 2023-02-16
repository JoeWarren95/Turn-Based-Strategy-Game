using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    /// <summary>
    /// this script handles the what the ragdoll does once it's spawned into the scene
    /// </summary>

    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        //this line matches all the bones of our ragdoll to the Unit it needs to be spawned on
        MatchAllChildTransform(originalRootBone, ragdollRootBone);

        //this line allows the ragdoll to pop up slightly when it's spawned into the scene
        ApplyExplosionToRagdoll(ragdollRootBone, 300f, transform.position, 10f);
    }

    private void MatchAllChildTransform(Transform root, Transform clone)
    {
        ///This function will cycle through each child in the ragdoll prefab and match
        ///each transform + transform rotation to the position it was in before the ragdoll was spawned
        ///this way our radoll won't spawn in T-posed
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if(cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransform(child, cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        //this function is used to pop up the ragdoll
        foreach(Transform child in root)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
