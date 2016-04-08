using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

	/*
	 * This class is responsible for fire the section in the system 
	 * it should have the Gameobject in the class in order to make the connection 
	 * it also have the connector to websocket server connection inoder to have the event 
	*/

public class deviceManager : MonoBehaviour {

	//public area 
	//the var to store the pointer of different section
	public RedController RedControllerObj;
	public Blue_Controller  BlueControllerObj;
	public YellowController YellowControllerObj;
	public GameObject SectionArea4;



	//save the Socket io element 
	private SocketIOComponent socket;
	//create buffer for detect the event of the hardware
	private ringBuffer section1 = new ringBuffer(5);
	private ringBuffer section2 = new ringBuffer(5);
	private ringBuffer section3 = new ringBuffer(5);

	/*
	 * Ring buffer class to detect the event of thye input hardare device 
	 * each new record should input to the buffer in order to get the rate of changeß
	 */
	public class ringBuffer
	{
		//Define thresouhold of the event
		private double thresholdChange = 4.5;
		public int normalRange = 200;
		private LinkedList<int> buffer = new LinkedList<int>();
		private int noSize = 0;
		private int sumOfBuffer = 0;
		//bool for counting user release the section already
		private bool doUserRelease = true;

		//constructor
		public ringBuffer(int bufferSize)
		{
			this.noSize = bufferSize;
		}

		//new input in to buffer
		public void newInput(int input){
			if (buffer.Count > this.noSize) {
				sumOfBuffer -=buffer.First.Value;
				buffer.RemoveFirst ();
			}
			buffer.AddLast (input);
			sumOfBuffer += input;
		}

		//get the event
		/*
		 * return vluae 		event type
		 * 		-1					OnPress
		 * 		0					Onstay
		 * 		1					OnExit 	
		 * 		999					no event happen
		 */
		public int getEvent()
		{
			//get the average change in the buffer 
			double averageChangeInBuffer = this.getAvergeChange();

			if (Mathf.Abs ((float)averageChangeInBuffer) > thresholdChange) {
				if (averageChangeInBuffer < 0) {
					if(doUserRelease){
						//triggering
						doUserRelease = false;
						Debug.Log("triggering +++++++++++++++++++++++++++++++++++++++");
						return -1;
					}else
						return 0;
				} else {
					//release once
					if(!doUserRelease){
						//releasing 
						//Debug.Log("releasing +*****************************************");
						doUserRelease = true;
						return 1;
					}else
						return 0;
				}
			} else {
				//within normal range +- 15% -> normal
				int change = (normalRange - buffer.Last.Value)/normalRange;
				change = Mathf.Abs(change);
				if(doUserRelease){
					//Debug.Log("no change %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
					return 999;
				}else{
					//Debug.Log("on stay 5555555555555555555555555555555555");
					return 0;
				}
			}
		}

		//get the change of value when the input come 
		public double getAvergeChange()
		{
			LinkedListNode<int> current = buffer.Last;
			LinkedListNode<int> past;
			double changeRate = 0.0000;
			for(int i = 0 ; i < buffer.Count-1; i++){
				past = current.Previous;
				changeRate +=  (double)(current.Value - past.Value)* 100/(double)past.Value ;
			}
			return changeRate/= buffer.Count;
		}

	}

	// the object start
	public void Start() 
	{
		//Websocket section
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		
		socket.On("open", TestOpen);
		socket.On("boop", TestBoop);
		socket.On("error", TestError);
		socket.On("close", TestClose);
		socket.On("data", getData);

		//for testing the connection btw the server and device  
		StartCoroutine("BeepBoop");
	}

	//When the dtat push from the server / hardware
	public void getData(SocketIOEvent e)
	{
		string data = e.data.GetField("sensor").str;
		data = data.Substring(0, data.Length - 2);
		string[] splitedData = data.Split(',');
		int[] dataArray = new int[splitedData.Length];
		for(int i = 0 ; i < splitedData.Length ; i++){
			dataArray[i] = int.Parse(splitedData[i]);
		}

		section1.newInput (dataArray[0]);//144
		section1.normalRange = 144;
		section2.newInput (dataArray[1]);//200
		section2.normalRange = 200;
		section3.newInput (dataArray[2]);//245
		section3.normalRange = 245;
		//double changeValue = section1.getAvergeChange ();

		int Area3State = section3.getEvent ();
		//Debug.Log ("section state: " + Area1State);

		if (Area3State == -1) {
			//Debug.Log("getting in -1");
			RedControllerObj.userPress();
			RedControllerObj.areaNormal = false;
			RedControllerObj.areaPress = true;
		} else if (Area3State == 0) {
			RedControllerObj.areaPress = false;
			RedControllerObj.areaPressStay = true;
		} else if (Area3State == 1) {
			RedControllerObj.areaPressStay = false;
			RedControllerObj.areaPressEnd = true;
			//Debug.Log("getting in 1");
		} else {
			RedControllerObj.areaNormal = true;
			RedControllerObj.areaPressEnd = false;
		}


		switch(section2.getEvent ()){
		case -1:
			BlueControllerObj.areaNormal = false;
			BlueControllerObj.areaPress = true;
			Debug.Log("getting in 22222");
			break;
		case 0:
			BlueControllerObj.areaPress = false;
			BlueControllerObj.areaPressStay = true;
			break;
		case 1:
			BlueControllerObj.areaPressStay = false;
			BlueControllerObj.areaPressEnd = true;
			break;
		case 999:
			BlueControllerObj.areaNormal = true;
			BlueControllerObj.areaPressEnd = false;
			break;
		}
		
		switch(section1.getEvent ()){
		case -1:
			YellowControllerObj.areaNormal = false;
			YellowControllerObj.areaPress = true;
			//Debug.Log("getting in 333333");
			break;
		case 0:
			YellowControllerObj.areaPress = false;
			YellowControllerObj.areaPressStay = true;
			break;
		case 1:
			YellowControllerObj.areaPressStay = false;
			YellowControllerObj.areaPressEnd = true;
			break;
		case 999:
			YellowControllerObj.areaNormal = true;
			YellowControllerObj.areaPressEnd = false;
			break;
		}
		return;
	}

	private IEnumerator BeepBoop()
	{
		// wait 1 seconds and continue
		yield return new WaitForSeconds(1);
		
		socket.Emit("beep");
		
		// wait 3 seconds and continue
		yield return new WaitForSeconds(3);
		
		socket.Emit("beep");
		
		// wait 2 seconds and continue
		yield return new WaitForSeconds(2);
		
		socket.Emit("beep");
		
		// wait ONE FRAME and continue
		yield return null;
		
		socket.Emit("beep");
		socket.Emit("beep");
	}
	
	public void TestOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}
	
	public void TestBoop(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Boop received: " + e.name + " " + e.data);
		
		if (e.data == null) { return; }
		
		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
			);
	}
	
	public void TestError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}
	
	public void TestClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}
}
