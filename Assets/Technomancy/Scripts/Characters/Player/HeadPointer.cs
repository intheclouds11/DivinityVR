using System;
using System.Linq;
using UnityEngine;

namespace intheclouds
{
    public class HeadPointer : MonoBehaviour
    {
        public float pointerRadius = 0.5f;
        public float pointerMaxDistance = 20;
        public float maxDistanceToHoverEnemy = 10f;
        public float maxDistanceToAttackEnemy = 1.5f;
        public LayerMask layerMask;
        public Color offensiveHighlightColor = new Color(1f, 0.2f, 0.2f);
        public Color supportHighlightColor = new Color(0.8f, 0.5f, 1);
        public Color neutralHighlightColor = new Color(0.1f, 0.5f, 0.2f);
        private BaseStats _combatantSelected;
        private PlayerHUDController _hudController;


        private void Awake()
        {
            _hudController = LocalUserObjects.instance.HUDController;
        }

        private void Update()
        {
            if (Physics.SphereCast(transform.position, pointerRadius, transform.forward, out RaycastHit hit, pointerMaxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    HandlePointingAtEnemy(hit);
                }
                // else if () // todo add checks for other objects of interest: items, NPCs
                // {
                //     
                // }
            }
            else
            {
                if (_combatantSelected)
                {
                    OutOfHoverRange();
                    OutOfAttackRange();
                }
            }
        }

        private void HandlePointingAtEnemy(RaycastHit hit)
        {
            _combatantSelected = hit.transform.GetComponentInParent<BaseStats>();

            if (!_combatantSelected.isAlive)
            {
                OutOfAttackRange();
                OutOfHoverRange();
                return;
            }
            
            if (Vector3.Distance(hit.transform.position, transform.position) <= maxDistanceToAttackEnemy)
            {
                if (UserInventory.instance.IsHoldingWeapon())
                {
                    _hudController.ShowPointerUI(ActionType.Attack, "AP: 2");
                }
                else
                {
                    _hudController.HidePointerUI(ActionType.Attack);
                }
            }
            else
            {
                OutOfAttackRange();
            }
            
            if (Vector3.Distance(hit.transform.position, transform.position) <= maxDistanceToHoverEnemy)
            {
                if (_combatantSelected.statusEffectsContainer.statusEffectList.Any())
                {
                    _hudController.ShowEnemyStatusEffectsUI(_combatantSelected.statusEffectsContainer.statusEffectList);
                }
                
                if (!_combatantSelected.Turn && !_combatantSelected.pointedAtByHand)
                {
                    if (_combatantSelected.TryGetComponent(out PlayerStats player))
                    {
                        player.modelHighlightEffect.outlineColor = supportHighlightColor;
                    }
                    else if (_combatantSelected.TryGetComponent(out BaseStats npc))
                    {
                        if (npc.InCombat)
                        {
                            npc.modelHighlightEffect.outlineColor = offensiveHighlightColor;
                        }
                        else
                        {
                            npc.modelHighlightEffect.outlineColor = neutralHighlightColor;
                        }
                    }

                    _combatantSelected.modelHighlightEffect.highlighted = true;
                    _combatantSelected.pointedAtByHead = true;
                }
            }
            else
            {
                OutOfHoverRange();
            }
        }

        private void OutOfHoverRange()
        {
            _hudController.HideEnemyStatusEffectsUI();
            _combatantSelected.pointedAtByHead = false;
            _combatantSelected.modelHighlightEffect.highlighted = false;
            _combatantSelected = null;
        }

        private void OutOfAttackRange()
        {
            _hudController.HidePointerUI(ActionType.Attack);
        }
    }
}