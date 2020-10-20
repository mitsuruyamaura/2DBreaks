using UnityEngine;

public class FixPhysicalState : MonoBehaviour {

    public bool fixPosition = true;
    public bool fixRotation = true;

    Vector3 own_initialRot;
    Vector3 own_initialLocalPos;
    Vector3 parent_pos;

    void Awake() {
        own_initialRot = this.transform.eulerAngles;
        own_initialLocalPos = this.transform.localPosition;
    }

    void Update() {
        if (fixPosition) {
            parent_pos = this.transform.parent.position;
            this.transform.position = parent_pos + own_initialLocalPos;
        }

        if (fixRotation) {
            this.transform.eulerAngles = own_initialRot;
        }
    }
}
