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
    [SerializeField] private PlayerStats player;
    
    [Header("[Set] Spline Settings")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject removeContainer;
    [SerializeField] private bool closedSpline;

    [Header("[Set] Orb Type Setting")]
    [SerializeField] private GameObject[] orbTypeList;
    [SerializeField] private int orbTypeListCount;
    [SerializeField] private GameObject orbSpecial;
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
    [SerializeField] private List<int> orbGroupList;
    
    [Header("[Stat] Spline Property")]
    [SerializeField] private float orbDiameterToSpline;
    [SerializeField] private float maximumOrbs;

    [Header("[Stat] State Variable")]
    [SerializeField] private int isOnCollision = 0;
    [SerializeField] private int orbAddedIndex = -1;
    [SerializeField] private int orbCheckIndex = -1;
    [SerializeField] private int orbCheckChainIndex = -1;
    [SerializeField] private int orbJoinOnMatchIndex = -1;

    [Header("[Stat] State Variable (Open Spline Only)")]
    [SerializeField] private bool isReachLimit = false;
    
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
        if (orbCount > 0)
        {
            if(orbAddedIndex >= 0)
            {
                PushNewOrbToContainer(orbAddedIndex);
            }
            if(orbCheckIndex >= 0 && orbJoinOnMatchIndex < 0 && isOnCollision == 0)
            {
                RemoveMatchOnAdded(orbCheckIndex);
            }
            if(orbJoinOnMatchIndex >= 0)
            {
                JoinOrbGroupOnMatch(orbJoinOnMatchIndex);
            }
            if(orbCheckChainIndex >= 0 && orbJoinOnMatchIndex < 0 && isOnCollision == 0)
            {
                RemoveMatchOnAddAfterMatch(orbCheckChainIndex,orbCheckChainIndex+1);
            }
        }
        else
        {
            /* Do nothing, shield fully deplete */
        }   
    }

    // Update after predetermined duration (1/50 seconds)
    void FixedUpdate()
    {
         //When Move the active section of balls along the path
        if (orbCount > 0)
        {
            if(closedSpline == false)
            {
                MoveOrbToPositive();
            }

            MoveOrb();
            
            if(orbAddedIndex < 0 && orbCheckIndex < 0 && orbCheckChainIndex < 0 && orbJoinOnMatchIndex < 0)
            {
                MoveOrbContainerForward();
            }
        }
        else
        {
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
            
            if(placeNumber == 0 && closedSpline == true)
            {
                toBeAddOrb = Instantiate(orbBlocker, position, rotation, container.transform);
            }
            else
            {
                toBeAddOrb = Instantiate(orbTypeList[UnityEngine.Random.Range(0, orbTypeListCount)], position, rotation, container.transform);
            }
            toBeAddOrb.tag = "Orb_Enemy";

            orbList.Add(toBeAddOrb);
            orbCurrentPositionList.Add(orbSplineRatioPosition);
            orbDestinationPositionList.Add(orbSplineRatioPosition);
            orbGroupList.Add(1);
        }
    }

    void MoveOrbToPositive()
    {
        if(orbCurrentPositionList[0] < 0.0f)
        {
            float distance = - orbCurrentPositionList[0];

            for(int index = 0; index < orbCount; index++)
            {
                orbDestinationPositionList[index] = orbDestinationPositionList[index] + distance;
            }
        }
        else
        {
            /* Do nothing */
        }
    }

    void MoveOrb()
    {
        Sequence mySequence;
        mySequence = DOTween.Sequence();
        mySequence.Pause();

        float maxSpeedAllow = Mathf.Max(moveOrbPerSecond, crashOrbPerSecond);
        float maxDistanceAllow = maxSpeedAllow * orbDiameterToSpline * fixedUpdateStep;

        for(int index = 0; index < orbCount; index++)
        {
            float difference = orbDestinationPositionList[index] - orbCurrentPositionList[index]; 
            orbCurrentPositionList[index] = orbCurrentPositionList[index] + Mathf.Clamp(difference, -maxDistanceAllow, +maxDistanceAllow);
        }

        for(int index = 0; index < orbCount; index++){
            //Calculate new position
            float newPosition;
            
            //Close Spline handle if position is not in range 0..1
            if(closedSpline == true)
            {
                newPosition = orbCurrentPositionList[index] % 1;
            }
            else
            {
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
                if(orbDestinationPositionList.SequenceEqual(orbCurrentPositionList))
                {
                    isOnCollision = 0;
                }
                else
                {
                    /* Still wait for on complete */
                }
            }
        );
    }

    void MoveOrbContainerForward()
    {
        for(int index = 0; index < orbCount; index++)
        {
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
            orbGroupList.Add(orbGroupList[touchIndex]);
        }
        else
        {
            orbList.Insert(addedIndex,orbObject);
            orbCurrentPositionList.Insert(addedIndex,newPosition);
            orbDestinationPositionList.Insert(addedIndex,newPosition);
            orbGroupList.Insert(addedIndex,orbGroupList[touchIndex]);
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

        //if it's not index orbCount-1 and 0
        while(addIndex > 0 && addIndex < orbCount-1 && newPosition <= orbCurrentPositionList[addIndex+1])
        {
            if(newPosition >= orbCurrentPositionList[addIndex-1] && newPosition <= orbCurrentPositionList[addIndex+1])
            {
                break;
            }
            else
            {
                newPosition = newPosition + 1;
            }
        }

        //if it's index 0
        while(addIndex == 0 && newPosition <= orbCurrentPositionList[addIndex+1])
        {
            if((newPosition >= orbCurrentPositionList[addIndex+1] - orbDiameterToSpline) && newPosition <= orbCurrentPositionList[addIndex+1])
            {
                break;
            }
            else
            {
                newPosition = newPosition + 1;
            }
        }

        //if it's index orbCount-1
        while(addIndex == orbCount-1 && (newPosition <= orbCurrentPositionList[addIndex-1] + 1))
        {
            if(newPosition >= orbCurrentPositionList[addIndex-1] && (newPosition <= orbCurrentPositionList[addIndex-1] + orbDiameterToSpline))
            {
                break;
            }
            else
            {
                newPosition = newPosition + 1;
            }
        }

        orbCurrentPositionList[addIndex] = newPosition;
        Debug.Log("interpolationRatio+lang:" + newPosition);


        JoinOrbGroupOnAdd(addIndex);
/*
        //Update another orb
        for(int index = 0; index < orbCount; index++)
        {
            //Calculate where will place on the spline from 0..1
            orbDestinationPositionList[index] = (orbCurrentPositionList[addIndex] + ((index-addIndex) * orbDiameterToSpline));
        }
*/
        orbCheckIndex = addIndex;
        orbAddedIndex = -1;
    }

    void RemoveMatchOnAdded(int checkIndex)
    {
        int matchToFirst = 0;
        int matchToLast = 0;
        int matchTotal = 0;

        //Delete if orb over limit
        if(closedSpline == true)
        {
            if(orbCount > (int)maximumOrbs)
            {
                //Delete on the orb opposite side
                int orbToDelete = (checkIndex + (int)maximumOrbs/2) % (int)maximumOrbs;
                //If orb is black, delete orb next to them.
                if(orbToDelete == 0)
                {
                    orbToDelete = 1;
                }
                else
                {
                    //OK
                }
                Debug.Log("Over orb delete: "+orbToDelete);

                orbList[orbToDelete].SetActive(false);
                orbList[orbToDelete].transform.parent = removeContainer.transform;
                orbList.RemoveAt(orbToDelete);
                orbCurrentPositionList.RemoveAt(orbToDelete);
                orbDestinationPositionList.RemoveAt(orbToDelete);
                orbGroupList.RemoveAt(orbToDelete);
                orbCount--;
                PushOrbBackTogetherAfterReachMax();
            }
        }
        else
        {
            for(int index = 0; index < orbCount; index++)
            {
                //Delete orb that over 1
                if(orbDestinationPositionList[index] >= 1.0f)
                {
                    Debug.Log("Over orb delete: "+index);

                    orbList[index].SetActive(false);
                    orbList[index].transform.parent = removeContainer.transform;
                    orbList.RemoveAt(index);
                    orbCurrentPositionList.RemoveAt(index);
                    orbDestinationPositionList.RemoveAt(index);
                    orbGroupList.RemoveAt(index);
                    orbCount--;
                    isReachLimit = true;
                } 
            }           
        }

        // Check from current to orbCount
        if(checkIndex < orbCount)
        {
            for(int current = checkIndex + 1; current < orbCount; current++)
            {
                if(orbList[checkIndex].name == orbList[current].name)
                {
                    matchToLast++;
                }
                else
                {
                    break;
                }
            }
        }
        else{
            //No orbs after orbCount
        }

        // Check from current to zero
        if(checkIndex > 0)
        {
            for(int current = checkIndex - 1; current >= 0; current--)
            {
                if(orbList[checkIndex].name == orbList[current].name)
                {
                    matchToFirst++;
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            //No orbs before zero
        }

        Debug.Log("checkIndex: "+checkIndex+" matchToFirst: "+matchToFirst+" matchToLast: "+matchToLast);

        matchTotal = matchToFirst + matchToLast + 1;

        if(matchTotal >= numberOrbMatchNeed)
        {
            for(int i = 0; i < matchTotal; i++)
            {
                orbList[checkIndex - matchToFirst].SetActive(false);
                orbList[checkIndex - matchToFirst].transform.parent = removeContainer.transform;
                orbList.RemoveAt(checkIndex - matchToFirst);
                orbCurrentPositionList.RemoveAt(checkIndex - matchToFirst);
                orbDestinationPositionList.RemoveAt(checkIndex - matchToFirst);
                orbGroupList.RemoveAt(checkIndex - matchToFirst);
                orbCount--;
            }
            orbCheckIndex = -1;
            orbCheckChainIndex = checkIndex-matchToFirst-1;
            orbJoinOnMatchIndex = checkIndex-matchToFirst;
            isOnCollision = 1;

            player.GetComponent<PlayerStats>().ScoreAdd(10 * matchTotal);
            player.GetComponent<PlayerStats>().OrbDestroy(matchTotal);
        }
        else
        {
            //Do nothing
            orbCheckIndex = -1;
            orbCheckChainIndex = -1;
        }
    }

    void RemoveMatchOnAddAfterMatch(int checkIndexBefore, int checkIndexAfter)
    {
        int matchToFirst = 0;
        int matchToLast = 0;
        int matchTotal = 0;

        if(checkIndexBefore < orbCount && checkIndexAfter < orbCount)
        {
            // Check if First and Last is a same
            if(orbList[checkIndexBefore].name == orbList[checkIndexAfter].name)
            {
                // Check from current to orbCount
                if(checkIndexAfter < orbCount)
                {
                    for(int current = checkIndexAfter + 1; current < orbCount; current++)
                    {
                        if(orbList[checkIndexAfter].name == orbList[current].name)
                        {
                            matchToLast++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //No orbs after orbCount
                }

                // Check from current to zero
                if(checkIndexBefore > 0)
                {
                    for(int current = checkIndexBefore - 1; current >= 0; current--)
                    {
                        if(orbList[checkIndexBefore].name == orbList[current].name)
                        {
                            matchToFirst++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //No orbs before zero
                }

                Debug.Log("checkIndexBefore: "+checkIndexBefore+"checkIndexAfter: "+checkIndexAfter+" matchToFirst: "+matchToFirst+" matchToLast: "+matchToLast);
                matchTotal = matchToFirst + matchToLast + 2;
            }
            else
            {
                Debug.Log("checkIndexBefore, checkIndexAfter: no match");
                matchTotal = 0;
            }
        }
        else
        {
            Debug.Log("checkIndexBefore, checkIndexAfter: no place");
            matchTotal = 0;
        }

        

        if(matchTotal >= numberOrbMatchNeed)
        {
            for(int i = 0; i < matchTotal; i++)
            {
                orbList[checkIndexBefore - matchToFirst].SetActive(false);
                orbList[checkIndexBefore - matchToFirst].transform.parent = removeContainer.transform;
                orbList.RemoveAt(checkIndexBefore - matchToFirst);
                orbCurrentPositionList.RemoveAt(checkIndexBefore - matchToFirst);
                orbDestinationPositionList.RemoveAt(checkIndexBefore - matchToFirst);
                orbGroupList.RemoveAt(checkIndexBefore - matchToFirst);
                orbCount--;
            }
            orbCheckChainIndex = checkIndexBefore - matchToFirst - 1;
            orbJoinOnMatchIndex = checkIndexBefore - matchToFirst;
            isOnCollision = 1;

            player.GetComponent<PlayerStats>().ScoreAdd(10 * matchTotal);
            player.GetComponent<PlayerStats>().OrbDestroy(matchTotal);
        }
        else
        {
            //Do nothing
            orbCheckChainIndex = -1;
        }
    }

    void JoinOrbGroupOnAdd(int joinOnAddedIndex)
    {
        // find first one and last one        
        int targetGroup = orbGroupList[joinOnAddedIndex];
        int firstInGroup = -99;
        int lastInGroup = -99;

        for(int index = 0; index < orbCount; index++)
        {
            if(orbGroupList[index] == targetGroup && firstInGroup < 0)
            {
                firstInGroup = index;
            }

            if(orbGroupList[index] != targetGroup && firstInGroup >= 0 && lastInGroup < 0)
            {
                lastInGroup = index;
                break;
            }
        }

        if(lastInGroup == -99)
        {
            lastInGroup = orbCount;
        }

        Debug.Log("AddinGroup Group:"+targetGroup+" / firstInGroup: "+firstInGroup+" / lastInGroup: "+lastInGroup);

        // arrange only first one and last one
        for(int index = firstInGroup; index < lastInGroup; index++)
        {
            orbDestinationPositionList[index] = orbDestinationPositionList[firstInGroup] + (orbDiameterToSpline * (index-firstInGroup));
        }

        // check if group can joined by matching the previous order
        if(joinOnAddedIndex - 1 >= 0)
        {
            if((orbList[joinOnAddedIndex].name == orbList[joinOnAddedIndex-1].name) && (orbGroupList[joinOnAddedIndex] != orbGroupList[joinOnAddedIndex-1]))
            {
                Debug.Log("JoinInGroupOnAdded (Prev) Group:"+targetGroup+" / list: "+orbList[joinOnAddedIndex].name+" / list-1: "+orbList[joinOnAddedIndex-1].name);
                // change group orb to the previous one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbGroupList[index] = orbGroupList[joinOnAddedIndex-1];
                }

                // find first one and last one
                targetGroup = orbGroupList[joinOnAddedIndex-1];
                firstInGroup = -99;
                lastInGroup = -99;

                for(int index = 0; index < orbCount; index++)
                {
                    if(orbGroupList[index] == targetGroup && firstInGroup < 0)
                    {
                        firstInGroup = index;
                    }

                    if(orbGroupList[index] != targetGroup && firstInGroup >= 0 && lastInGroup < 0)
                    {
                        lastInGroup = index;
                        break;
                    }
                }

                if(lastInGroup == -99)
                {
                    lastInGroup = orbCount;
                }                

                // arrange only first one and last one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbDestinationPositionList[index] = orbDestinationPositionList[firstInGroup] + (orbDiameterToSpline * (index-firstInGroup));
                }
            }
        }


        // check if group can joined by matching the next order
        if(joinOnAddedIndex + 1 < orbCount)
        {
            if((orbList[joinOnAddedIndex].name == orbList[joinOnAddedIndex+1].name) && (orbGroupList[joinOnAddedIndex] != orbGroupList[joinOnAddedIndex+1]))
            {
                Debug.Log("JoinInGroupOnAdded (Next) Group:"+targetGroup+" / list: "+orbList[joinOnAddedIndex].name+" / list+1: "+orbList[joinOnAddedIndex+1].name);
                // change group orb to the previous one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbGroupList[index] = orbGroupList[joinOnAddedIndex+1];
                }

                // find first one and last one
                targetGroup = orbGroupList[joinOnAddedIndex+1];
                firstInGroup = -99;
                lastInGroup = -99;

                for(int index = 0; index < orbCount; index++)
                {
                    if(orbGroupList[index] == targetGroup && firstInGroup < 0)
                    {
                        firstInGroup = index;
                    }

                    if(orbGroupList[index] != targetGroup && firstInGroup >= 0 && lastInGroup < 0)
                    {
                        lastInGroup = index;
                        break;
                    }
                }

                if(lastInGroup == -99)
                {
                    lastInGroup = orbCount;
                }

                // arrange only first one and last one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbDestinationPositionList[index] = orbDestinationPositionList[firstInGroup] + (orbDiameterToSpline * (index-firstInGroup));
                }
            }
        }

        //check if it's overlapped to other group by on first in group
        if(firstInGroup > 0){
            if(orbDestinationPositionList[firstInGroup] - orbDestinationPositionList[firstInGroup-1] < orbDiameterToSpline*0.75)
            {
                Debug.Log("JoinInGroupOnAdded (Prev/Overlap) Group:"+targetGroup+" / list: "+orbList[joinOnAddedIndex].name+" / list-1: "+orbList[joinOnAddedIndex-1].name);
                // change group orb to the previous one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbGroupList[index] = orbGroupList[joinOnAddedIndex-1];
                }

                // find first one and last one
                targetGroup = orbGroupList[joinOnAddedIndex-1];
                firstInGroup = -99;
                lastInGroup = -99;

                for(int index = 0; index < orbCount; index++)
                {
                    if(orbGroupList[index] == targetGroup && firstInGroup < 0)
                    {
                        firstInGroup = index;
                    }

                    if(orbGroupList[index] != targetGroup && firstInGroup >= 0 && lastInGroup < 0)
                    {
                        lastInGroup = index;
                        break;
                    }
                }

                if(lastInGroup == -99)
                {
                    lastInGroup = orbCount;
                }

                // arrange only first one and last one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbDestinationPositionList[index] = orbDestinationPositionList[firstInGroup] + (orbDiameterToSpline * (index-firstInGroup));
                }
            }
        }

        //check if it's overlapped to other group by on last in group
        if(lastInGroup + 1 < orbCount)
        {
            if(orbDestinationPositionList[lastInGroup+1] - orbDestinationPositionList[lastInGroup] < orbDiameterToSpline*0.75)
            {
                Debug.Log("JoinInGroupOnAdded (Next/Overlap) Group:"+targetGroup+" / list: "+orbList[joinOnAddedIndex].name+" / list+1: "+orbList[joinOnAddedIndex+1].name);
                // change group orb to the next one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbGroupList[index] = orbGroupList[joinOnAddedIndex+1];
                }

                // find first one and last one
                targetGroup = orbGroupList[joinOnAddedIndex+1];
                firstInGroup = -99;
                lastInGroup = -99;

                for(int index = 0; index < orbCount; index++)
                {
                    if(orbGroupList[index] == targetGroup && firstInGroup < 0)
                    {
                        firstInGroup = index;
                    }

                    if(orbGroupList[index] != targetGroup && firstInGroup >= 0 && lastInGroup < 0)
                    {
                        lastInGroup = index;
                        break;
                    }
                }

                if(lastInGroup == -99)
                {
                    lastInGroup = orbCount;
                }

                // arrange only first one and last one
                for(int index = firstInGroup; index < lastInGroup; index++)
                {
                    orbDestinationPositionList[index] = orbDestinationPositionList[firstInGroup] + (orbDiameterToSpline * (index-firstInGroup));
                }
            }
        }

    }

    void JoinOrbGroupOnMatch(int joinOnMatchIndex)
    {
        if(joinOnMatchIndex > 0 && joinOnMatchIndex < orbCount)
        {
            Debug.Log("JoinOrbGroupOnMatch"+joinOnMatchIndex+":"+orbList[joinOnMatchIndex].name+"/Prev"+orbList[joinOnMatchIndex-1].name);
            if(orbList[joinOnMatchIndex].name == orbList[joinOnMatchIndex-1].name)
            {
                int oldGroup1 = orbGroupList[joinOnMatchIndex-1];

                for(int index = joinOnMatchIndex; index < orbCount; index++)
                {
                    orbDestinationPositionList[index] = orbDestinationPositionList[0] + (orbDiameterToSpline * index);
                }
                for(int index = joinOnMatchIndex; index < orbCount; index++)
                {
                    orbGroupList[index] = oldGroup1;
                }
            }
            else
            {
                int newGroupNumber = orbGroupList.Max() + 1;
                for(int index = joinOnMatchIndex; index < orbCount; index++)
                {
                    orbGroupList[index] = newGroupNumber;
                }
            }
        }

        orbJoinOnMatchIndex = -1;
    }

    void PushOrbBackTogetherAfterReachMax()
    {
        for(int index = 0; index < orbCount; index++)
        {
            orbDestinationPositionList[index] = orbDestinationPositionList[0] + (orbDiameterToSpline * index);
        }
    }
}



