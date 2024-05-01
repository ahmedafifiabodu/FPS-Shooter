using System.Collections;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    [SerializeField] private float _autoDisableAfter = 5f;

    private void OnEnable() => StartCoroutine(DisableAfterTime(_autoDisableAfter));

    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}