using Cinemachine;
using System.Collections;
using UnityEngine;

public class LightningStrike: Skill
{
    private CharacterBlockComponent moveZero;
    private PlayerCameraController cameraController;
    private bool bAttack = false;

    protected override void Reset()
    {
        base.Reset();

        type = SkillType.LightningStrike;
    }

    protected override void Awake()
    {
        base.Awake();

        moveZero = rootObject.GetComponent<CharacterBlockComponent>();
        cameraController = rootObject.GetComponent<PlayerCameraController>();
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        moveZero.CanBlock = false;
        string name = "Electric_Charge";

        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

    public override void Begin_Collision(AnimationEvent e)
    {
        //검기 사운드 재생
        string name = "Slash_LightningStrike";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);


        hitIndex++;
        Vector3 startPosition = rootObject.transform.position + Vector3.up + rootObject.transform.forward;
        Vector3 endPosition = startPosition + rootObject.transform.forward * 5.0f;
        Collider[] colliders = Physics.OverlapCapsule(startPosition, endPosition, 1.0f, (1 << 6));
        bAttack = false;

        foreach (Collider collider in colliders)
        {
            if (collider.transform != rootObject.transform)
            {
                OnTriggerEnter(collider);
                bAttack = true;
            }
        }

        if (!bAttack)
            return;

        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 1.0f);
        cameraController.ChangeCamera(PlayerCameraController.CameraType.Front);

        CharacterController controller = rootObject.GetComponent<CharacterController>();
        controller.Move(rootObject.transform.forward * 5.0f);
    }
    public override void Play_Particle()
    {
        if (!bAttack)
            return;
        base.Play_Particle();

        if (doActionData.hittingDatas[0].Particle == null)
            return;

        animator.speed = 0.0f;
        StartCoroutine(Recover_AnimatorSpeed(1.0f));

        Vector3 particlePosition = rootObject.transform.position - rootObject.transform.forward * 5.0f;
        Instantiate(doActionData.hittingDatas[0].Particle, particlePosition, Quaternion.identity);
    }

    public override void End_DoAction()
    {
        base.End_DoAction();

        moveZero.CanBlock = true;
    }

    private IEnumerator Recover_AnimatorSpeed(float time)
    {
        yield return new WaitForSeconds(time);
        animator.speed = 1.0f;
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseIn, 1.0f);
        cameraController.ChangeCamera(PlayerCameraController.CameraType.Basic);
    }

}