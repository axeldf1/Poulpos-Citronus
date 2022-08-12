using PlacedObjects;
using UnityEngine;

namespace Grid
{
    public class BuildingGhost : MonoBehaviour {

        private Transform _visual;
        private PlacedObjectTypeSo _placedObjectTypeSo;

        private void Start() {
            RefreshVisual();

            GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
        }

        private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
            RefreshVisual();
        }

        private void LateUpdate() {
            Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
            targetPosition.y = 0f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

            transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }

        private void RefreshVisual() {
            if (_visual != null) {
                Destroy(_visual.gameObject);
                _visual = null;
            }

            PlacedObjectTypeSo placedObjectTypeSo = GridBuildingSystem.Instance.PlacedObjectType;

            if (placedObjectTypeSo != null) {
                _visual = Instantiate(placedObjectTypeSo.visual, Vector3.zero, Quaternion.identity);
                _visual.parent = transform;
                _visual.localPosition = Vector3.zero;
                _visual.localEulerAngles = Vector3.zero;
                SetLayerRecursive(_visual.gameObject, 11);
            }
        }

        private void SetLayerRecursive(GameObject targetGameObject, int layer) {
            targetGameObject.layer = layer;
            foreach (Transform child in targetGameObject.transform) {
                SetLayerRecursive(child.gameObject, layer);
            }
        }

    }
}

