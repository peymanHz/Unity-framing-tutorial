using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class AnimationOverride : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] soAnimationTypeArrey = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAniamtion;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        //initialise animation type dictionary keyed by animation clip
        animationTypeDictionaryByAniamtion = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArrey)
        {
            animationTypeDictionaryByAniamtion.Add(item.animationClip, item);

        }

        //initilise animation type dictionary keyed by string
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArrey)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    public void ApplyCharacterCustomisationParameters(List<CharachterAttribute> charachterAttributeList)
    {
        //Stopwatch s1 = Stopwatch.StartNew();

        //loop through all character attributes and set the animation override controller for each
        foreach (CharachterAttribute charachterAttribute in charachterAttributeList)
        {
            Animator currentAnimator = null;
            List<KeyValuePair<AnimationClip, AnimationClip>> animationkeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animatorSOAssetName = charachterAttribute.characterPart.ToString();

            // find animators in scene that match the scriptable object animator type
            Animator[] animatorsArrey = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animatorsArrey)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            //get base currrent animations for animator
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationslist = new List<AnimationClip>(aoc.animationClips);

            foreach (AnimationClip animationClip in animationslist)
            {
                //find animation in dictionary
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAniamtion.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {
                    string key = charachterAttribute.characterPart.ToString() + charachterAttribute.partVariantColour.ToString() +
                      charachterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_animationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_animationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_animationType.animationClip;

                        animationkeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            //apply animation updates to animation override controller and then update animator with the new controller
            aoc.ApplyOverrides(animationkeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }

}
