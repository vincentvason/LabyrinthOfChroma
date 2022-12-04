using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

using static PlayerStats;

public class EnemyShieldContainer : MonoBehaviour
{   
    [Header("[Set] Player Settings")]
    [SerializeField] private GameObject player;
    
    [Header("[Set] Spline Settings")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject removeContainer;
    [SerializeField] private bool closedSpline;

    [Header("[Set] Orb Type Setting")]
    [SerializeField] private GameObject[] orbTypeList;
    [SerializeField] private GameObject orbBlocker;

    [Header("[Set] Timing Settings")]
    [SerializeField] private float moveOrbPerSecond = 1f;
    [SerializeField] private float crashOrbPerSecond = 3f;
    [SerializeField] private float fixedUpdateStep = 0.02f;
    [SerializeField] private int orbCount;
    
    [Header("[Set] Game Settings")]
    [SerializeField] private int numberOrbMatchNeed = 3;

    [Header("[Stat] Orb Property")]
    [SerializeField] private List<GameObject> orbList;
    [SerializeField] private List<float> orbCurrentPositionList;
    [SerializeField] private List<float> orbDestinationPositionList;
    
    [Header("[Stat] Spline Property")]
    [SerializeField] private float orbDiameterToSpline;
    [SerializeField] private float maximumOrbs;

    [Header("[Stat] State Variable")]
    [SerializeField] private int isOnCollision = 0;
    [SerializeField] private int orbAddedIndex = -1;
    [SerializeField] private int orbCheckIndex = -1;
    [SerializeField] private int orbCheckChainIndex = -1;
    [SerializeField] private int orbSpacingIndex = -1;
    
    // Start is called before the first frame update
    void Start()
    {        
        //Calculated Orb Radius from Collider
        SetDiameter(orbTypeList[0]);
        DOTween.SetTweensCapacity(48825, 1950);
        
        //List of orbs
        orbList = new List<GameObject>();

        //Start Instantiate Orb Set
        SetOrbToPosition();
        
    }

    // Update every frame
    void Update()
    {
        if (orbCount > 0){
            if(orbAddedIndex >= 0){
                PushNewOrbToContainer(orbAddedIndex);
            }
            if(orbCheckIndex >= 0 && orbSpacingIndex < 0 && isOnCollision == 0){
                RemoveMatchOnAdded(orbCheckIndex);
            }
            if(orbSpacingIndex >= 0){
                PushOrbBackTogether();
            }
            if(orbCheckChainIndex >= 0 && orbSpacingIndex < 0 && isOnCollision == 0){
                RemoveMatchOnAddedAfterMatch(orbCheckChainIndex,orbCheckChainIndex+1);
            }
        }
        else{
            /* Do nothing, shield fully deplete */
        }   
    }

    // Update after predetermined duration (1/50 seconds)
    void FixedUpdate()
    {
         //When Move the active section of balls along the path
        if (orbCount > 0){
            MoveOrb();
            if(orbAddedIndex < 0 && orbCheckIndex < 0 && orbCheckChainIndex < 0 && orbSpacingIndex < 0){
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
        maximumOrbs = 1 / orbDiameterToSpline;
        return orbDiameterToSpline;
    }

    void SetOrbToPosition()
    {
        for(int placeNumber = 0; placeNumber < orbCount; placeNumber++){
            float orbSplineRatioPosition = orbDiameterToSpline * placeNumber;
            Vector3 position = spline.EvaluatePosition(orbSplineRatioPosition);
            position.z = 0;

            Vector3 forward = Vector3.Normalize(spline.EvaluateTangent(orbSplineRatioPosition));
            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject toBeAddOrb;
            
            if(placeNumber == 0)
            {
                toBeAddOrb = Instantiate(orbBlocker, position, rotation, container.transform);
            }
            else
            {
                toBeAddOrb = Instantiate(orbTypeList[UnityEngine.Random.Range(0, orbTypeList.Length)], position, rotation, container.transform);
            }
            toBeAddOrb.tag = "Orb_Enemy";

            orbList.Add(toBeAddOrb);
            orbCurrentPositionList.Add(orbSplineRatioPosition);
            orbDestinationPositionList.Add(orbSplineRatioPosition);
        }
    }

    void MoveOrb()
    {
        Sequence mySequence;
        mySequence = DOTween.Sequence();
        mySequence.Pause();

        float maxSpeedAllow = Mathf.Max(moveOrbPerSecond, crashOrbPerSecond);
        float maxDistanceAllow = maxSpeedAllow * orbDiameterToSpline * fixedUpdateStep;

        for(int index = 0; index < orbCount; index++){
            float difference = orbDestinationPositionList[index] - orbCurrentPositionList[index]; 
            orbCurrentPositionList[index] = orbCurrentPositionList[index] + Mathf.Clamp(difference, -maxDistanceAllow, +maxDistanceAllow);
        }

        for(int index = 0; index < orbCount; index++){
            //Calculate new position
            float newPosition;
            
            //Close Spline handle if position is not in range 0..1
            if(closedSpline == true){
                newPosition = orbCurrentPositionList[index] % 1;
            }
            else{
                newPosition = orbCurrentPositionList[index];
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
            mySequence.Insert(0, orbList[index].transform.DOMove(position, 1/50f));
            mySequence.Insert(0, orbList[index].transform.DORotate(rotation.eulerAngles, 1/50f));
        }

        mySequence.Play();
        mySequence.OnComplete(() =>
            {
                if(orbDestinationPositionList.SequenceEqual(orbCurrentPositionList)){
                    isOnCollision = 0;
                }
                else{
                    
                }
            }
        );
    }

    void MoveOrbContainerForward()
    {
        for(int index = 0; index < orbCount; index++){
            orbDestinationPositionList[index] = orbDestinationPositionList[index] + (moveOrbPerSecond * orbDiameterToSpline * Time.deltaTime);
        }
    }

    public void AddNewBall(GameObject orbObject, int touchIndex)
    {
        int addedIndex;
        float newPosition;

        //Get Nearest point to spline of orb container
        NativeSpline native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, orbObject.transform.position, out float3 nearest, out float interpolationRatio);

        if(touchIndex == orbCount - 1)
        {
            addedIndex = touchIndex;
            newPosition = Mathf.Floor(orbCurrentPositionList[touchIndex]) + interpolationRatio;
        }
        else if(interpolationRatio < (orbCurrentPositionList[touchIndex] % 1f))
        {
            addedIndex = touchIndex;
            newPosition = Mathf.Floor(orbCurrentPositionList[touchIndex]) + interpolationRatio;
        }
        else if(interpolationRatio > (orbCurrentPositionList[touchIndex] % 1f))
        {
            addedIndex = touchIndex + 1;
            newPosition = Mathf.Floor(orbCurrentPositionList[touchIndex + 1]) + interpolationRatio;
        }
        else
        {
            int place = UnityEngine.Random.Range(0, 2); //Random 0 or 1
            addedIndex = touchIndex + place;
            newPosition = Mathf.Floor(orbCurrentPositionList[touchIndex + place]) + interpolationRatio;
        }

        if(addedIndex >= orbCount)
        {
            orbList.Add(orbObject);
            orbCurrentPositionList.Add(newPosition);
            orbDestinationPositionList.Add(newPosition);
        }
        else{
            orbList.Insert(addedIndex,orbObject);
            orbCurrentPositionList.Insert(addedIndex,newPosition);
            orbDestinationPositionList.Insert(addedIndex,newPosition);
        }

        orbObject.transform.parent = container.transform;
		orbObject.transform.SetSiblingIndex(addedIndex);
        orbAddedIndex = addedIndex;

        orbCount++;
    }


    void PushNewOrbToContainer(int addIndex)
    {   
        //Get Nearest point to spline of orb container
        NativeSpline native = new NativeSpline(spline.Spline, spline.transform.localToWorldMatrix);
        SplineUtility.GetNearestPoint(native, orbList[addIndex].transform.position, out float3 nearest, out float interpolationRatio);
        Debug.Log("interpolationRatio:" + interpolationRatio);

        float newPosition = interpolationRatio;

        //if it's not index orb_count-1 and 0
        while(addIndex > 0 && addIndex < orbCount-1 && newPosition <= orbCurrentPositionList[addIndex+1])
        {
            if(newPosition >= orbCurrentPositionList[addIndex-1] && newPosition <= orbCurrentPositionList[addIndex+1]){
                break;
            }
            else{
                newPosition = newPosition + 1;
            }
        }

        //if it's index 0
        while(addIndex == 0 && newPosition <= orbCurrentPositionList[addIndex+1])
        {
            if((newPosition >= orbCurrentPositionList[addIndex+1] - orbDiameterToSpline) && newPosition <= orbCurrentPositionList[addIndex+1]){
                break;
            }
            else{
                newPosition = newPosition + 1;
            }
        }

        //if it's index orb_count-1
        while(addIndex == orbCount-1 && (newPosition <= orbCurrentPositionList[addIndex-1] + 1))
        {
            if(newPosition >= orbCurrentPositionList[addIndex-1] && (newPosition <= orbCurrentPositionList[addIndex-1] + orbDiameterToSpline)){
                break;
            }
            else{
                newPosition = newPosition + 1;
            }
        }

        orbCurrentPositionList[addIndex] = newPosition;
        Debug.Log("interpolationRatio+lang:" + newPosition);

        //Update another orb
        for(int index = 0; index < orbCount; index++){
            //Calculate where will place on the spline from 0..1
            orbDestinationPositionList[index] = (orbCurrentPositionList[addIndex] + ((index-addIndex) * orbDiameterToSpline));
        }

        orbCheckIndex = addIndex;
        orbAddedIndex = -1;
    }

    void RemoveMatchOnAdded(int checkIndex)
    {
        int matchToFirst = 0;
        int matchToLast = 0;
        int matchTotal = 0;

        //Delete a opposite orb if over limit
        if(orbCount > (int)maximumOrbs && closedSpline == true){
                int orbToDelete = (checkIndex + (int)maximumOrbs/2) % (int)maximumOrbs;
                Debug.Log("Over orb delete: "+orbToDelete);

                orbList[orbToDelete].SetActive(false);
                orbList[orbToDelete].transform.parent = removeContainer.transform;
                orbList.RemoveAt(orbToDelete);
                orbCurrentPositionList.RemoveAt(orbToDelete);
                orbDestinationPositionList.RemoveAt(orbToDelete);
                orbCount--;
                PushOrbBackTogether();
        }

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
                orbCurrentPositionList.RemoveAt(checkIndex - matchToFirst);
                orbDestinationPositionList.RemoveAt(checkIndex - matchToFirst);
                orbCount--;
            }
            orbCheckIndex = -1;
            orbCheckChainIndex = checkIndex-matchToFirst-1;
            orbSpacingIndex = 1;
            isOnCollision = 1;

            player.GetComponent<PlayerStats>().ScoreAdd(10 * matchTotal);
            player.GetComponent<PlayerStats>().OrbDestroy(matchTotal);
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
                orbCurrentPositionList.RemoveAt(checkIndexBefore - matchToFirst);
                orbDestinationPositionList.RemoveAt(checkIndexBefore - matchToFirst);
                orbCount--;
            }
            orbCheckChainIndex = checkIndexBefore - matchToFirst - 1;
            orbSpacingIndex = 1;
            isOnCollision = 1;

            player.GetComponent<PlayerStats>().ScoreAdd(10 * matchTotal);
            player.GetComponent<PlayerStats>().OrbDestroy(matchTotal);
        }
        else{
            //Do nothing
            orbCheckChainIndex = -1;
            orbSpacingIndex = -1;
        }
    }

    void PushOrbBackTogether(){
        for(int index = 0; index < orbCount; index++){
            orbDestinationPositionList[index] = orbDestinationPositionList[0] + (orbDiameterToSpline * index);
        }
        orbSpacingIndex = -1;
    }
}



