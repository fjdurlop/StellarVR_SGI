//-----------------------------------------------------------------------
// <copyright file="ObjectController.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using System.Threading;

/// <summary>
/// Controls target objects behaviour.
/// </summary>
public class ObjectController : MonoBehaviour
{
    /// <summary>
    /// The material to use when this object is inactive (not being gazed at).
    /// </summary>
    public Material InactiveMaterial;

    /// <summary>
    /// The material to use when this object is active (gazed at).
    /// </summary>
    public Material GazedAtMaterial;

    // The objects are about 1 meter in radius, so the min/max target distance are
    // set so that the objects are always within the room (which is about 5 meters
    // across).
    private const float _minObjectDistance = 0.1f;
    private const float _maxObjectDistance = 1000.0f;
    private const float _minObjectHeight = 0.1f;
    private const float _maxObjectHeight = 200.0f;
    private Vector3 generalCamPosition = new Vector3(0.0f, 200.0f, 0.0f);

    private Renderer _myRenderer;
    private Vector3 _startingPosition;
    private float timer;
    private float timer_middle;

    private bool inSight;
    private Transform current;
    private Transform cam;
    private bool activeTracking;

    private Material originalMaterial;
    private Material highlightMaterial;
    private Color originalColor;
    private Color newColor;

    private string[] planetsArray = new string[] { "Sun_GEO", "Mercury_GEO", 
        "Venus_GEO", "Earth_GEO","Mars_GEO","Jupiter_GEO","Saturn_GEO",
        "Uranus_GEO","Pluto_GEO" };
    
    private int counterPlanet = 0;

