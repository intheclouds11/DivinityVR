using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class AbilityPointer : MonoBehaviour
    {
        public Transform pointerEndTransform;
        public Vector3 maxDistanceVector = new Vector3(0, 0, 5);
        public LayerMask layerMask;
        public bool isOffensiveHighlight;
        public Color offensiveHighlightColor = new Color(1f, 0.2f, 0.2f);
        public Color supportHighlightColor = new Color(0.8f, 0.5f, 1);
        public BaseStats combatantSelected;
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            pointerEndTransform.localPosition = maxDistanceVector;
        }

        private void Update()
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, pointerEndTransform.position);
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistanceVector.z, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    combatantSelected = hit.transform.GetComponentInParent<BaseStats>();
                    if (isOffensiveHighlight)
                    {
                        combatantSelected.modelHighlightEffect.outlineColor = offensiveHighlightColor;
                    }
                    else
                    {
                        combatantSelected.modelHighlightEffect.outlineColor = supportHighlightColor;
                    }

                    combatantSelected.modelHighlightEffect.highlighted = true;
                    // Debug.Log($"abilitypointer hit valid target! {hit.transform.gameObject}", hit.transform);
                }
                else
                {
                    if (combatantSelected)
                    {
                        combatantSelected.modelHighlightEffect.highlighted = false;
                        combatantSelected = null;
                    }

                    // Debug.Log($"abilitypointer hit INVALID target! {hit.transform.gameObject}", hit.transform);
                }

                lineRenderer.SetPosition(1, hit.point);
                pointerEndTransform.position = hit.point;
            }
            else
            {
                pointerEndTransform.localPosition = maxDistanceVector;
                if (combatantSelected)
                {
                    combatantSelected.modelHighlightEffect.highlighted = false;
                    combatantSelected = null;
                }
                
                // Debug.Log($"abilitypointer hit NOTHING}");
            }
        }
    }
}