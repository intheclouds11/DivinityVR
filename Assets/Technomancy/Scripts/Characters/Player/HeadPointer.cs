using TMPro;
using UnityEngine;

namespace intheclouds
{
    public class HeadPointer : MonoBehaviour
    {
        public Vector3 maxDistanceVector = new Vector3(0, 0, 20);
        public LayerMask layerMask;
        public Color offensiveHighlightColor = new Color(1f, 0.2f, 0.2f);
        public Color supportHighlightColor = new Color(0.8f, 0.5f, 1);
        public Color neutralHighlightColor = new Color(0.1f, 0.5f, 0.2f);
        public BaseStats combatantSelected;
        private bool showPointerInfo;

        private void Update()
        {
            if (Physics.SphereCast(transform.position, 1, transform.forward, out RaycastHit hit, maxDistanceVector.z, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    LocalUserObjects.Instance.HUDController.ShowPointerUI(ActionType.Selection, hit.transform.GetComponentInParent<BaseStats>().Name);

                    if (Vector3.Distance(hit.transform.position, transform.position) <= 1.5f)
                    {
                        LocalUserObjects.Instance.HUDController.ShowPointerUI(ActionType.Attack, "Attack AP: 2"); // todo: change this to only show if close range weapon equipped
                        combatantSelected = hit.transform.GetComponentInParent<BaseStats>();
                        if (!combatantSelected.Turn && !combatantSelected.pointedAtByHand)
                        {
                            if (combatantSelected.TryGetComponent(out PlayerStats player))
                            {
                                player.modelHighlightEffect.outlineColor = supportHighlightColor;
                            }
                            else if (combatantSelected.TryGetComponent(out BaseStats npc))
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

                            combatantSelected.modelHighlightEffect.highlighted = true;
                            combatantSelected.pointedAtByHead = true;
                            // Debug.Log($"headpointer hit valid target! {hit.transform.gameObject}", hit.transform);
                        }
                    }
                    else if (combatantSelected)
                    {
                        LocalUserObjects.Instance.HUDController.HidePointerUI(ActionType.Attack);
                        combatantSelected.pointedAtByHead = false;
                        combatantSelected.modelHighlightEffect.highlighted = false;
                        combatantSelected = null;
                    }
                }
                // else if () // todo add checks for other objects of interest: items, NPCs
                // {
                //     
                // }
            }
            else
            {
                LocalUserObjects.Instance.HUDController.HidePointerUI(ActionType.Selection);
            }
        }
    }
}