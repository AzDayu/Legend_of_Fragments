using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackCooldown = 1.0f;
    public float attackColliderTime = 1.0f;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private GameObject weaponCollider;

    private float lastAttackTime = 0f;
    public bool isAttacking { get; private set; }

    private Animator anim;
    private PlayerInputHandler inputHandler;
    private PlayerAudio playerAudio;

    public bool isEquip;


    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    void Start()
    {
        if (weaponObject != null)
        {
            weaponObject.SetActive(false);
            weaponCollider.SetActive(false);
            isEquip = false;
        }
    }

    void Update()
    {
        if (inputHandler.AttackPressed && !isAttacking && Time.time >= lastAttackTime + attackCooldown && isEquip)
        {
            if (anim.GetBool("isGrounded") == true)
            {
                PerformAttack();
            }
        }

        if (inputHandler.WeaponPressed)
        {
            Equip();
        }

    }

    private void PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        anim.SetTrigger("doAttack");
        if (playerAudio != null)
        {
            playerAudio.PlayPlayerSound(PlayerSoundType.Slash);
        }

        Invoke(nameof(ResetAttack), attackCooldown);

        Invoke(nameof(WeaponCollider), attackColliderTime);

    }

    private void ResetAttack()
    {
        isAttacking = false;
        weaponCollider.SetActive(false);
    }

    private void Equip()
    {
        isEquip = !isEquip;

        if (weaponObject != null)
        {
            weaponObject.SetActive(isEquip);

        }
    }

    private void WeaponCollider()
    {
        weaponCollider.SetActive(true);
    }

}