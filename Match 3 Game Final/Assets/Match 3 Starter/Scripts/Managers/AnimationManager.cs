using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;
    public SkeletonAnimation attacker;
    public SkeletonAnimation opponent;
    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<AnimationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayAttackerAnimation()
    {
        attacker.state.ClearTracks();
        attacker.state.SetAnimation(1, "skill", false);
        attacker.loop = false;

        SFXManager.instance.PlaySFX(Clip.attackerSkill);


        attacker.AnimationState.Complete += attackerAnimationCompleted;

    }

    private void attackerAnimationCompleted(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.trackIndex == 1)
        {
            PlayOpponentDefenseAnimation();
            PlayAttackerIdleAnimation(trackEntry);

        }
        else if (trackEntry.trackIndex == 2)
        {
            PlayAttackerIdleAnimation(trackEntry);
        }



    }

    private void PlayAttackerDefenseAnimation()
    {

        attacker.state.ClearTracks();
        attacker.state.SetAnimation(2, "fight-mode", false);
        attacker.loop = false;

        attacker.AnimationState.Complete += attackerAnimationCompleted;

    }

    private void PlayAttackerIdleAnimation(Spine.TrackEntry trackEntry)
    {
        attacker.state.SetAnimation(3, "idle", true);
        attacker.loop = true;
        attacker.AnimationState.Complete += null;

    }

    public void PlayOpponentAnimation()
    {
        opponent.state.ClearTracks();
        opponent.state.SetAnimation(1, "skill", false);
        opponent.loop = false;
        SFXManager.instance.PlaySFX(Clip.opponentSkill);


        opponent.AnimationState.Complete += opponentAnimationCompleted;

    }

    private void PlayOpponentDefenseAnimation()
    {
        opponent.state.ClearTracks();
        opponent.state.SetAnimation(2, "hit", false);
        opponent.loop = false;

        opponent.AnimationState.Complete += opponentAnimationCompleted;
    }

    private void opponentAnimationCompleted(Spine.TrackEntry trackEntry)
    {

        if (trackEntry.trackIndex == 1)
        {
            PlayAttackerDefenseAnimation();
            PlayOpponentIdleAnimation(trackEntry);

        }
        else if (trackEntry.trackIndex == 2)
        {
            PlayOpponentIdleAnimation(trackEntry);
        }
    }

    private void PlayOpponentIdleAnimation(Spine.TrackEntry trackEntry)
    {

        opponent.state.ClearTracks();
        opponent.state.SetAnimation(3, "idle", true);
        opponent.AnimationState.Complete += null;
    }
}
