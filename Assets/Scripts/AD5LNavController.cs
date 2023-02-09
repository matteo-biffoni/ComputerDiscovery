using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class AD5LNavController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    public Animator Ad5LIoAnimator;

    public Transform Box;

    public Transform BoxPosition;
    public Transform InsideIOPosition;
    public Transform OutsideIOPosition;
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int PickBox = Animator.StringToHash("pickBox");
    public Transform HandL, HandR;
    private bool _updateBoxPosition;
    private Vector3 _boxOffsetWithHand;
    private CapsuleCollider _collider;
    private BoxCollider _boxCollider;
    public CapsuleCollider WallCollider;
    private static readonly int AD5LDeparting = Animator.StringToHash("AD5L_Departing");
    private static readonly int AD5LArriving = Animator.StringToHash("AD5L_Arriving");

    private void OnEnable()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _boxCollider = Box.GetComponent<BoxCollider>();
    }

    private void OnDisable()
    {
        _navMeshAgent = null;
        _animator = null;
    }

    private bool TargetReached()
    {
        if (_navMeshAgent.pathPending) return false;
        if (!(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)) return false;
        return !_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude <= 0f;
    }

    public IEnumerator SendBoxInNetwork()
    {
        var transform1 = transform;
        var startPos = transform1.position;
        var startOrientation = transform1.rotation;
        _collider.enabled = false;
        _boxCollider.enabled = false;
        WallCollider.enabled = false;
        _animator.SetBool(Walking, true);
        var navMeshPath = new NavMeshPath();
        if (_navMeshAgent.CalculatePath(BoxPosition.position, navMeshPath))
        {
            _navMeshAgent.SetDestination(BoxPosition.position);
        }
        StartCoroutine(SmoothTurnTo(Quaternion.Euler(0, 90, 0)));
        yield return new WaitUntil(TargetReached);
        _animator.SetBool(Walking, false);
        _animator.SetBool(PickBox, true);
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "AD5L_Armature|AD5L_PickBox");
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        var boxPos = Box.position;
        _updateBoxPosition = true;
        yield return new WaitUntil(() =>
            _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "AD5L_Armature|AD5L_IdleWithBox");
        Ad5LIoAnimator.SetBool(AD5LDeparting, true);
        AudioManager.Play(Ad5LIoAnimator.transform, AudioManager.Instance.AD5LDoor);
        yield return new WaitForSeconds(0.5f);
        Ad5LIoAnimator.SetBool(AD5LDeparting, false);
        _animator.SetBool(Walking, true);
        if (_navMeshAgent.CalculatePath(InsideIOPosition.position, navMeshPath))
        {
            _navMeshAgent.SetDestination(InsideIOPosition.position);
        }
        yield return new WaitUntil(TargetReached);
        yield return new WaitWhile(() => Ad5LIoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        if (_navMeshAgent.CalculatePath(OutsideIOPosition.position, navMeshPath))
        {
            _navMeshAgent.SetDestination(OutsideIOPosition.position);
        }
        yield return new WaitUntil(TargetReached);
        if (_navMeshAgent.CalculatePath(InsideIOPosition.position, navMeshPath))
        {
            _navMeshAgent.SetDestination(InsideIOPosition.position);
        }
        yield return new WaitForSeconds(0.8f);
        Ad5LIoAnimator.SetBool(AD5LArriving, true);
        yield return new WaitForSeconds(0.1f);
        Ad5LIoAnimator.SetBool(AD5LArriving, false);
        yield return new WaitUntil(TargetReached);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Play(Ad5LIoAnimator.transform, AudioManager.Instance.AD5LDoor);
        yield return new WaitWhile(() => Ad5LIoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        if (_navMeshAgent.CalculatePath(BoxPosition.position, navMeshPath))
        {
            _navMeshAgent.SetDestination(BoxPosition.position);
        }
        yield return new WaitUntil(TargetReached);
        _animator.SetBool(PickBox, false);
        _animator.SetBool(Walking, false);
        yield return new WaitUntil(() =>
            _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "AD5L_Armature|AD5L_DropBox");
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        _updateBoxPosition = false;
        Box.position = boxPos;
        yield return new WaitUntil(() =>
            _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "AD5L_Armature|AD5L_Idle");
        yield return new WaitForSeconds(0.3f);
        _animator.SetBool(Walking, true);
        if (_navMeshAgent.CalculatePath(startPos, navMeshPath))
        {
            _navMeshAgent.SetDestination(startPos);
        }
        yield return new WaitUntil(TargetReached);
        _animator.SetBool(Walking, false);
        yield return StartCoroutine(SmoothTurnTo(startOrientation));
        _collider.enabled = true;
        _boxCollider.enabled = true;
        WallCollider.enabled = true;
        transform.GetComponent<NetworkManager>().CameBackFromNetwork();
    }
    
    private IEnumerator SmoothTurnTo(Quaternion rotation)
    {
        while (Quaternion.Angle(transform.rotation, rotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3f);
            yield return null;
        }
    }

    private void Update()
    {
        if (_updateBoxPosition)
        {
            var pos = new Vector3();
            var position = HandL.position;
            pos.y = position.y + 0.05f;
            pos.x = position.x;
            var position1 = HandR.position;
            pos.z = position1.z + (position.z - position1.z) / 2;
            Box.position = pos;
        }
    }
}
