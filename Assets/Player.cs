using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private AAAA playerInput;
    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _lookAction;
    private InputAction _modeAction;
    private InputAction _blockAction;
    private InputAction _dodgeAction;
    private InputAction _abilityAction;
    private InputAction _spAbilityAction;
    private InputAction _sprintAction;
    private InputAction _pauseAction;
    private Vector2 MoveDir;
    private Rigidbody _rigidbody;
    private Animator animator;
    private CinemachineImpulseSource _cameraShake;
    [SerializeField] Camera _camera;
    [SerializeField] public Camera _cameraCinematic;
    [SerializeField] public Camera _cameraCinematic2;
    [SerializeField] Scriptable _data;
    [SerializeField] private UnityEvent Anim1;
    [SerializeField] private UnityEvent Anim2;
    [SerializeField] private UnityEvent Anim3;
    [SerializeField] private UnityEvent Anim4;
    [SerializeField] private UnityEvent Anim5;
    [SerializeField] private UnityEvent NormalMode;
    [SerializeField] private UnityEvent SpecialMode;
    [SerializeField] private UnityEvent RunMode;
    [SerializeField] private UnityEvent IdleMode;
    [SerializeField] private UnityEvent SpecialAttack;
    [SerializeField] private GameObject AttackTrigger;
    [SerializeField] private GameObject AttackTrigger2;
    [SerializeField] private Material SpecialMaterial;
    [SerializeField] private Material NormalMaterial;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Renderer _rendererCube;
    [SerializeField] private VisualEffect slashMaterial;
    [SerializeField] private VisualEffect slashMaterial2;
    [SerializeField] private VisualEffect slashMaterial3;
    [SerializeField] private VisualEffect slashMaterial4;
    [SerializeField] private VisualEffect slashMaterial5;
    [SerializeField] private GameObject cubeSpecial;
    [SerializeField] private GameObject Timeline;
    private PlayableDirector timeDirector;

    [SerializeField] private ParticleSystem SpecialParticle;
    [SerializeField] private ParticleSystem SpecialParticle2;


    [SerializeField] private GameObject fireSlash;

    public AfterImageScript afterImages;
    private bool canAfterImage = true;
    private VisualEffect fireSlashVFX;

    private Volume volume;
    float timer;
    private float _rotationX;
    private float _rotationY;
    private float _lookSensitivity = 0.1f;
    private float _maxLookAngle = 30f;
    public int _count;
    public int _trueCount;
    public GameObject _gameObject;
    bool test1;
    bool test2;
    bool test3;
    bool test4;
    bool test5;
    bool IsWalking;
    bool isSpecial;
    bool isSprinting;
    bool isShielding;
    public bool isCinematic;
    private void Awake()
    {
        playerInput = new AAAA();
        
    }
    void Start()
    {

        fireSlashVFX = fireSlash.GetComponentInChildren<VisualEffect>();
        test1 = false;
        test2 = false;
        test3 = false;
        test4 = false;
        test5 = false;
        IsWalking = false;
        isSprinting = false;
        volume = _camera.GetComponentInChildren<Volume>();
        _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        _cameraShake = GetComponent<CinemachineImpulseSource>();
        if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.active = false;
        }
        _count = 0;
        _trueCount = 0;
        timer = 0.0f;
        NormalMode.Invoke();
        slashMaterial.SetFloat("_GreenMode", 0f);
        slashMaterial2.SetFloat("_GreenMode", 0f);
        slashMaterial3.SetFloat("_GreenMode", 0f);
        slashMaterial4.SetFloat("_GreenMode", 0f);
        slashMaterial5.SetFloat("_GreenMode", 0f);
        fireSlashVFX.SetFloat("GreenModeAbility", 0f);
        IdleMode.Invoke();
        isSpecial = false;
        AttackTrigger.SetActive(false);
        AttackTrigger2.SetActive(false);
        //var mainModule = psHitEnemy.main;
        //initialColor = mainModule.startColor.color;
        _data.isSpecialMode = false;
        _data._player = this;
        isShielding = false;
        cubeSpecial.SetActive(false);
        _rendererCube = cubeSpecial.GetComponent<Renderer>();
        timeDirector = Timeline.GetComponent<PlayableDirector>();
        isCinematic = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetBool("Walking", IsWalking);
        Move();
        Combo();
    }

    private void OnEnable()
    {
        _moveAction = playerInput.Player.Move;
        _moveAction.Enable();
        _moveAction.performed += OnMovement;
        _moveAction.canceled += OnMoveCancelled;

        _attackAction = playerInput.Player.Attack;
        _attackAction.Enable();
        _attackAction.performed += OnAttacking;
        _attackAction.canceled += OnAttackCancelled;

        _lookAction = playerInput.Player.Look;
        _lookAction.Enable();
        _lookAction.performed += LookAction;

        _modeAction = playerInput.Player.Mode;
        _modeAction.Enable();
        _modeAction.performed += ModeAction;

        _blockAction = playerInput.Player.Block;
        _blockAction.Enable();
        _blockAction.performed += BlockAction;
        _blockAction.canceled += BlockActionCancelled;

        _sprintAction = playerInput.Player.Sprint;
        _sprintAction.Enable();
        _sprintAction.performed += SprintAction;
        _sprintAction.canceled += SprintActionCancelled;

        _pauseAction = playerInput.Player.Pause;
        _pauseAction.Enable();
        _pauseAction.performed += OnPause;


        _dodgeAction = playerInput.Player.Dodge;
        _dodgeAction.Enable();
        _dodgeAction.performed += DodgeAction;

        _abilityAction = playerInput.Player.Ability;
        _abilityAction.Enable();
        _abilityAction.performed += AbilityAction;

        _spAbilityAction = playerInput.Player.SpecialAbility;
        _spAbilityAction.Enable();
        _spAbilityAction.performed += SpAbilityAction;
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _moveAction.performed -= OnMovement;
        _moveAction.canceled -= OnMoveCancelled;

        _attackAction.Disable();
        _attackAction.performed -= OnAttacking;
        _attackAction.canceled -= OnAttackCancelled;

        _lookAction.Disable();
        _lookAction.performed -= LookAction;

        _modeAction.Disable();
        _modeAction.performed -= ModeAction;

        _blockAction.Disable();
        _blockAction.performed -= BlockAction;
        _blockAction.canceled -= BlockActionCancelled;

        _pauseAction.Disable();
        _pauseAction.performed -= OnPause;

        _moveAction.actionMap.Disable();

    }

    private void OnMovement(InputAction.CallbackContext context)
    {
        MoveDir = context.ReadValue<Vector2>();
        IsWalking = true;
        RunMode.Invoke();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        _gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void OnPauseExit(InputAction.CallbackContext context)
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
    private void OnAttacking(InputAction.CallbackContext context)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsName("Idle") || stateInfo.IsName("Walking"))
        {
            _count = 0;
            _trueCount = 0;
            animator.ResetTrigger("attack");
            animator.ResetTrigger("count1");
            animator.ResetTrigger("count2");
            animator.ResetTrigger("count3");
            animator.ResetTrigger("count4");
        }
        switch (_count)
        {
            default:
                {
                    break;
                }
            case 0:
                {
                    animator.SetTrigger("attack");
                    test1 = true;
                    break;
                }
            case 1:
                {
                    animator.SetTrigger("count1");
                    test2 = true;
                    break;
                }
            case 2:
                {
                    animator.SetTrigger("count2");
                    test3 = true;
                    break;
                }
            case 3:
                {
                    animator.SetTrigger("count3");
                    test4 = true;
                    break;
                }
            case 4:
                {
                    animator.SetTrigger("count4");
                    test5 = true;
                    break;
                }
        }
        _count++;


    }

    private void Combo()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsName("attack1") && test1 == true)
        {
            test1 = false;
            
            

            StartCoroutine(OnCorouAttack(0.30f,1));
        }
        else if (stateInfo.IsName("attack2") && test2 == true)
        {
            test2 = false;
            StartCoroutine(OnCorouAttack(0.25f, 2));
            //_cameraShake.GenerateImpulse();
            //Anim2.Invoke();
        }
        else if (stateInfo.IsName("attack3") && test3 == true)
        {
            test3 = false;
            
            StartCoroutine(OnCorouAttack(0.25f,3));
            //_cameraShake.GenerateImpulse();
            //Anim3.Invoke();
        }
        else if (stateInfo.IsName("attack4") && test4 == true)
        {
            test4 = false;
           
            StartCoroutine(OnCorouAttack(0.25f,4));
            //_cameraShake.GenerateImpulse();
            //Anim4.Invoke();
        }
        else if (stateInfo.IsName("attack5") && test5 == true)
        {
            test5 = false;
            StartCoroutine(OnCorouAttack(0.15f,5));
            //_cameraShake.GenerateImpulse();
            //Anim5.Invoke();
        }
    }

    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        MoveDir = Vector2.zero;
        IsWalking = false;
        IdleMode.Invoke();
    }
    
    private void OnAttackCancelled(InputAction.CallbackContext context)
    {

    }

    private void LookAction(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();

        _rotationX -= lookInput.y * _lookSensitivity;
        _rotationY += lookInput.x * _lookSensitivity;

        _rotationX = Mathf.Clamp(_rotationX, -_maxLookAngle, _maxLookAngle);

    }

    private void ModeAction(InputAction.CallbackContext context)
    {
        isSpecial = !isSpecial;
        if (!isSpecial)
        {
            NormalMode.Invoke();
            if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
            {
                colorAdjustments.active = false;
                
            }
            _renderer.material = NormalMaterial;
            slashMaterial.SetFloat("_GreenMode", 0f);
            slashMaterial2.SetFloat("_GreenMode", 0f);
            slashMaterial3.SetFloat("_GreenMode", 0f);
            slashMaterial4.SetFloat("_GreenMode", 0f);
            slashMaterial5.SetFloat("_GreenMode", 0f);
            fireSlashVFX.SetFloat("GreenModeAbility", 0f);
            _data.isSpecialMode = isSpecial;

        }
        else
        {
            SpecialMode.Invoke();
            if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
            {
                colorAdjustments.active = true;
            }
            _renderer.material = SpecialMaterial;
            slashMaterial.SetFloat("_GreenMode", 0.22f);
            slashMaterial2.SetFloat("_GreenMode", 0.22f);
            slashMaterial3.SetFloat("_GreenMode", 0.22f);
            slashMaterial4.SetFloat("_GreenMode", 0.22f);
            slashMaterial5.SetFloat("_GreenMode", 0.22f);
            fireSlashVFX.SetFloat("GreenModeAbility", 0.22f);
            _data.isSpecialMode = isSpecial;
        }
    }

    private void BlockAction(InputAction.CallbackContext context) {
        isShielding = true;
        animator.SetBool("Shielding", isShielding);
        StartCoroutine(OnCorouShield());

    }

    private void BlockActionCancelled(InputAction.CallbackContext context)
    {
        isShielding = false;
        animator.SetBool("Shielding", isShielding);
        _renderer.materials[1].SetFloat("_ApparitionFloat", 1);
    }

    private void SprintAction(InputAction.CallbackContext context)
    {
        isSprinting = true;
        animator.SetBool("Running", isSprinting);
    }

    private void SprintActionCancelled(InputAction.CallbackContext context)
    {
        isSprinting = false;
        animator.SetBool("Running", isSprinting);
    }

    private void DodgeAction(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Dodging");
        StartCoroutine(OnCorouDodge());
        
    }

    private void AbilityAction(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Ability");
        StartCoroutine(OnCorouAbility());
       
    }

    private void SpAbilityAction(InputAction.CallbackContext context)
    {
        if(isSpecial)
        {cubeSpecial.SetActive(true);
            _attackAction.actionMap.Disable();
            StartCoroutine(OnCorouSpecial());
        }
    }

    private IEnumerator StartAfterImagesDuringSprint()
    {
        afterImages.startAfterImages();

        yield return new WaitForSeconds(2f);

        if (!isSpecial)
        {
            canAfterImage = false;
        }

        afterImages.stopAfterImages();
    }

    private void Move()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsName("Idle") || stateInfo.IsName("Walking") || stateInfo.IsName("Sprinting"))
        {
            var fwd = _camera.transform.forward;
            fwd.y = 0;
            fwd.Normalize();
            var r = _camera.transform.right;
            r.y = 0;
            r.Normalize();

            Vector3 forwardVect = fwd * MoveDir.y * 10;
            Vector3 rightVect = r * MoveDir.x * 10;
            if (isSprinting)
            {
                forwardVect *= 2;
                rightVect *= 2;

                if(canAfterImage)
                {
                    StartCoroutine(StartAfterImagesDuringSprint());
                }
            }
            else
            {
                canAfterImage = true;
            }
            forwardVect.y = _rigidbody.velocity.y;
            _rigidbody.velocity = forwardVect + rightVect;

            var dir = forwardVect + rightVect;
            dir.y = 0;
            transform.LookAt(transform.position + (dir));
        }
        else
        {
            var fwd = _camera.transform.forward;
            fwd.y = 0;
            fwd.Normalize();
            var r = _camera.transform.right;
            r.y = 0;
            r.Normalize();

            Vector3 forwardVect = fwd * 0 * 10;
            Vector3 rightVect = r * 0 * 10;
            if (isSprinting)
            {
                forwardVect *= 2;
                rightVect *= 2;
            }
            forwardVect.y = _rigidbody.velocity.y;
            _rigidbody.velocity = forwardVect + rightVect;

            var dir = forwardVect + rightVect;
            dir.y = 0;
            transform.LookAt(transform.position + (dir));
        }
    }
    private IEnumerator OnCorouAttack(float testtimer,int id)
    {
        while (true)
        {
            
            timer += Time.deltaTime;
            if (timer >= testtimer)
            {
                break;
            }
            yield return null;
        }
        switch (id)
        {
            case 1:
                {
                    //_cameraShake.GenerateImpulse();
                    //slashMaterial.transform.position = slashTransform1.position;
                    //slashMaterial.transform.rotation = slashTransform1.rotation;
                    //slashMaterial.transform.SetParent(null);
                    AudioManager.Instance.PlaySFX("FireSlash");
                    Anim1.Invoke();
                    break;
                }
            case 2:
                {
                    AudioManager.Instance.PlaySFX("FireSlash");
                    _cameraShake.GenerateImpulse();
                    Anim2.Invoke();
                    _rigidbody.AddForce(transform.forward*3000);
                    break;
                }
            case 3:
                {
                    //_cameraShake.GenerateImpulse();
                    AudioManager.Instance.PlaySFX("FireSlash");
                    Anim3.Invoke();
                    break;
                }
            case 4:
                {
                    AudioManager.Instance.PlaySFX("FireSlash");
                    _cameraShake.GenerateImpulse();
                    Anim4.Invoke();
                    _rigidbody.AddForce(transform.forward * 3000);
                    break;
                }
            case 5:
                {
                    //_cameraShake.GenerateImpulse();
                    AudioManager.Instance.PlaySFX("FireSlash");
                    Anim5.Invoke();
                    animator.ResetTrigger("attack");
                    animator.ResetTrigger("count1");
                    animator.ResetTrigger("count2");
                    animator.ResetTrigger("count3");
                    animator.ResetTrigger("count4");
                    _count = 0;
                    break;
                }
                
        }
        
        timer = 0f;
        StartCoroutine(OnCorouHitBox());

    }
    private IEnumerator OnCorouShield()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            float value = Mathf.Lerp(1, -1, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            _renderer.materials[1].SetFloat("_ApparitionFloat", value);
            if (isShielding == false)
            {
                _renderer.materials[1].SetFloat("_ApparitionFloat", 1);
                break;
            }
            yield return null;
        }

    }
    private IEnumerator OnCorouHitBox()
    {
        yield return new WaitForSeconds(0.2f);
        AttackTrigger.SetActive(true);
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f)
            {
                break;
            }
            yield return null;
        }
        AttackTrigger.SetActive(false);
        _trueCount++;

    }

    private IEnumerator OnCorouDodge()
    {
        yield return new WaitForSeconds(0.2f);
        afterImages.startAfterImages();
        _rigidbody.AddForce(-transform.forward * 5000);
    }

    private IEnumerator OnCorouAbility()
    {
        yield return new WaitForSeconds(0.4f);
        GameObject go = Instantiate(fireSlash);
        AudioManager.Instance.PlaySFX("Special");
        go.transform.position = transform.position + transform.right * 1f;
        go.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 40);
    }

    private IEnumerator OnCorouSpecial()
    {
        float elapsedTime = 0f;
        Time.timeScale = 0.5f;
        while (elapsedTime < 0.12f)
        {
            float value1 = Mathf.Lerp(1, 0f, elapsedTime / 0.12f);
            float value2 = Mathf.Lerp(1, 0.5f, elapsedTime / 0.12f);
            float value3 = Mathf.Lerp(1, 0f, elapsedTime / 0.12f);
            elapsedTime += Time.deltaTime;
            _rendererCube.materials[1].SetFloat("_ApparitionCross", value1);
            _rendererCube.materials[1].SetFloat("_CrossSize", value2);
            _rendererCube.materials[0].SetFloat("_GlobalSize", value3);
            yield return null;
        }
        Time.timeScale = 0.25f;
        SpecialParticle.Play();
        yield return new WaitForSeconds(0.12f);
        SpecialParticle.Stop();
        Time.timeScale = 1f;
        cubeSpecial.SetActive(false);
        _count = 0;
        _trueCount = 0;
        animator.ResetTrigger("attack");
        animator.ResetTrigger("count1");
        animator.ResetTrigger("count2");
        animator.ResetTrigger("count3");
        animator.ResetTrigger("count4");
        yield return new WaitForSeconds(1f);
        _cameraCinematic.gameObject.SetActive(true);
        _cameraCinematic2.transform.position = new Vector3(_cameraCinematic.transform.position.x + 4.58f - 0.275f, _cameraCinematic.transform.position.y + 4.77f - 2.07f, _cameraCinematic.transform.position.z + 2.06f - 2.848f);
        //_cameraCinematic2.transform.rotation = Quaternion.Euler(_cameraCinematic.transform.rotation.x + 41.824f - 31.824f, _cameraCinematic.transform.rotation.y + 119.1f -212.01f, _cameraCinematic.transform.rotation.z + 0 - 0);
        _cameraCinematic2.transform.LookAt(gameObject.transform.position + gameObject.transform.forward * 20f);
        _cameraCinematic2.gameObject.SetActive(true);
        timeDirector.Play();
        isCinematic = true;
        yield return new WaitForSeconds(8f);
        isSpecial = !isSpecial;
        NormalMode.Invoke();
        if (volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.active = false;

        }
        _renderer.material = NormalMaterial;
        slashMaterial.SetFloat("_GreenMode", 0f);
        slashMaterial2.SetFloat("_GreenMode", 0f);
        slashMaterial3.SetFloat("_GreenMode", 0f);
        slashMaterial4.SetFloat("_GreenMode", 0f);
        slashMaterial5.SetFloat("_GreenMode", 0f);
        fireSlashVFX.SetFloat("GreenModeAbility", 0f);
        _data.isSpecialMode = isSpecial;
        SpecialAttack.Invoke();
        StartCoroutine(Shake(_cameraCinematic2.transform.position));
        AttackTrigger2.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SpecialParticle2.Stop();
        AttackTrigger2.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        _cameraCinematic.gameObject.SetActive(false);
        _cameraCinematic2.gameObject.SetActive(false);
        _attackAction.actionMap.Enable();
        isCinematic = false;




    }
    private IEnumerator Shake(Vector3 originalPosition)
    {
        float shakeTimeRemaining = 1f;  // Duration of the shake

        while (shakeTimeRemaining > 0)
        {
            // Randomize the offset for the shake
            float xOffset = UnityEngine.Random.Range(-0.4f, 0.4f);
            float yOffset = UnityEngine.Random.Range(-0.4f, 0.4f);
            float zOffset = UnityEngine.Random.Range(-0.4f, 0.4f);

            // Apply the shake offset to the camera's position
            _cameraCinematic2.transform.position = originalPosition + new Vector3(xOffset, yOffset, zOffset);

            // Decrease the remaining shake time
            shakeTimeRemaining -= Time.deltaTime;

            // Wait for the next frame to continue shaking
            yield return null;
        }

        // After the shake is done, reset the camera's position
        _cameraCinematic2.transform.position = originalPosition;
    }
}
