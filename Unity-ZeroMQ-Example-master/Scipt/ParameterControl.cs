using UnityEngine.XR.WSA.Input;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using NetMQ.Sockets;
using NetMQ;
using System.Threading;

[RequireComponent(typeof(CarController))]
public class ParameterControl : MonoBehaviour
{
    //UI setting
    [Range(0,1)][SerializeField] private float m_Steering;
    [Range(0, 1)] [SerializeField] private float m_Accelaration;
    [SerializeField] private float m_Footbrake;
    [SerializeField] private float m_Handbrake;

    //parameters setting
    public float steering
    {
        get;
        set;
    }
    public float accelaration
    {
        get;
        set;
    }
    public float footbrake
    {
        get;
        set;
    }
    public float handbrake
    {
        get;
        set;
    }
    
    //class parameter setting
    CarController carController;
    Rigidbody carRigidbpdy;
    GestureRecognizer gestureRecognizer;
    Thread thread;
    TextMesh mesh;
    PublisherSocket pub = new PublisherSocket();


    // Start is called before the first frame update
    void Start()
    {
        steering = 0;
        accelaration = 0;
        footbrake = 0;
        handbrake = 0;
        carController = GetComponent<CarController>();
        carRigidbpdy = GetComponent<Rigidbody>();
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.DoubleTap | GestureSettings.Tap | GestureSettings.Hold);
        gestureRecognizer.TappedEvent += TapEventHandler;
        gestureRecognizer.StartCapturingGestures();
        thread = new Thread(new ThreadStart(GetHololensInfo));
        thread.Start();
        mesh = GameObject.Find("Canvas").GetComponentInChildren<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        carController.Move(steering,accelaration,footbrake,handbrake);
        if (carRigidbpdy.velocity.magnitude<0.1)
        {
            mesh.text = "Current speed is :0" + "    Accelaration degree is： " + accelaration;
        }
        else {
            mesh.text = "Current speed is :" + carRigidbpdy.velocity.magnitude.ToString() + "    Accelaration degree is： " + accelaration;
        }
        
    }
    void TapEventHandler(InteractionSourceKind source, int tapcount, Ray headray)
    {
        if (source == InteractionSourceKind.Hand && tapcount == 1)
        {
            OneTapResponse(headray);
        }
        if (InteractionSourceKind.Hand == source && tapcount == 2)
        {
            DoubleTapResponse(headray);
        }
        if (InteractionSourceKind.Other == source)
        {
         //   print("wkehfqghqlgf");
        }
    }
    public void OneTapResponse(Ray ray)
    {
        RaycastHit raycast;
        if (Physics.Raycast(ray, out raycast))
        {
            string hitname = raycast.transform.name;
            if (hitname != null && hitname != "")
            {
                switch (hitname)
                {
                    case "SpeedUp":
                        SpeedUp();
                        break;
                    case "SpeedDown":
                        SpeedDown();
                        break;
                    case "TurnLeft":
                        TurnLeft();
                        break;
                    case "TurnRight":
                        TurnRight();
                        break;
                }
            }
        }
    }
    public void DoubleTapResponse(Ray ray)
    {
        RaycastHit raycast;
        if (Physics.Raycast(ray, out raycast))
        {
            if (raycast.transform.name.Equals("SpeedUp"))
            {

            }
        }
    }
    public void SpeedUp() {
        if(accelaration<=0.9f&&accelaration>=0)
         accelaration = accelaration + 0.1f;
    }
    public void SpeedDown()
    {
        if (accelaration <= 1f && accelaration >= 0.1f)
            accelaration = accelaration - 0.1f;
    }
    public void TurnLeft()
    {
        if (steering <= 1f && accelaration >= -0.9f)
            steering = steering - 0.1f;
    }
    public void TurnRight()
    {
        if (accelaration <= 0.9f && accelaration >= -1)
            steering = steering + 0.1f;
    }
    public void Reset()
    {
        accelaration = 0;
        steering = 0;
        this.transform.localPosition = new Vector3(-1, -5, 10);
        this.transform.localEulerAngles = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

    }
    private void OnDisable()
    {
       
        gestureRecognizer.Dispose();
        thread.Abort();
        pub.Close();
    }
    public void GetHololensInfo() {
        
        pub.Bind("tcp://*:10088");
      //  HololensMessage hololensmessage;
        while (true) {
            // hololensmessage = new HololensMessage(1,"somewhere",this.gameObject.transform.localPosition);
            //  string message = HololensMessage.GetHololensMessage(hololensmessage);
            // JObject jObject = JObject.Parse(message);
            // hitinfo =(string) jObject["Name"];
            // mesh.text = message;
            // print(message);
            pub.SendMoreFrame("Windows").SendFrame(carRigidbpdy.velocity.magnitude.ToString());
            Thread.Sleep(2000);
        }
    }
}
