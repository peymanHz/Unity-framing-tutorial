using System.Collections;
using UnityEngine;

public class VFXManager : SingletonMonobehavior<VFXManager>
{
    private WaitForSeconds twoSecond;
    [SerializeField] private GameObject reapingPrefab = null;

    protected override void Awake()
    {
        base.Awake();

        twoSecond = new WaitForSeconds(2);
    }

    private void OnDisable()
    {
        EventHandler.HavertsActionEffectEvent -= displayHavertsActionEffect;
    }

    private void OnEnable()
    {
        EventHandler.HavertsActionEffectEvent += displayHavertsActionEffect;
    }

    private IEnumerator DisplayHavertsActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectGameObject.SetActive(false);
    }

    private void displayHavertsActionEffect(Vector3 effectPosition, HavertsActionEffect havertsActionEffect)
    {
        switch (havertsActionEffect)
        {
            case HavertsActionEffect.deciduousLeavesFalling:
                break;

            case HavertsActionEffect.pineConeFalling:
                break;

            case HavertsActionEffect.choppingTreeTrunck:
                break;

            case HavertsActionEffect.breakingStone:
                break;

            case HavertsActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisplayHavertsActionEffect(reaping, twoSecond)); 
                break;

            case HavertsActionEffect.none:
                break;

            default:
                break;
        }
    }

}
