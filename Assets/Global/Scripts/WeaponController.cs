using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon mechanics Settings")]
    public float rateFire;
    public float throwForce = 500f;
    private float isFiring;
    public float isAiming;

    [Header("Ammo")]
    public int ammo;
    public int magazineAmmo;

    [Header("Fire Mode")]
    public bool SemiAuto;
    public bool FullAuto;
    private float shootTimer = 0f;

    [Header("Weapon Sound")]
    public AudioClip shootSound;

    [Header("Animation / Clamp")]
    public bool canShoot;
    public bool canAim;

    [Header("Fx Effect")]
    public GameObject muzzleFlash;

    [Header("Ray Target")]
    public GameObject enemyTarget;
    public GameObject cameraTarget;

    private GameObject playerCamera;
    private GameObject objectParent;
    private GameObject objectPivot;
    private Transform muzzlePosition;
    private Animator animator;

    void Awake()
    {
        muzzlePosition = GameObject.Find("muzzlePosition").transform;
        playerCamera = GameObject.Find("PlayerCamera");
        objectParent = GameObject.Find("Object");
        objectPivot = GameObject.Find("ObjectPivot");
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        PlayerRayEnemy();
        PlayerRayCamera();
        FullAutoShoot();
    }

    #region [----Fonction Update----]

    void PlayerRayEnemy()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 100f, Color.black);

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 100f) == true)
        {
            enemyTarget = hit.transform.gameObject;
        }
        else
        {
            enemyTarget = null;
        }
    }

    void PlayerRayCamera()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 2.5f, Color.cyan);

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 2.5f) == true)
        {
            cameraTarget = hit.transform.gameObject;
        }
        else
        {
            cameraTarget = null;
        }
    }

    void FullAutoShoot()
    {
        if (transform.parent.name == "ObjectPivot" && isFiring == 1 && FullAuto == true && Time.time >= shootTimer && ammo > 0 && canShoot == true)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position);
            GameObject fxBullet = Instantiate(muzzleFlash, muzzlePosition.position, Quaternion.identity);
            fxBullet.transform.SetParent(transform);
            Destroy(fxBullet, 0.1f);
            shootTimer = Time.time + 1f / rateFire;

            if (ammo > 1)
            {
                animator.SetTrigger("shoot");
            }

            else if (ammo == 1)
            {
                animator.SetTrigger("shoot2");
            }

            ammo -= 1;
        }

        else if (ammo == 0)
        {

            print("tes a sec");
            shootTimer = 0f;
        }
    }

    #endregion

    #region [----Fonction----]

    void OnTake()
    {
        if (cameraTarget != null && cameraTarget.transform.parent.name == "Object" && objectPivot.transform.childCount == 0)
        {
            animator.SetTrigger("take");
            cameraTarget.transform.SetParent(objectPivot.transform);
            cameraTarget.transform.localPosition = Vector3.zero;
            cameraTarget.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            cameraTarget.transform.localScale = Vector3.one;
            cameraTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            cameraTarget.GetComponent<MeshCollider>().enabled = false;
            StartCoroutine(TimeCalculatorTake());
        }
    }

    void OnThrow()
    {
        if (objectPivot.transform.childCount == 1)
        {
            transform.SetParent(objectParent.transform);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GetComponent<MeshCollider>().enabled = true;
            GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward * throwForce);
            canShoot = false;
            canAim = false;
        }
    }

    void OnLeftClick(InputValue value)
    {
        isFiring = value.Get<float>();

        if (transform.parent.name == "ObjectPivot" && SemiAuto == true && isFiring == 1 && canShoot == true && ammo > 0)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position);
            GameObject fxBullet = Instantiate(muzzleFlash, muzzlePosition.position, Quaternion.identity);
            fxBullet.transform.SetParent(transform);
            Destroy(fxBullet, 0.1f);

            if (ammo > 1)
            {
                animator.SetTrigger("shoot");
            }

            else if (ammo == 1)
            {
                animator.SetTrigger("shoot2");
            }

            ammo -= 1;

            if (enemyTarget != null)
            {
                if (enemyTarget.name == "Enemy 1" || enemyTarget.name == "Enemy 2")
                {
                    print("ennemy toucher hagglag");
                }
            }
        }

        else if (ammo == 0)
        {
            print("tes a sec");
        }
    }

    void OnRightClick(InputValue value)
    {
        isAiming = value.Get<float>();

        if (transform.parent.name == "ObjectPivot" && isAiming == 1 && canAim == true)
        {
            transform.localPosition = new Vector3(-0.25f, 0.15f, 0.05f);
        }

        else if (transform.parent.name == "ObjectPivot" && isAiming == 0)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    void OnReloading()
    {
        if (transform.parent.name == "ObjectPivot" && ammo < 18)
        {
            transform.localPosition = Vector3.zero;
            ammo = magazineAmmo;
            animator.SetTrigger("reload");
            print("Reloaded!");
            StartCoroutine(TimeCalculatorReload());
        }
    }

    void OnShootMode()
    {
        if (SemiAuto == true)
        {
            FullAuto = true;
            SemiAuto = false;
        }
        else
        {
            FullAuto = false;
            SemiAuto = true;
        }
    }

    #endregion

    #region [----TimeCalculator / CanShoot----]

    IEnumerator TimeCalculatorReload()
    {
        canShoot = false;
        canAim = false;
        yield return new WaitForSeconds(92f / 30f);
        canAim = true;
        canShoot = true;
    }

    IEnumerator TimeCalculatorTake()
    {
        canShoot = false;
        canAim = false;
        yield return new WaitForSeconds(19f / 30f);
        canAim = true;
        canShoot = true;
    }

    #endregion
}




