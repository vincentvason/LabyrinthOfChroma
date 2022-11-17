using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class EnemyShieldContainer : MonoBehaviour
{   

    public SplineContainer spline;
    public GameObject container;
    public GameObject removeContainer;

    public bool closedSpline;
    public GameObject[] orbTypeList;
    public float moveOrbPerSecond = 1f;
    public float collisionDuration = 0.3f;
    public int orbCount;
    public float orbSpace = 0.01f;
    public int numberOrbMatchNeed = 3;
    

    float orbRadius;

    public Ease ease;

    public List<GameObject> orbList;
    public List<float> orbPositionList;

    public float orbDiameterToSpline;
    public float maximumOrbs;

    public int isMoving = 0;
    public int orbAddedIndex = -1;
    public int orbCheckIndex = -1;
    public int orbCheckChainIndex = -1;
    public int orbSpacingIndex = -1;
    
    // Start is called before the first frame update
    void Start()
    {        
        //Calculated Orb Radius from Collider
        SetDiameter(orbTypeList[0]);
        DOTween.SetTweensCapacity(48825, 1950);
        
        //List of orbs
        orbList = new List<GameObject>();

        //Start Instantiate Orb Set
        for(int i = 0; i < orbCount; i++){
            //Calculate where will place on the spline from 0..1
            SetOrbToPosition(orbTypeList[UnityEngine.Random.Range(0, orbTypeList.Length)], i);
        }
    }

    // Update after predetermined duration (1/50 seconds)
    void FixedUpdate()
    {
         //When Move the active section of balls along the path
        if (orbCount > 0){
            if(orbAddedIndex >= 0){
                PushNewOrbToContainer(orbAddedIndex);
            }
            if(orbCheckIndex >= 0 && orbSpacingIndex < 0){
                RemoveMatchOnAdded(orbCheckIndex);
            }
            if(orbCheckChainIndex >= 0 && orbSpacingIndex < 0){
                RemoveMatchOnAddedAfterMatch(orbCheckChainIndex,orbCheckChainIndex+1);
            }
            if(orbSpacingIndex >= 0){
                PushOrbBackTogether();
            }
            if(orbAddedIndex < 0 && orbCheckIndex < 0 && orbCheckChainIndex < 0 && orbSpacingIndex < 0 ){
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
        orbDiameterToSpline = orbSplineRatio+orbSpace;
        maximumOrbs = 1 / orbDiameterToSpline;
        return orbDiameterToSpline;
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
        
        toBeAddOrb = Instantiate(thisGameObject, position, rotation, container.transform);
        toBeAddOrb.tag = "Orb_Enemy";

        orbList.Add(toBeAddOrb);
        orbPositionList.Add(orbSplineRatioPosition);
        return orbSplineRatioPosition;
    }

    void MoveOrbContainerForward()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Pause();

        //Update distance
        for(int count = 0; count < orbCount; count++){
            //Calculate where will place on the spline from 0..1
            float newPosition = (orbPositionList[count] + (orbDiameterToSpline * (moveOrbPerSecond/50f)));

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
            mySequence.Insert(0, orbList[count].transform.DOLocalMove(position, 1/50f));
            mySequence.Insert(0, orbList[count].transform.DOLocalRotate(rotation.eulerAngles, 1/50f));
            orbPositionList[count] = newPosition;
        }

        isMoving = 1;
        mySequence.Play();
        mySequence.OnComplete(() =>
            {
                isMoving = 0;
            }
        );

    }

    public void AddNewBall(GameObject orbObject, int touchIndex)
    {
        int addedIndex;

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
            addedIndex = touchIndex;
        }
        else if(interpolationRatio > orbPositionList[touchIndex]){
            addedIndex = touchIndex + 1;
        }
        else{
            addedIndex = touchIndex + UnityEngine.Random.Range(0, 2); //Random 0 or 1
        }

        if(addedIndex >= orbCount){
            orbList.Add(orbObject);
            orbPositionList.Add(interpolationRatio);
        }
        else{
            orbList.Insert(addedIndex,orbObject);
            orbPositionList.Insert(addedIndex,interpolationRatio);
        }

        orbObject.transform.parent = container.transform;
		orbObject.transform.SetSiblingIndex(addedIndex);
        orbAddedIndex = addedIndex;

        orbCount++;
    }


    void PushNewOrbToContainer(int addIndex)
    {   
        Sequence mySequence = DOTween.Sequence();
        mySequence.Pause();

        //Get Nearest point to spline of orb container
        NativeSpline native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, orbList[addIndex].transform.position, out float3 nearest, out float interpolationRatio);

        //Move New orb to container
        mySequence.Insert(0, orbList[addIndex].transform.DOLocalMove(nearest, collisionDuration).SetEase(ease));
        orbPositionList[addIndex] = interpolationRatio;

        //Update another orb
        for(int count = 0; count < orbCount; count++){
            //Calculate where will place on the spline from 0..1
            float newPosition = (orbPositionList[addIndex] + ((count-addIndex) * orbDiameterToSpline)) % 1f;

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
            mySequence.Join(orbList[count].transform.DOLocalRotate(rotation.eulerAngles, collisionDuration));
            mySequence.Join(orbList[count].transform.DOLocalMove(position, collisionDuration).SetLoops(1).SetEase(ease));
            orbPositionList[count] = newPosition;
            //If this is the last iteration, check on compete. else just move
        }

        isMoving = 1;
        mySequence.Play();
        mySequence.OnComplete(() =>
            {
                orbCheckIndex = orbAddedIndex;
                orbAddedIndex = -1;
                Debug.Log("PushNewOrbToContainer: Complete");
                isMoving = 0;
            }
        );  
    }

    void RemoveMatchOnAdded(int checkIndex)
    {
        int matchToFirst = 0;
        int matchToLast = 0;
        int matchTotal = 0;

        // Check from current to orbCount
        if(checkIndex < orbCount){
            for(int current = checkIndex + 1; current < orbCount; current++){
                if(orbList[checkIndex].name == orbList[current].name){
                    matchToLast++;
                }
                else{
                    break;
                }
            }
        }
        else{
            //No orbs after orbCount
        }

        // Check from current to zero
        if(checkIndex > 0){
            for(int current = checkIndex - 1; current >= 0; current--){
                if(orbList[checkIndex].name == orbList[current].name){
                    matchToFirst++;
                }
                else{
                    break;
                }
            }
        }
        else{
            //No orbs before zero
        }

        Debug.Log("checkIndex: "+checkIndex+" matchToFirst: "+matchToFirst+" matchToLast: "+matchToLast);

        matchTotal = matchToFirst + matchToLast + 1;

        if(matchTotal >= numberOrbMatchNeed){
            for(int i = 0; i < matchTotal; i++){
                orbList[checkIndex - matchToFirst].SetActive(false);
                orbList[checkIndex - matchToFirst].transform.parent = removeContainer.transform;
                orbList.RemoveAt(checkIndex - matchToFirst);
                orbPositionList.RemoveAt(checkIndex - matchToFirst);
                orbCount--;
            }
            orbCheckIndex = -1;
            orbCheckChainIndex = checkIndex - matchToFirst - 1;
            orbSpacingIndex = 1;
        }
        else{
            //Do nothing
            orbCheckIndex = -1;
            orbCheckChainIndex = -1;
            orbSpacingIndex = -1;
        }
    }

    void RemoveMatchOnAddedAfterMatch(int checkIndexBefore, int checkIndexAfter)
    {
        int matchToFirst = 0;
        int matchToLast = 0;
        int matchTotal = 0;

        if(checkIndexBefore < orbCount && checkIndexAfter < orbCount){
            // Check if First and Last is a same
            if(orbList[checkIndexBefore].name == orbList[checkIndexAfter].name){
                // Check from current to orbCount
                if(checkIndexAfter < orbCount){
                    for(int current = checkIndexAfter + 1; current < orbCount; current++){
                        if(orbList[checkIndexAfter].name == orbList[current].name){
                            matchToLast++;
                        }
                        else{
                            break;
                        }
                    }
                }
                else{
                    //No orbs after orbCount
                }

                // Check from current to zero
                if(checkIndexBefore > 0){
                    for(int current = checkIndexBefore - 1; current >= 0; current--){
                        if(orbList[checkIndexBefore].name == orbList[current].name){
                            matchToFirst++;
                        }
                        else{
                            break;
                        }
                    }
                }
                else{
                    //No orbs before zero
                }

                Debug.Log("checkIndexBefore: "+checkIndexBefore+"checkIndexAfter: "+checkIndexAfter+" matchToFirst: "+matchToFirst+" matchToLast: "+matchToLast);
                matchTotal = matchToFirst + matchToLast + 2;
            }
            else{
                Debug.Log("checkIndexBefore, checkIndexAfter: no match");
                matchTotal = 0;
            }
        }
        else{
            Debug.Log("checkIndexBefore, checkIndexAfter: no place");
            matchTotal = 0;
        }

        

        if(matchTotal >= numberOrbMatchNeed){
            for(int i = 0; i < matchTotal; i++){
                orbList[checkIndexBefore - matchToFirst].SetActive(false);
                orbList[checkIndexBefore - matchToFirst].transform.parent = removeContainer.transform;
                orbList.RemoveAt(checkIndexBefore - matchToFirst);
                orbPositionList.RemoveAt(checkIndexBefore - matchToFirst);
                orbCount--;
            }
            orbCheckChainIndex = checkIndexBefore - matchToFirst - 1;
            orbSpacingIndex = 1;
        }
        else{
            //Do nothing
            orbCheckChainIndex = -1;
            orbSpacingIndex = -1;
        }
    }

    void PushOrbBackTogether(){
        //no need to push if count < 1
        if(orbCount <= 1){
            orbSpacingIndex = -1;
            return;
        }

        Sequence mySequence = DOTween.Sequence();
        mySequence.Pause();

        float updatePosition;
        float[] oldPosition = new float[orbCount];
        float[] newPosition = new float[orbCount];

        for(int count = 0; count < orbCount; count++){
            oldPosition[count] = orbPositionList[count];
            newPosition[count] = oldPosition[0] + (count * orbDiameterToSpline);

            if(closedSpline == true && oldPosition[count] < newPosition[count]){
                oldPosition[count] = oldPosition[count] + 1;
            }
            else{
                //No need to wrap from finish to beginning.
            }
        }

        //Update another orb
        Debug.Log("newPosition@0 at "+orbPositionList[0]);
        for(int count = 0; count < orbCount; count++){
            //Calculate where will place on the spline from 0..1
            for(int i = 0; i < 50; i++){
                updatePosition = oldPosition[count] + ((newPosition[count]-oldPosition[count]) * (i/50f));
                Debug.Log("updatePosition@"+count+"f"+i+" at "+updatePosition);
                //Close Spline handle if position is not in range 0..1
                if(closedSpline == true){
                    if(updatePosition < 0){
                        updatePosition = updatePosition + 1;
                    }
                    else if(updatePosition > 1){
                        updatePosition = updatePosition - 1;
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
                Vector3 position = spline.EvaluatePosition(updatePosition);
                position.z = 0;
                    
                //Rotation Wise (SplineAnimate.cs @ EvaluatePositionAndRotation, UpdateTransform)
                Vector3 forward = Vector3.Normalize(spline.EvaluateTangent(updatePosition));
                float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                //Move
                mySequence.Insert((i*collisionDuration)/50, orbList[count].transform.DOLocalRotate(rotation.eulerAngles, collisionDuration/50));
                mySequence.Insert((i*collisionDuration)/50, orbList[count].transform.DOLocalMove(position, collisionDuration/50));
                orbPositionList[count] = updatePosition;
            }
        }

        isMoving = 1;
        mySequence.Play();
        mySequence.OnComplete(() =>
            {
                orbSpacingIndex = -1;
                isMoving = 0;
            }
        );  
    }
}



