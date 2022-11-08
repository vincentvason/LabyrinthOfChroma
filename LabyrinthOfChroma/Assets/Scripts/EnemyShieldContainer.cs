using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class EnemyShieldContainer : MonoBehaviour
{   
    public SplineContainer spline;
    public bool closedSpline;
    public GameObject[] orbTypeList;
    public float moveOrbPerSecond = 1f;
    public float collisionDuration = 0.45f;
    public int orbCount;
    public int numberOrbMatchNeed = 3;
    
    float orbRadius;

    public Ease ease;

    public List<GameObject> orbList;
    public List<float> orbPositionList;

    public float orbDiameterToSpline;
    public float maximumOrbs;

    public int orbAddedIndex = -1;
    public int orbCheckIndex = -1;
    public int orbSpacingIndex = -1;
    

    // Start is called before the first frame update
    void Start()
    {        
        //Calculated Orb Radius from Collider
        SetDiameter(orbTypeList[0]);
        
        //List of orbs
        orbList = new List<GameObject>();

        //Start Instantiate Orb Set
        for(int i = 0; i < orbCount; i++){
            //Calculate where will place on the spline from 0..1
            SetOrbToPosition(orbTypeList[UnityEngine.Random.Range(0, orbTypeList.Length)], i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //When Move the active section of balls along the path
        if (orbCount > 0){
            if(orbAddedIndex >= 0){
                PushNewOrbToContainer(orbAddedIndex);
            }
            if(orbCheckIndex >= 0 && orbSpacingIndex < 0){
                RemoveMatchOnAdded(orbCheckIndex);
            }
            if(orbSpacingIndex >= 0){
                PushOrbBackTogether();
            }
            if(orbCheckIndex == -1 && orbAddedIndex == -1 && orbSpacingIndex == -1){
                MoveOrbContainerForward();
            }
        }
        else{
            /* Do nothing, shield fully deplete */
        }
    }

    float SetDiameter(GameObject thisGameObject)
    {
        float orbRadius = thisGameObject.GetComponent<CircleCollider2D>().radius;
        float splineLength = spline.CalculateLength();
        float orbSplineRatio = ((orbRadius*2))/splineLength;
        orbDiameterToSpline = orbSplineRatio;
        maximumOrbs = 1 / orbSplineRatio;
        return orbSplineRatio;
    }

    float SetOrbToPosition(GameObject thisGameObject, int placeNumber)
    {
        float orbSplineRatioPosition = orbDiameterToSpline * placeNumber;
        Vector3 position = spline.EvaluatePosition(orbSplineRatioPosition);
        position.z = 0;

        Vector3 forward = Vector3.Normalize(spline.EvaluateTangent(orbSplineRatioPosition));
        float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject toBeAddOrb;
        
        toBeAddOrb = Instantiate(thisGameObject, position, rotation, gameObject.transform);
        toBeAddOrb.tag = "Orb_Enemy";

        orbList.Add(toBeAddOrb);
        orbPositionList.Add(orbSplineRatioPosition);
        return orbSplineRatioPosition;
    }

    void MoveOrbContainerForward()
    {
        //Update distance
        for(int count = 0; count < orbCount; count++){
            //Calculate where will place on the spline from 0..1
            float newPosition = (orbPositionList[count] + (orbDiameterToSpline * (moveOrbPerSecond/100f))) % 1f;

            //Random and add orb to the set
            //Position Wise
            Vector3 position = spline.EvaluatePosition(newPosition);
            position.z = 0;
                
            //Rotation Wise (SplineAnimate.cs @ EvaluatePositionAndRotation, UpdateTransform)
            Vector3 forward = Vector3.Normalize(spline.EvaluateTangent(newPosition));
            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //Move
            orbList[count].transform.DOMove(position, 1f/100f);
            orbList[count].transform.rotation = rotation;
            orbPositionList[count] = newPosition;
        }
    }

    public void AddNewBall(GameObject orbObject, int touchIndex)
    {
        //Get Nearest point to spline of orb container
        NativeSpline native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, orbObject.transform.position, out float3 nearest, out float interpolationRatio);

        float orbPositionNew = interpolationRatio;

        //Only in closed spline
        if(closedSpline == true && Mathf.Abs(orbPositionList[touchIndex] - orbPositionNew) > orbDiameterToSpline){
            if(orbPositionList[touchIndex] > orbPositionNew){
                orbPositionNew = 1 + orbPositionNew;
            }
            else if(orbPositionList[touchIndex] < orbPositionNew){
                orbPositionNew = 1 - orbPositionNew;
            }
        }
        else{
            //No need to wrap from finish to beginning.
        }

        if(interpolationRatio < orbPositionList[touchIndex]){
            orbAddedIndex = touchIndex;
        }
        else if(interpolationRatio > orbPositionList[touchIndex]){
            orbAddedIndex = touchIndex + 1;
        }
        else{
            orbAddedIndex = touchIndex + UnityEngine.Random.Range(0, 2); //Random 0 or 1
        }

        Debug.Log("Orb Added Index:" +orbAddedIndex);

        if(orbAddedIndex >= orbCount){
            orbList.Add(orbObject);
            orbPositionList.Add(interpolationRatio);
        }
        else{
            orbList.Insert(orbAddedIndex,orbObject);
            orbPositionList.Insert(orbAddedIndex,interpolationRatio);
        }

        orbObject.transform.parent = gameObject.transform;
		orbObject.transform.SetSiblingIndex(orbAddedIndex);

        orbCount++;
    }


    void PushNewOrbToContainer(int addIndex)
    {
        int onComplete = 0;

        //Get Nearest point to spline of orb container
        NativeSpline native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, orbList[addIndex].transform.position, out float3 nearest, out float interpolationRatio);

        //Move New orb to container
        orbList[addIndex].transform.DOMove(nearest, collisionDuration).SetEase(ease);   
        orbPositionList[addIndex] = interpolationRatio;

        //Update another orb
        for(int count = 0; count < orbCount; count++){
            //Calculate where will place on the spline from 0..1
            float newPosition = (orbPositionList[addIndex] + ((count-addIndex) * orbDiameterToSpline * (moveOrbPerSecond))) % 1f;

            //Close Spline handle if position is not in range 0..1
            if(closedSpline == true){
                if(newPosition < 0){
                    newPosition = newPosition + 1;
                }
                else if(newPosition > 1){
                    newPosition = newPosition - 1;
                }
                else{
                    //already in 0..1
                }
            }
            else{
                //No need to wrap from finish to beginning.
            }

            //Random and add orb to the set
            //Position Wise
            Vector3 position = spline.EvaluatePosition(newPosition);
            position.z = 0;
                
            //Rotation Wise (SplineAnimate.cs @ EvaluatePositionAndRotation, UpdateTransform)
            Vector3 forward = Vector3.Normalize(spline.EvaluateTangent(newPosition));
            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //Move
            orbList[count].transform.rotation = rotation;
            orbPositionList[count] = newPosition;
            //If this is the last iteration, check on compete. else just move
            orbList[count].transform.DOMove(position, collisionDuration).SetLoops(1).SetEase(ease).OnComplete(() =>
                {
                    onComplete++;    
                    Debug.Log("onCompete: "+onComplete);
                    if(onComplete >= orbCount){
                        orbAddedIndex = -1;
                        orbCheckIndex = addIndex;
                    }  
                }
            );            
        }
    }

void RemoveMatchOnAdded(int checkIndex)
    {
        int startPointer;
        int currentPointer = 0;
        int checkPointer = 0;
        int matchOrbNumber = 0;

        Debug.Log("RemoveMatchOnAdded");

        if(checkIndex - numberOrbMatchNeed < 0){
            startPointer = 0;
        }
        else{
            startPointer = checkIndex - numberOrbMatchNeed;
        }

        //Pointer for current
        for(currentPointer = startPointer; currentPointer < (checkIndex + numberOrbMatchNeed) && currentPointer < orbCount; currentPointer++){
            //Pointer for check
            for(checkPointer = currentPointer; checkPointer < orbCount; checkPointer++){
                Debug.Log("CurrentPointer:"+currentPointer+"CheckPointer:"+checkPointer);
                //If matched, Added and continue, else, stop
                if(orbList[currentPointer].name == orbList[checkPointer].name){
                    matchOrbNumber++;
                }
                else{
                    break;
                }
            }
            //If already reach matched stop, if not continue with reset match orb number
            if(matchOrbNumber >= numberOrbMatchNeed){
                break;
            }
            else{
                matchOrbNumber = 0;
            }
        }

        Debug.Log("CurrentPointer:"+currentPointer+"CheckPointer:"+checkPointer+"Matched:"+matchOrbNumber);

        if(matchOrbNumber >= numberOrbMatchNeed){
            for(int i = 0; i < orbCount; i++){
                Debug.Log("Object["+i+"]:"+orbList[i].name);
            }
            for(int count = currentPointer; count < currentPointer + matchOrbNumber && currentPointer < orbCount; count++){
                Debug.Log("DeleteOrb:"+count);
                Destroy(orbList[currentPointer]);
                orbList.RemoveAt(currentPointer);
                orbPositionList.RemoveAt(currentPointer);
                orbCount--;
            }
            orbCheckIndex = checkIndex - 1;
            orbSpacingIndex = 1;
        }
        else{
            //Do nothing
            orbCheckIndex = -1;
            orbSpacingIndex = -1;
        }
    }

    void PushOrbBackTogether(){
        int onComplete = 0;
        
        //Update another orb
        for(int count = 1; count < orbCount; count++){
            //Calculate where will place on the spline from 0..1
            float newPosition = orbPositionList[0] + (count * orbDiameterToSpline) % 1f;

            //Close Spline handle if position is not in range 0..1
            if(closedSpline == true){
                if(newPosition < 0){
                    newPosition = newPosition + 1;
                }
                else if(newPosition > 1){
                    newPosition = newPosition - 1;
                }
                else{
                    //already in 0..1
                }
            }
            else{
                //No need to wrap from finish to beginning.
            }
    
            //Random and add orb to the set
            //Position Wise
            Vector3 position = spline.EvaluatePosition(newPosition);
            position.z = 0;
                
            //Rotation Wise (SplineAnimate.cs @ EvaluatePositionAndRotation, UpdateTransform)
            Vector3 forward = Vector3.Normalize(spline.EvaluateTangent(newPosition));
            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //Move
            orbList[count].transform.rotation = rotation;
            orbPositionList[count] = newPosition;
            orbList[count].transform.DOMove(position, 1f).SetEase(ease).OnComplete(() =>
                {
                    onComplete++;    
                    Debug.Log("onCompete(Spacing): "+onComplete);
                    if(onComplete >= orbCount - 1){
                        orbSpacingIndex = -1;
                    }  
                }
            );        
        }
    }
}



