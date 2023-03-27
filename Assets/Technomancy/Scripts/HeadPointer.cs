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
        public BaseStats combatantSelected;
        private bool showPointerInfo;
        
        private void Update()
        {
            if (Physics.SphereCast(transform.position, 1, transform.forward, out RaycastHit hit, maxDistanceVector.z, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    var hudText = $"{hit.transform.GetComponentInParent<BaseStats>().Name} 3AP todo actual AP...";
                    LocalUserObjects.Instance.genericPointerInfo.ShowInfo(ActionType.Attack, hudText);
                    // pointerInfo.gameObject.SetActive(true);
                    // pointerInfo.AttackIcon.SetActive(true);
                    // pointerInfo.InfoText.text = $"{hit.transform.GetComponentInParent<BaseStats>().Name} 3AP todo actual AP...";
                    
                    combatantSelected = hit.transform.GetComponentInParent<BaseStats>();
                    if (!combatantSelected.Turn && !combatantSelected.pointedAtByHand)
                    {
                        if (combatantSelected.TryGetComponent(out PlayerStats player))
                        {
                            player.modelHighlightEffect.outlineColor = supportHighlightColor;
                        }
                        else if (combatantSelected.TryGetComponent(out PlayerStats enemy))
                        {
                            enemy.modelHighlightEffect.outlineColor = offensiveHighlightColor;
                        }

                        combatantSelected.modelHighlightEffect.highlighted = true;
                        combatantSelected.pointedAtByHead = true;
                        // Debug.Log($"abilitypointer hit valid target! {hit.transform.gameObject}", hit.transform);
                    }
                }
            }
            else
            {
                // pointerInfo.gameObject.SetActive(false);
                // pointerInfo.AttackIcon.SetActive(false);
                if (combatantSelected && !combatantSelected.Turn)
                {
                    LocalUserObjects.Instance.genericPointerInfo.HideInfo(ActionType.Attack);
                    combatantSelected.pointedAtByHead = false;
                    combatantSelected.modelHighlightEffect.highlighted = false;
                    combatantSelected = null;
                }
            }
        }
    }
}