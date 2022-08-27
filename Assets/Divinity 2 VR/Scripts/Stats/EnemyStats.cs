using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class EnemyStats : MonoBehaviour, ICharacter
    {
        public EnemyStatsSO enemyStatsSO;
        public string Name { get; set; }
        public GameObject CharacterType { get; set; }
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider physicalArmorSlider;
        [SerializeField] private Slider magicArmorSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI physicalArmorText;
        [SerializeField] private TextMeshProUGUI magicArmorText;
        [SerializeField] private GameObject hitPopupPrefab;
        [SerializeField] private GameObject hitPopupsParent;
        public float hitPopupSpeed = 0.5f;

        [Tooltip("Enemy Stats")]
        public int currentHealth
        {
            get { return _currentHealth; }
            set
            {
                _currentHealth = value;
                UpdateHealthInfo();
            }
        }
        private int _currentHealth;
        public int maxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                UpdateHealthInfo();
            }
        }
        private int _maxHealth;
        public int currentPhysicalArmor
        {
            get { return _currentPhysicalArmor; }
            set
            {
                _currentPhysicalArmor = value;
                UpdatePhysicalArmorInfo();
            }
        }
        private int _currentPhysicalArmor;
        public int maxPhysicalArmor
        {
            get { return _maxPhysicalArmor; }
            set
            {
                _maxPhysicalArmor = value;
                UpdatePhysicalArmorInfo();
            }
        }
        private int _maxPhysicalArmor;
        public int currentMagicArmor
        {
            get { return _currentMagicArmor; }
            set
            {
                _currentMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        private int _currentMagicArmor;
        public int maxMagicArmor
        {
            get { return _maxMagicArmor; }
            set
            {
                _maxMagicArmor = value;
                UpdateMagicArmorInfo();
            }
        }
        private int _maxMagicArmor;
        public int currentAP
        {
            get { return _currentAP; }
            set
            {
                _currentAP = value;
                if (_currentAP == 0)
                {
                    turn = false;
                    GetComponent<EnemyAI>().EndTurn();
                }
            }
        }
        private int _currentAP;
        public int maxAP
        {
            get { return _maxAP; }
            set { _maxAP = value; }
        }
        private int _maxAP;
        public int earnedXP
        {
            get { return _earnedXP; }
            set { _earnedXP = value; }
        }
        private int _earnedXP;
        public bool turn
        {
            get { return _turn; }
            set
            {
                _turn = value;
                if (_turn)
                {
                    GetComponent<EnemyAI>().StartTurn();
                    // todo: apply status effects here!
                }
                else
                {
                    GameManager.Instance.nextTurn = true;
                }
            }
        }
        private bool _turn;

        public CharacterAttributes attributes;

        public bool isAlive = true;
        public AudioClip[] hurtAudioClips;
        public AudioClip[] deadAudioClips;
        public event Action EnemyDamaged;
        public event Action EnemyDied;
        private AudioSource audioSource;
        private List<GameObject> activeHitPopups = new List<GameObject>();

        public bool attackOnSight = true;
        public bool enemyEngaged;

        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
            if (!enemyStatsSO)
            {
                Debug.LogError("No EnemyStatsSO assigned!", this);
                return;
            }

            InitializeStats();
        }

        private void InitializeStats()
        {
            attributes = GetComponent<CharacterAttributes>();
            CharacterType = gameObject;
            Name = enemyStatsSO.Name;
            maxHealth = enemyStatsSO.maxHealth;
            currentHealth = enemyStatsSO.currentHealth;
            maxPhysicalArmor = enemyStatsSO.maxPhysicalArmor;
            currentPhysicalArmor = enemyStatsSO.currentPhysicalArmor;
            maxMagicArmor = enemyStatsSO.maxMagicArmor;
            currentMagicArmor = enemyStatsSO.currentMagicArmor;
            maxAP = enemyStatsSO.maxAP;
            currentAP = enemyStatsSO.currentAP;
            earnedXP = enemyStatsSO.earnedXP;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (attackOnSight && !enemyEngaged)
                {
                    GameManager.Instance.UpdateGameState(GameState.CombatStart, this);
                }
            }
        }

        private void Update()
        {
            HitPopupUpdate();
        }

        private void HitPopupUpdate()
        {
            if (activeHitPopups.Count > 0)
            {
                foreach (var activeHitPopup in activeHitPopups)
                {
                    if (activeHitPopup)
                    {
                        activeHitPopup.transform.Translate(0, hitPopupSpeed * Time.deltaTime, 0);
                        activeHitPopup.transform.localScale +=
                            (activeHitPopup.transform.localScale + new Vector3(0.001f, 0.001f, 0.001f)) * Time.deltaTime;
                    }
                }
            }
        }

        public void TakeDamage(PlayerStats wieldingUser, DamageType damageType, int damage)
        {
            if (!isAlive) return;

            if (!enemyEngaged)
            {
                GameManager.Instance.UpdateGameState(GameState.CombatStart, this);
            }

            var newHitPopup = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            newHitPopup.GetComponent<TextMeshProUGUI>().text = damage.ToString();
            activeHitPopups.Add(newHitPopup);

            if (damageType == DamageType.Physical)
            {
                if (currentPhysicalArmor - damage >= 0)
                {
                    currentPhysicalArmor -= damage;
                }
                else
                {
                    currentHealth -= damage - currentPhysicalArmor;
                    currentPhysicalArmor = 0;
                }

                newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else if (damageType == DamageType.Magic)
            {
                if (currentMagicArmor - damage >= 0)
                {
                    currentMagicArmor -= damage;
                }
                else
                {
                    currentHealth -= damage - currentMagicArmor;
                    currentMagicArmor = 0;
                }

                newHitPopup.GetComponent<TextMeshProUGUI>().color = Color.blue;
            }

            if (currentHealth > 0)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.volume = 1;
                audioSource.PlayOneShot(hurtAudioClips[Random.Range(0, hurtAudioClips.Length)]);
                EnemyDamaged?.Invoke();
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.volume = 0.7f;
                audioSource.PlayOneShot(deadAudioClips[Random.Range(0, deadAudioClips.Length)]);
                isAlive = false;

                // todo: something about this causes crashes randomly
                // var rbBody = transform.GetChild(0).GetComponent<Rigidbody>();
                // rbBody.isKinematic = false;
                // rbBody.useGravity = true;
                // Destroy(transform.GetChild(0).GetChild(0).GetComponent<Rigidbody>()); // remove head rb since setting like above causes crash..

                EnemyDied?.Invoke();
                wieldingUser.ObtainXP(earnedXP);
            }
        }

        private void UpdateMagicArmorInfo()
        {
            magicArmorSlider.maxValue = maxMagicArmor;
            magicArmorSlider.value = currentMagicArmor;
            magicArmorText.text = $"{currentMagicArmor}/{maxMagicArmor}";
        }

        private void UpdatePhysicalArmorInfo()
        {
            physicalArmorSlider.maxValue = maxPhysicalArmor;
            physicalArmorSlider.value = currentPhysicalArmor;
            physicalArmorText.text = $"{currentPhysicalArmor}/{maxPhysicalArmor}";
        }

        private void UpdateHealthInfo()
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
}