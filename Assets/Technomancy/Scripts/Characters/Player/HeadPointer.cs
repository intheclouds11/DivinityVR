using UnityEngine;

namespace intheclouds
{
    public class HeadPointer : MonoBehaviour
    {
        public float pointerRadius = 0.5f;
        public float pointerMaxDistance = 20;
        public float maxDistanceToHoverEnemy = 1.5f;
        public LayerMask layerMask;
        public Color offensiveHighlightColor = new Color(1f, 0.2f, 0.2f);
        public Color supportHighlightColor = new Color(0.8f, 0.5f, 1);
        public Color neutralHighlightColor = new Color(0.1f, 0.5f, 0.2f);
        private BaseStats _combatantSelected;

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
                    DeselectCombatant();
                }
            }
        }

        private void HandlePointingAtEnemy(RaycastHit hit)
        {
            if (Vector3.Distance(hit.transform.position, transform.position) <= maxDistanceToHoverEnemy)
            {
                if (UserInventory.instance.IsHoldingWeapon())
                {
                    LocalUserObjects.instance.HUDController.ShowPointerUI(ActionType.Attack, "AP: 2");
                }
                else
                {
                    LocalUserObjects.instance.HUDController.HidePointerUI(ActionType.Attack);
                }
                
                _combatantSelected = hit.transform.GetComponentInParent<BaseStats>();
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
            else if (_combatantSelected)
            {
                DeselectCombatant();
            }
        }

        private void DeselectCombatant()
        {
            LocalUserObjects.instance.HUDController.HidePointerUI(ActionType.Attack);
            _combatantSelected.pointedAtByHead = false;
            _combatantSelected.modelHighlightEffect.highlighted = false;
            _combatantSelected = null;
        }
    }
}