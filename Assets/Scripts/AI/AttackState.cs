using System.Collections;
using UnityEngine;

public class AttackState : State
{
    private Avatar _avatarAgent = null;
    private Opponent _opponentComponent = null;
    private Coroutine _attackingCheckCoroutine = null;

    private const float CHANCE_FOR_NEXT_ATTACK = 0.45f;
    
    public override void Enter(Avatar agent)
    {
        if (_opponentComponent == null)
        {
            _opponentComponent = agent.GetComponent<Opponent>();
        }

        _avatarAgent = agent;
        _attackingCheckCoroutine = null;

        agent.ResetAttack();
        agent.OnDamageDone += OnAvatarTakeDamage;
    }

    public override void Exit(Avatar agent)
    {
        agent.ResetAttack();
        agent.OnDamageDone -= OnAvatarTakeDamage;
    }

    public override void Update(Avatar agent)
    {
        _opponentComponent.MoveToDesiredLocationFromTarget(0.6f, 0.3f);

        if (_attackingCheckCoroutine == null)
        {
            agent.Attack();
            _attackingCheckCoroutine = agent.StartCoroutine(Co_AttackingCheck());
        }
    }

    private void OnAvatarTakeDamage(Avatar avatarToDamage, float damage, bool isKO, Vector3 impactPoint)
    {
        _avatarAgent.StopCoroutine(_attackingCheckCoroutine);
        _attackingCheckCoroutine = null;

        if (Random.Range(0.0f, 1.0f) < CHANCE_FOR_NEXT_ATTACK)
        {
            _avatarAgent.Attack();
            _attackingCheckCoroutine = _avatarAgent.StartCoroutine(Co_AttackingCheck());
        }
        else
        {
            _opponentComponent.UpdateStateToIdle();
        }
    }

    private IEnumerator Co_AttackingCheck()
    {
        yield return new WaitForSeconds(2);

        _opponentComponent.UpdateStateToIdle();
    }
}