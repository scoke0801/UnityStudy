using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace Enemy
{

    public class EnemyHealth : HealthBase
    {
        public float health = 100f;
        public GameObject healthHUD;
        public GameObject bloodSample;
        public bool headShot;

        private float totalHealth;

        private Transform weapon;
        private Transform hud;
        private RectTransform healthBar;

        private float originalBarScale;

        private HealthHUD healthUI;

        private Animator anim;
        private StateController controller;
        private GameObject gameController;

        private void Awake()
        {
            hud = Instantiate(healthHUD, transform).transform;

            if (!hud.gameObject.activeSelf)
            {
                hud.gameObject.SetActive(true);
            }

            totalHealth = health;
            healthBar = hud.transform.Find("Bar").GetComponent<RectTransform>();

            healthUI = hud.GetComponent<HealthHUD>();
            originalBarScale = healthBar.sizeDelta.x;

            anim = GetComponent<Animator>();
            controller = GetComponent<StateController>();
            gameController = GameObject.FindGameObjectWithTag("GameController");

            foreach(Transform child in anim.GetBoneTransform(HumanBodyBones.RightHand))
            {
                weapon = child.Find("muzzle");
                if(weapon != null)
                {
                    break;
                }
            }

            weapon = weapon.parent;
        }

        private void UpdateHealthBar()
        {
            float scaleFactor = health / totalHealth;

            healthBar.sizeDelta = new Vector2(scaleFactor * originalBarScale, healthBar.sizeDelta.y);
        }

        private void RemoveAllForces()
        {
            foreach(Rigidbody body in GetComponentsInChildren<Rigidbody>())
            {
                body.isKinematic = false;
                body.velocity = Vector3.zero;
            }
        }

        public void Kill()
        {
            foreach(MonoBehaviour monoBehaviour in GetComponents<MonoBehaviour>())
            {
                if(this != monoBehaviour)
                {
                    Destroy(monoBehaviour);
                }
            }

            Destroy(GetComponent<NavMeshAgent>());
            RemoveAllForces();
            controller.focusSight = false;
            anim.SetBool(Defs.AnimatorKey.Aim, false);
            anim.SetBool(Defs.AnimatorKey.Corner, false);
            anim.enabled = false;
            Destroy(weapon.gameObject);
            Destroy(hud.gameObject);
            IsDead = true;
        }

        public override void TakeDamage(Vector3 location, Vector3 direction, float damage, Collider bodyPart = null, GameObject origin = null)
        {
            if (!IsDead && headShot && bodyPart.transform == anim.GetBoneTransform(HumanBodyBones.Head))
            {
                damage *= 10;
                gameController.SendMessage("HeadShotCallback", SendMessageOptions.DontRequireReceiver);
            }

            Instantiate(bloodSample, location, Quaternion.LookRotation(-direction), transform);

            health -= damage;

            if (!IsDead)
            {
                anim.SetTrigger(Defs.AnimatorKey.Hit);

                healthUI.SetVisible();
                UpdateHealthBar();
                controller.variables.feelAlert = true;
                controller.personalTarget = controller.aimTarget.position;
            }

            if(health <= 0)
            {
                if (!IsDead)
                {
                    Kill();
                }

                // 렉돌 ... 하면 좋은데 일단 단순하게 넘어지도록
                Rigidbody rigid = bodyPart.GetComponent<Rigidbody>();
                rigid.mass = 40;
                rigid.AddForce(100f * direction.normalized, ForceMode.Impulse);
            }
        }
    }
}