    private bool allow_selection = false;
    private bool middle_entered = false;
    private bool block_change =false;
    private bool rotate_planet =false;
    private bool menu_activated =false;



    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        _startingPosition = transform.parent.localPosition;
        _myRenderer = GetComponent<Renderer>();
        SetMaterial(false);
        inSight = false;
        current = this.transform;
        cam = GameObject.Find("Player").transform;
        cam.position = generalCamPosition;
        activeTracking = false;
        //originalColor = _myRenderer.material.color;
        newColor = originalColor;
        menu_activated = true;

    }

    /// <summary>
    /// Teleports this instance randomly when triggered by a pointer click.
    /// </summary>
    public void Update()
    {
        timer += Time.deltaTime;
        if(inSight && timer > 2.0f)
        {
            selectingButton();
            Singleton.instance.setPlanet(current);
            activeTracking = Singleton.instance.isSun();
            //movingCameraToObjective();
        }
        if (activeTracking) bringMenuPlayer();
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter()
    {
        timer = 0.0f;
        inSight = true;
        SetMaterial(true);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
        resetFocus();
        SetMaterial(false);

        _myRenderer = GetComponent<Renderer>();
        _myRenderer.material.color = originalColor;
    }

    /// <summary>
    /// This method is called by the Main Camera when it is gazing at this GameObject and the screen
    /// is touched.
    /// </summary>
    public void OnPointerClick()
    {

    }

    /// <summary>
    /// Sets this instance's material according to gazedAt status.
    /// </summary>
    ///
    /// <param name="gazedAt">
    /// Value `true` if this object is being gazed at, `false` otherwise.
    /// </param>
    private void SetMaterial(bool gazedAt)
    {
        if (InactiveMaterial != null && GazedAtMaterial != null)
        {
            _myRenderer.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
        }
    }
    private void selectingButton()
    {
        // Get the name of the GameObject this script is attached to
        string objectName = gameObject.name;
        string type = "";
        // Log the object name to the console
        //Debug.Log("Object Name: " + objectName);
        GameObject selectedObject = GameObject.Find(objectName);
        Collider collider = selectedObject.GetComponent<Collider>();

        Renderer myRenderer = GetComponent<Renderer>();
        
        newColor.a = 2.0f;
        myRenderer.material.color = newColor;
        //myRenderer.material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1));

        if (collider != null)
        {
            type = "button";
            Debug.Log("This GameObject has a Collider.");
        }
        else
        {
            Debug.Log("planet is11"+selectedObject.name);

            type = "planet";
            Debug.Log("This GameObject does not have a Collider.");
        }

        if (selectedObject.CompareTag("planet")){
            type = "planet";
            Debug.Log("planet is11"+selectedObject.name);

        }
        else if(selectedObject.CompareTag("buttons")){
            type = "button";
        }

        if(type == "button"){
            // distinguish objects by tags
            if (selectedObject.GetComponent<Collider>().CompareTag("buttons")){
                Debug.Log("button!");
                
                if(objectName == "button_middle"){
                    timer_middle += Time.deltaTime;
                    if (!middle_entered){
                        timer_middle = 0.0f;
                        allow_selection = true;
                    }
                    middle_entered = true;

                    if (allow_selection){ //update after middle_pressed is true
                        Debug.Log("Num planets"+ planetsArray.Length.ToString());
                        if(counterPlanet < 9){
                            counterPlanet+=1;
                        }else{
                            counterPlanet = 0;
                        }

                        timer_middle= 0.0f;
                        Debug.Log("Counter "+ counterPlanet.ToString());
                        //GameObject selectedPlanet =null ; //default takes sun

                        GameObject selectedPlanet  = GameObject.Find(planetsArray[counterPlanet]);
                        Debug.Log("Planet selected: "+ selectedPlanet.name);
                        changeSphere(selectedPlanet);
                    }
                    if (timer_middle<3){
                        allow_selection =false;
                    } else {
                        allow_selection =true;
                    }
                    
                }
                else if(objectName == "button_right_up"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    //textMeshPro = GetComponent<TextMeshPro>();
                    //textMenu.text = selectedPlanet.name;

                    Debug.Log("CounterPlanet "+ counterPlanet.ToString());
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"rotate", "right");
                }
                else if(objectName == "button_left_up"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    //textMeshPro = GetComponent<TextMeshPro>();
                    //textMenu.text = selectedPlanet.name;

                    Debug.Log("CounterPlanet "+ counterPlanet.ToString());
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_left_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"rotate", "left");
                }
                else if(objectName == "button_right"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"scale", "up");
                }
                else if(objectName == "button_left"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"scale", "down");
                }
                else if(objectName == "button_right_down"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"translate", "forward");
                }
                else if(objectName == "button_left_down"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"translate", "backward");
                }
                else if(objectName == "button_middle_2"){
                    
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    //textMeshPro = GetComponent<TextMeshPro>();
                    //textMenu.text = selectedPlanet.name;

                    Debug.Log("CounterPlanet "+ counterPlanet.ToString());
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);

                    movePlanet(selectedPlanet,"rotate", "right");

                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    
                }
                else if(objectName == "button_left_up_2"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    //textMeshPro = GetComponent<TextMeshPro>();
                    //textMenu.text = selectedPlanet.name;

                    Debug.Log("CounterPlanet "+ counterPlanet.ToString());
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_left_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"rotate", "left");
                }
                else if(objectName == "button_right_2"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"scale", "up");
                }
                else if(objectName == "button_left_2"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"scale", "down");
                }
                else if(objectName == "button_right_down_2"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"translate", "forward");
                }
                else if(objectName == "button_left_down_2"){
                    TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
                    GameObject selectedPlanet  = GameObject.Find(textMenu.text);
                    Debug.Log("button_right_up: "+ selectedPlanet.name);
                    movePlanet(selectedPlanet,"translate", "backward");
                }
                else if(objectName == "button_exit"){
                    Vector3 targetPosition = new Vector3(10000f, 10000f, -10000f); // Adjust the target position as needed
                    float moveSpeed = 100f; // Adjust the movement speed as needed
                    GameObject menuPlayer = GameObject.Find("Menu_play");

                    menuPlayer.transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                    
                }

                

            }
        }
        else if(type == "planet"){
            Debug.Log("planet is11"+selectedObject.name);

            if (selectedObject.CompareTag("planet")){
                Debug.Log("planet is"+selectedObject.name);
                Debug.Log("planet is"+objectName);

                if(objectName == "Sun_GEO"){
                    bringMenuPlayer();
                    
                }
                
            }
        }
        
    }

    public void resetFocus()
    {
        timer = 0.0f;
        inSight = false;
    }

    public void changeSphere(GameObject selectedPlanet){
        
        //selectedPlanet = GameObject.Find(planetsArray[counterPlanet]);
        Debug.Log("Planet selected: "+ selectedPlanet.name);


        GameObject sphereMenu = GameObject.Find("Sphere_menu");
        TMPro.TextMeshPro textMenu = GameObject.Find("texto_menu").GetComponent<TMPro.TextMeshPro>();
        if (menu_activated){
            TMPro.TextMeshPro textMenu2 = GameObject.Find("texto_menu_2").GetComponent<TMPro.TextMeshPro>();
            textMenu2.text = selectedPlanet.name;
        }
        

        //textMeshPro = GetComponent<TextMeshPro>();
        textMenu.text = selectedPlanet.name;

        sphereMenu.GetComponent<Renderer>().material = selectedPlanet.GetComponent<Renderer>().material;
        //Vector3 originalScale = selectedPlanet.transform.localScale;

    }

    public void movePlanet(GameObject selectedPlanet, string action, string direction){
        
        //selectedPlanet = GameObject.Find(planetsArray[counterPlanet]);
        //Debug.Log("movePlanet name: "+ selectedPlanet.name);

        // sfera
        GameObject sphereMenu = GameObject.Find("Sphere_menu");
        
        //textMeshPro = GetComponent<TextMeshPro>();
        
        //sphereMenu.GetComponent<Renderer>().material = selectedPlanet.GetComponent<Renderer>().material;
        //Vector3 originalScale = selectedPlanet.transform.localScale;

        if(action =="rotate"){
            // Rotate the planet based on the direction
            //Debug.Log("movePlanet name: "+ selectedPlanet.name);

            float rotationAmount = 1f; // Adjust this value as needed
            Vector3 rotationVector = Vector3.zero;

            switch (direction)
            {
                case "left":
                    rotationVector = Vector3.up;
                    break;
                case "right":
                    rotationVector = Vector3.down;
                    break;
                // Add more cases for other directions if needed
            }

            selectedPlanet.transform.Rotate(rotationVector * rotationAmount);
            sphereMenu.transform.Rotate(rotationVector * rotationAmount);
        }
        else if(action =="scale"){
            // Scale the planet based on the direction
            float scaleFactor = 0.01f; // Adjust this value as needed

            switch (direction)
            {
                case "up":
                    selectedPlanet.transform.localScale += Vector3.one * scaleFactor;
                    break;
                case "down":
                    selectedPlanet.transform.localScale -= Vector3.one * scaleFactor;
                    break;
                // Add more cases for other directions if needed
            }
        }
        else if(action =="translate"){
            // Translate the planet based on the direction
            float translationAmount = 0.01f; // Adjust this value as needed

            switch (direction)
            {
                case "forward":
                    selectedPlanet.transform.Translate(Vector3.forward * translationAmount);
                    break;
                case "backward":
                    selectedPlanet.transform.Translate(Vector3.back * translationAmount);
                    break;
                // Add more cases for other directions if needed
            }
        }

    }

    public void bringMenuPlayer(){
        //GameObject sphereMenu = GameObject.Find("Sphere_menu");
        //Transform cam = GameObject.Find("Player").transform;
        //Debug.Log("moving menu");
        //public Transform playerTransform;
        Transform playerTransform = GameObject.Find("Player").transform;
        GameObject menu = GameObject.Find("Menu_play");
     

        Transform menuTransform = menu.transform;
        Transform cameraTransform = GameObject.Find("Main Camera").transform;

        
        Vector3 relativePosition = new Vector3(-0.85f, 4.74f, -2.71f); // Adjust the relative position as needed

        float movementSpeed = 5f; // Adjust the movement speed as needed
        bool hasRotated = false;
        // Check if the player transform is assigned
        if (playerTransform != null)
        {
            // Calculate the target position relative to the player
            Vector3 targetPosition = playerTransform.position + relativePosition;
            //Vector3 targetRotate = playerTransform.rotation + relativeRotation;
            Quaternion relativeRotation = Quaternion.Inverse(cameraTransform.rotation) * transform.rotation;
            if(!hasRotated){
                //menuTransform.Rotate(relativeRotation.eulerAngles * Time.deltaTime);
                hasRotated = true;

                menuTransform.LookAt(playerTransform);
                //Quaternion _lookRotation = Quaternion.LookRotation((cameraTransform.position ).normalized);
	
                //over time
                //menuTransform.rotation = Quaternion.Slerp(menuTransform.rotation, _lookRotation, Time.deltaTime * turn_speed);
                
                //instant
                //menuTransform.rotation = _lookRotation;
                menuTransform.Rotate(Vector3.up, 270f);
            }
            
            // Move the object towards the target position
            menuTransform.position = Vector3.Lerp(menuTransform.position, targetPosition, Time.deltaTime * movementSpeed);
            //menuTransform.LookAt(cameraTransform);

        }
        else
        {
            Debug.LogError("Player transform not assigned. Please assign the player transform in the Inspector.");
        }
    }

}
