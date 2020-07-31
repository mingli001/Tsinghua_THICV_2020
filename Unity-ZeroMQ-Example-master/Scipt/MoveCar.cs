using UnityEngine;
using UnityEngine.XR.WSA.Input;
public class MoveCar : MonoBehaviour
{
    GameObject vehicle;
    GestureRecognizer recognizer;
    float speed;
    int RoadRotationDegree;
    Vector3 direction,cornertreeposition;
    bool IsMoving = true;                     //控制车辆的启停，刚进入场景车辆静止
    // Start is called before the first frame update
    void Start()
    {
        vehicle = this.gameObject;
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.DoubleTap | GestureSettings.Tap |GestureSettings.Hold);
        recognizer.TappedEvent += TapEventHandler;
        recognizer.StartCapturingGestures();
        RoadRotationDegree = 3;
        speed = 1f; ;
        transform.position = new Vector3(1,-0.7f,2.1f);
    }

    // Update is called once per frame
    void Update()
    {
        GoRound(RoadRotationDegree, IsMoving);
    }
    //控制小车开始转圈，速度可控
    void TapEventHandler(InteractionSourceKind source,int tapcount,Ray headray)
    {
        if (source == InteractionSourceKind.Hand && tapcount == 1) {
            OneTapResponse(headray,IsMoving);
        }
        if (InteractionSourceKind.Hand == source && tapcount == 2) {
            DoubleTapResponse();
        }
        if (InteractionSourceKind.Other == source)
        {
            print("wkehfqghqlgf");
        }
    }
  
    //对单Tap进行响应
    void OneTapResponse(Ray headray,bool istomove) {
        RaycastHit raycast;
        if (Physics.Raycast(headray, out raycast)) {
            if (raycast.transform.name.Equals("SpeedControl")) {
                speed = 0.5f * speed;
                if (speed < 0.5) {
                    speed = 0.5f;
                }
                return; 
            }
        }
        GoRound(RoadRotationDegree,istomove);
        IsMoving = !IsMoving;
    }
    //小车移动速度加速，每点击一次增加0.3,当小车speed大于3可能会冲出场景，所以需要进行限制
    void DoubleTapResponse() {
        speed =2*speed;
        if (speed > 8)
        {
            speed = 8;
        }
    }
    //点按三次对小车进行减速，最低速0.1
    public float GetSpeed() {
        return this.speed;
    }
    void GoRound(int rotationDegree,bool istomove)
    {
        if (!istomove)
        {
            return;
        }
            switch (rotationDegree)
            {
                case 0:
                    vehicle.transform.localEulerAngles = new Vector3(0, 0, 0);
                    direction = new Vector3(0, 0, -0.1f);
                    vehicle.transform.localPosition += speed * direction;
                    cornertreeposition = new Vector3(-9.2f, -0.7f, 1.9f);
                    if (IsInRange(vehicle.transform.position, cornertreeposition, 0.8f))
                    {
                        RoadRotationDegree = 3;
                    }
                    break;
                case 1:
                    vehicle.transform.localEulerAngles = new Vector3(0, 90, 0);
                    direction = new Vector3(-0.1f, 0, 0);
                    vehicle.transform.localPosition += speed * direction;
                    cornertreeposition = new Vector3(-9, -0.7f, 32.5f);
                    if (IsInRange(vehicle.transform.position, cornertreeposition, 0.6f))
                    {
                        RoadRotationDegree = 0;
                    }
                    break;
                case 2:
                    vehicle.transform.localEulerAngles = new Vector3(0, 180, 0);
                    direction = new Vector3(0, 0, 0.1f);
                    vehicle.transform.localPosition += speed * direction;
                    cornertreeposition = new Vector3(9.2f, -0.7f, 32.5f);
                    if (IsInRange(vehicle.transform.position, cornertreeposition, 0.6f))
                    {
                        RoadRotationDegree = 1;
                    }
                    break;
                case 3:
                    vehicle.transform.localEulerAngles = new Vector3(0, 270, 0);
                    direction = new Vector3(0.1f, 0, 0);
                    vehicle.transform.localPosition += speed * direction;
                    cornertreeposition = new Vector3(9.2f, -0.7f, 2.4f);
                    if (IsInRange(vehicle.transform.position, cornertreeposition, 0.5f))
                    {
                        RoadRotationDegree = 2;
                    }
                    break;
                default:
                    break;
            }
    }
    bool IsInRange(Vector3 currentposition, Vector3 cornertreeposition, float triggerdistance)
    {
        float actualdistance = Mathf.Pow(Mathf.Pow(currentposition.x - cornertreeposition.x, 2) + Mathf.Pow(currentposition.y - cornertreeposition.y, 2) + Mathf.Pow(currentposition.z - cornertreeposition.z, 2), 0.5f);
        if (actualdistance <= triggerdistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
