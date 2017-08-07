﻿/*
 * Programmer: Bryan Cancel
 * Last Updated: 8/3/17
 * 
 * FUSE -vs- OVERLAP
 *      *This Refers to object A and object B that have their own respective outlines
 *      FUSE -> by DEFAULT outline A will "Interact" with outline B... 
 *          so it seems like object A and object B are merging into 1 sprite
 *      OVERLAP -> IF obj A and obj B have oultines with Masks that affect different layers
 *          (and ofcourse the outlines are being clipped within their own perspective range)
 *          outline A will overlap outline B [If (outline A orderInLayer) > (outline B orderInLayer)]
 *          
 * NOTE: 
 * [IF] someone wants to be our parent -AND- they are not already our child -> (Let Them be your parent) 
 * [ELSE] (dont let them because you are their parent)
 * 
 * NOTE: Duplication of the Object in (Edit -or- Play) Mode will create the object but with all the DEFAULT outline settings
 * 
 * NOTE: this Object "Executes In Edit Mode" For the Sole purpose of chossing your settings
 *          HOWEVER... when you are actually using the asset you MUST change the variables from code
 *          
 *          REASON... this is because this object is [NOT SERIALIZABLE]... so unity will clone it and use its copy in game mode
 *          REASON ITS NOT ADDED... its alot more trouble that its worth and can cause significant problems if not used properly
 *          
 *          I created the inspector Script for each outline type to help easily make changes without the need for your own code while in both modes
 *          
 * NOTE: children must not have an inspector helper script... or they will not follow their parent properly     
 * 
 * NOTE: 
 * [IF] someone wants to be our parent -AND- they are not already our child -> (Let Them be your parent) 
 * [ELSE] (dont let them because you are their parent)
 *
 * Currently we only pass (1) Sprite Overlay, AND, (2) Basic Outline Data (3) Clipping Mask [on/off] to our children
 * NOTE: use code sniplet below in variable set area to pass that variable value to your children
 *          Keep in Mind the Concave and Convex Outline Types dont share all the variables
 *          I Considered most of the settings they dont share sprite specific
    
    for (int i = 0; i < children.Count; i++)
    {
        if (children[i] != null)
        {
            if (children[i].GetComponent<concaveOutline>() != null)
                children[i].GetComponent<concaveOutline>().Active_SO = active_SO;
            if (children[i].GetComponent<convexOutline>() != null)
                children[i].GetComponent<convexOutline>().Active_SO = active_SO;
        }
        else
        {
            children.RemoveAt(i);
            i--;
        }
    }
 * 
 * NOTE: for the outline to work properly with an animation, you must set the "Animator" "Update Mode" to "Animate Physics"
 * 
 * LIMITATION 1: since I am using the sprite to create an outline... if the sprite SOURCE is semi transparent then the outline then the overlay will also be semi transparent
 *               in areas where the outline is semitransparent the opacities might no be the same
 * SOLUTION 1: use shader that grabs the silhouette of the sprite as a solid color regardless of semi transparency and use that... (I wasn't able to find said shader... and I dont know HLSL)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace objOutlines
{
    [ExecuteInEditMode]
    public class concaveOutline : MonoBehaviour
    {
        //-----parent child reltionship

        [Header("Family Relationships")]
        public GameObject parentGOWithScript;
        GameObject prevParentGOWithScript;

        public List<GameObject> children; //NOTE: children MUST NOT have an inspector helper script... or they will not follow their parent properly

        //--- Optimization

        bool updateSpriteEveryFrame;
        public bool UpdateSpriteEveryFrame
        {
            get { return updateSpriteEveryFrame; }
            set { updateSpriteEveryFrame = value; }
        }

        //-----Variables for Used in Awake----- (currently NONE)

        //-----Debugging Variables-----

        GameObject outlineGameObjectsFolder; //contains all the outlines
                                             //IMPORTANT NOTE: currently only one outline is supported

        bool showOutline_GOs_InHierarchy_D;
        public bool ShowOutline_GOs_InHierarchy_D
        {
            get { return showOutline_GOs_InHierarchy_D; }
            set
            {
                showOutline_GOs_InHierarchy_D = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().showOutline_GOs_InHierarchy_D = showOutline_GOs_InHierarchy_D;

                if (showOutline_GOs_InHierarchy_D)
                    outlineGameObjectsFolder.hideFlags = HideFlags.None;
                else
                    outlineGameObjectsFolder.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        //-----Overlay Variables-----

        GameObject spriteOverlay;

        bool active_SO;
        public bool Active_SO
        {
            get { return active_SO; }
            set
            {
                active_SO = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().active_SO = active_SO;

                spriteOverlay.SetActive(active_SO);

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if(children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().Active_SO = active_SO;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().Active_SO = active_SO;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                } 
            }
        }

        int orderInLayer_SO;
        public int OrderInLayer_SO
        {
            get { return orderInLayer_SO; }
            set
            {
                orderInLayer_SO = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().orderInLayer_SO = orderInLayer_SO;

                spriteOverlay.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_SO;

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().OrderInLayer_SO = orderInLayer_SO;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().OrderInLayer_SO = orderInLayer_SO;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        Color color_SO;
        public Color Color_SO
        {
            get { return color_SO; }
            set
            {
                color_SO = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().color_SO = color_SO;

                spriteOverlay.GetComponent<SpriteRenderer>().color = color_SO;

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().Color_SO = color_SO;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().Color_SO = color_SO;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        //-----Clipping Mask Variables

        GameObject clippingMask; //gameobject with sprite mask

        bool clipCenter_CM;
        public bool ClipCenter_CM
        {
            get { return clipCenter_CM; }
            set
            {
                clipCenter_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().clipCenter_CM = clipCenter_CM;

                //enable or disable mask
                clippingMask.GetComponent<SpriteMask>().enabled = clipCenter_CM;

                //update how our edge gameobjects interact with the mask
                if (clipCenter_CM == true)
                {
                    if (edges_1 != null)
                        foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                            dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    if (edges_1 != null)
                        foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                            dictVal.Key.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
                }

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().ClipCenter_CM = clipCenter_CM;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().ClipCenter_CM = clipCenter_CM;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        } //NOTE: used in update function... doesnt have to do anyting special for get and set...

        [Range(0, 1)]
        float alphaCutoff_CM;
        public float AlphaCutoff_CM
        {
            get { return alphaCutoff_CM; }
            set
            {
                alphaCutoff_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().alphaCutoff_CM = alphaCutoff_CM;

                clippingMask.GetComponent<SpriteMask>().alphaCutoff = alphaCutoff_CM;
            }
        }

        bool customRange_CM;
        public bool CustomRange_CM
        {
            get { return customRange_CM; }
            set
            {
                customRange_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().customRange_CM = customRange_CM;

                clippingMask.GetComponent<SpriteMask>().isCustomRangeActive = customRange_CM;
            }
        }

        int frontLayer_CM;
        public int FrontLayer_CM
        {
            get { return frontLayer_CM; }
            set
            {
                frontLayer_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().frontLayer_CM = frontLayer_CM;

                clippingMask.GetComponent<SpriteMask>().frontSortingLayerID = frontLayer_CM;
            }
        }

        int backLayer_CM;
        public int BackLayer_CM
        {
            get { return backLayer_CM; }
            set
            {
                backLayer_CM = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().backLayer_CM = backLayer_CM;

                clippingMask.GetComponent<SpriteMask>().backSortingLayerID = backLayer_CM;
            }
        }

        //-----Outline Variables-----

        GameObject Outline;

        bool active_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool Active_O
        {
            get { return active_O; }
            set
            {
                active_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().active_O = active_O;

                //all edges set active
                if (edges_1 != null)
                    foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
                        entry.Key.SetActive(active_O);

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().Active_O = active_O;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().Active_O = active_O;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        [Space(10)]

        Color color_O;
        public Color Color_O
        {
            get { return color_O; }
            set
            {
                color_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().color_O = color_O;

                //update our edges with the new color
                if (edges_1 != null)
                    foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                        dictVal.Key.GetComponent<SpriteRenderer>().color = color_O;

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().Color_O = color_O;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().Color_O = color_O;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        int orderInLayer_O;
        public int OrderInLayer_O
        {
            get { return orderInLayer_O; }
            set
            {
                orderInLayer_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().orderInLayer_O = orderInLayer_O;

                //update our edges with the new color
                if (edges_1 != null)
                    foreach (KeyValuePair<GameObject, Vector2> dictVal in edges_1)
                        dictVal.Key.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().OrderInLayer_O = orderInLayer_O;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().OrderInLayer_O = orderInLayer_O;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        float size_O; //NOTE: this size refers to the world space thickness of the outline
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public float Size_O
        {
            get { return size_O; }
            set
            {
                float oldSize = size_O;

                value = (value >= 0) ? value : 1;
                size_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().size_O = size_O;

                UpdatepositionsOfEdges();

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<convexOutline>() != null) //different outline type
                            children[i].GetComponent<convexOutline>().Size_O = (oldSize * size_O) / children[i].GetComponent<convexOutline>().Size_O;
                        else if (children[i].GetComponent<concaveOutline>() != null) //same outline type
                            children[i].GetComponent<concaveOutline>().Size_O = size_O;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        bool scaleWithParentX_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool ScaleWithParentX_O
        {
            get { return scaleWithParentX_O; }
            set
            {
                scaleWithParentX_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().scaleWithParentX_O = scaleWithParentX_O;

                UpdatepositionsOfEdges();

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().ScaleWithParentX_O = scaleWithParentX_O;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().ScaleWithParentX_O = scaleWithParentX_O;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        bool scaleWithParentY_O;
        //NOTE: used in update function... doesnt have to do anyting special for get and set...
        public bool ScaleWithParentY_O
        {
            get { return scaleWithParentY_O; }
            set
            {
                scaleWithParentY_O = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().scaleWithParentY_O = scaleWithParentY_O;

                UpdatepositionsOfEdges();

                //TODO... reconfigure to work with any of our 6 scripts
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] != null)
                    {
                        if (children[i].GetComponent<concaveOutline>() != null)
                            children[i].GetComponent<concaveOutline>().ScaleWithParentY_O = scaleWithParentY_O;
                        if (children[i].GetComponent<convexOutline>() != null)
                            children[i].GetComponent<convexOutline>().ScaleWithParentY_O = scaleWithParentY_O;
                    }
                    else
                    {
                        children.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        //-----Push Type Variables-----

        Dictionary<GameObject, Vector2> edges_1;

        bool pushType_Regular_or_Custom_OP;
        public bool PushType_Regular_or_Custom_OP
        {
            get { return pushType_Regular_or_Custom_OP; }
            set
            {
                pushType_Regular_or_Custom_OP = value; //update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().pushType_Regular_or_Custom_OP = pushType_Regular_or_Custom_OP;

                if (pushType_Regular_or_Custom_OP) //f -> t (CLEAR custom oultine data)
                {
                    destroyEdgesThatCreateOutline();

                    makeSureWeHaveCorrectNumberOfEdges();

                    updateNormalEdgeVectors(); //NOTE: this also updates the positions ofthe edges
                }
                else //t -> f (KEEP regular outline data)
                    UpdatepositionsOfEdges();
            }
        }

        //---Regular

        int objsMakingOutline_OPR; //also the count of gameobjects that make up the outline
        public int ObjsMakingOutline_OPR
        {
            get { return objsMakingOutline_OPR; }
            set
            {
                objsMakingOutline_OPR = (value >= 0) ? value : 0;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().objsMakingOutline_OPR = objsMakingOutline_OPR;

                makeSureWeHaveCorrectNumberOfEdges();

                updateNormalEdgeVectors();
            }
        }

        float startAngle_OPR;
        public float StartAngle_OPR
        {
            get { return startAngle_OPR; }
            set
            {
                startAngle_OPR = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().startAngle_OPR = startAngle_OPR;

                UpdatepositionsOfEdges();
            }
        }

        bool radialPush_OPR; //push objs to edge of circle or to edge of box
        public bool RadialPush_OPR
        {
            get { return radialPush_OPR; }
            set
            {
                radialPush_OPR = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().radialPush_OPR = radialPush_OPR;

                UpdatepositionsOfEdges();
            }
        }

        //---Custom

        bool stdSize_OPC;
        public bool StdSize_OPC
        {
            get { return stdSize_OPC; }
            set
            {
                stdSize_OPC = value;//update local value
                if (gameObject.GetComponent<inspectorForConcaveOutline>() != null)//update inspector value
                    gameObject.GetComponent<inspectorForConcaveOutline>().stdSize_OPC = stdSize_OPC;

                UpdatepositionsOfEdges();
            }
        }

        void Awake()
        {
            //--- Cover Duplication Problem

            Transform psblOF_T = this.transform.Find("Outline Folder");
            if (psblOF_T != null) //transform found
            {
                GameObject psblOF_GO = psblOF_T.gameObject;
                if (psblOF_GO.transform.parent.gameObject == gameObject) //this gameobject ours
                    DestroyImmediate(psblOF_GO);
            }

            //--- Object Instantiation

            //Outline Folder [MUST BE FIRST]
            outlineGameObjectsFolder = new GameObject("Outline Folder");
            copyOriginalGO_Transforms(outlineGameObjectsFolder);
            outlineGameObjectsFolder.transform.parent = this.transform;

            //Outline Around Sprite Object Folder
            Outline = new GameObject("Outline Around Sprite Folder");
            copyOriginalGO_Transforms(Outline);
            Outline.transform.parent = outlineGameObjectsFolder.transform;

            //Sprite Overlay
            spriteOverlay = new GameObject("Sprite Overlay");
            spriteOverlay.AddComponent<SpriteRenderer>();
            //---
            var tempMaterial = new Material(spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial);
            tempMaterial.shader = Shader.Find("GUI/Text Shader");
            spriteOverlay.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;
            //---
            copyOriginalGO_Transforms(spriteOverlay);
            spriteOverlay.transform.parent = outlineGameObjectsFolder.transform;
            copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

            //Sprite Mask
            clippingMask = new GameObject("Sprite Mask");
            clippingMask.AddComponent<SpriteMask>();
            copyOriginalGO_Transforms(clippingMask);
            clippingMask.transform.parent = outlineGameObjectsFolder.transform;
            clippingMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;

            //---Children
            children = new List<GameObject>();
            if (parentGOWithScript != null && children.Contains(parentGOWithScript) == false) //if someone wants to be our parent... and they are not already our child...
            {
                if (parentGOWithScript.GetComponent<concaveOutline>() != null)
                {
                    if (parentGOWithScript.GetComponent<concaveOutline>().children.Contains(this.gameObject) == false)
                        parentGOWithScript.GetComponent<concaveOutline>().children.Add(this.gameObject);
                }
                else if (parentGOWithScript.GetComponent<convexOutline>() != null)
                {
                    if (parentGOWithScript.GetComponent<convexOutline>().children.Contains(this.gameObject) == false)
                        parentGOWithScript.GetComponent<convexOutline>().children.Add(this.gameObject);
                }
            }
            else
                parentGOWithScript = null;

            //*****Set Variable Defaults*****

            //--- Optimization

            UpdateSpriteEveryFrame = true;

            //----- Debugging

            ShowOutline_GOs_InHierarchy_D = false;

            //--- Sprite Overlay

            Active_SO = false;
            OrderInLayer_SO = this.GetComponent<SpriteRenderer>().sortingOrder + 1; //by default in front
            Color_SO = new Color(0, 0, 1, .5f);

            //--- Clipping Mask

            ClipCenter_CM = true;
            AlphaCutoff_CM = .25f;

            CustomRange_CM = false;
            FrontLayer_CM = 0; //by defaults maps to "default" layer
            BackLayer_CM = 0; //by defaults maps to "default" layer

            //----- Outline

            Active_O = true; //NOTE: to hide the outline temporarily use: (1)color -or- (2)size

            Color_O = Color.red;
            OrderInLayer_O = this.GetComponent<SpriteRenderer>().sortingOrder - 1; //by default behind
            Size_O = 2f;
            ScaleWithParentX_O = false;
            ScaleWithParentY_O = false;

            //--- Push Type

            edges_1 = new Dictionary<GameObject, Vector2>();

            PushType_Regular_or_Custom_OP = true;

            //regular

            ObjsMakingOutline_OPR = 8;
            StartAngle_OPR = 0;
            RadialPush_OPR = false;

            //custom

            StdSize_OPC = false;
        }

        void Update()
        {
            //---Sprite Updating

            if (UpdateSpriteEveryFrame)
            {
                //update sprite overlay
                copySpriteRendererData(this.GetComponent<SpriteRenderer>(), spriteOverlay.GetComponent<SpriteRenderer>());

                //update clipping mask
                clippingMask.GetComponent<SpriteMask>().sprite = this.GetComponent<SpriteRenderer>().sprite;

                if (edges_1 != null)
                    foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
                        copySpriteRendererData(this.GetComponent<SpriteRenderer>(), entry.Key.GetComponent<SpriteRenderer>());
            }

            //---Clear Children we no longer need

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == null)
                {
                    children.RemoveAt(i);
                    i--;
                }
            }

            //---parent child relationship

            if (parentGOWithScript != prevParentGOWithScript) //if we change parents
            {
                //TODO... reconfigure to work with any of our 6 scripts
                if (prevParentGOWithScript != null) //If we had a parent... break all ties with them
                {
                    if (parentGOWithScript.GetComponent<convexOutline>() != null)
                    {
                        if (prevParentGOWithScript.GetComponent<convexOutline>().children.Contains(this.gameObject) == true)
                            prevParentGOWithScript.GetComponent<convexOutline>().children.Remove(this.gameObject);
                    }
                    else if (parentGOWithScript.GetComponent<concaveOutline>() != null)
                    {
                        if (prevParentGOWithScript.GetComponent<concaveOutline>().children.Contains(this.gameObject) == true)
                            prevParentGOWithScript.GetComponent<concaveOutline>().children.Remove(this.gameObject);
                    }
                }

                //make ties with new parent
                if (parentGOWithScript != null && children.Contains(parentGOWithScript) == false) //if someone wants to be our parent... and they are not already our child...
                {
                    if (parentGOWithScript.GetComponent<concaveOutline>() != null)
                    {
                        if (parentGOWithScript.GetComponent<concaveOutline>().children.Contains(this.gameObject) == false)
                        {
                            parentGOWithScript.GetComponent<concaveOutline>().children.Add(this.gameObject);
                            parentGOWithScript.GetComponent<concaveOutline>().updateUniversalVars();
                        }

                    }
                    else if (parentGOWithScript.GetComponent<convexOutline>() != null)
                    {
                        if (parentGOWithScript.GetComponent<convexOutline>().children.Contains(this.gameObject) == false)
                        {
                            parentGOWithScript.GetComponent<convexOutline>().children.Add(this.gameObject);
                            parentGOWithScript.GetComponent<convexOutline>().updateUniversalVars();
                        }

                    }
                }
                else
                    parentGOWithScript = null;
            }
            prevParentGOWithScript = parentGOWithScript;
        }

        //Update the Variables shared by all outline types
        public void updateUniversalVars()
        {
            //--- Sprite Overlay
            Active_SO = Active_SO;
            OrderInLayer_SO = OrderInLayer_SO;
            Color_SO = Color_SO;

            //----- Outline
            Active_O = Active_O;
            Color_O = Color_O;
            OrderInLayer_O = OrderInLayer_O; //by default behind
            Size_O = Size_O;
            ScaleWithParentX_O = ScaleWithParentX_O;
            ScaleWithParentY_O = ScaleWithParentY_O;
        }

        void UpdatepositionsOfEdges()
        {
            if (edges_1 != null)
                foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
                    UpdateEdgePosition(entry.Key, entry.Value);
        }

        void UpdateEdgePosition(GameObject anEdge, Vector2 vect)
        {
            if (PushType_Regular_or_Custom_OP)
            {
                anEdge.transform.position = vect.normalized; //use ONLY vector (1) direction

                if (RadialPush_OPR) //Radial Push
                    anEdge.transform.position *= Size_O;
                else //Square Push
                    anEdge.transform.position *= Mathf.Abs(Size_O / Mathf.Cos((Vector2.Angle(vect, Vector2.up) % 90) * Mathf.Deg2Rad));
            }
            else
            {
                if (StdSize_OPC) //STANDARD size for all vectors
                    anEdge.transform.position = vect.normalized * Size_O; //use ONLY vector (1) direction
                else
                    anEdge.transform.position = vect; //use ONLY vector (1) direction (2) magnitude
            }

            //NOTE: as of now our position is still on a compass in the (0,0,0) position

            if (ScaleWithParentX_O)
                anEdge.transform.position = new Vector2(anEdge.transform.position.x * this.transform.localScale.x, anEdge.transform.position.y);

            if (ScaleWithParentY_O)
                anEdge.transform.position = new Vector2(anEdge.transform.position.x, anEdge.transform.position.y * this.transform.localScale.y);

            anEdge.transform.position = this.transform.position + (this.transform.rotation * anEdge.transform.position);
        }

        //--- Outline Edge List Edits

        public bool addOutline(Vector2 outlineDirection)
        {
            return addOutline(outlineDirection, false);
        }

        bool addOutline(Vector2 outlineDirection, bool sudo)
        {
            if (PushType_Regular_or_Custom_OP == false || sudo == true)
            {
                if (Outline != null)
                {
                    //NOTE: here we must manually take into account, color, order in layer, activeness

                    GameObject tempSpriteCopy = new GameObject();
                    tempSpriteCopy.AddComponent<SpriteRenderer>();

                    copyOriginalGO_Transforms(tempSpriteCopy);

                    //assign our parent
                    tempSpriteCopy.transform.parent = Outline.transform;

                    //use text shader so that we only conserve the silhouette of our sprite
                    var tempMaterial = new Material(tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial);
                    tempMaterial.shader = Shader.Find("GUI/Text Shader");
                    tempSpriteCopy.GetComponent<SpriteRenderer>().sharedMaterial = tempMaterial;

                    //set color
                    tempSpriteCopy.GetComponent<SpriteRenderer>().color = Color_O;

                    //set sorting layer
                    tempSpriteCopy.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer_O;

                    //set sprite renderer data
                    copySpriteRendererData(this.GetComponent<SpriteRenderer>(), tempSpriteCopy.GetComponent<SpriteRenderer>());

                    //set interaction with mask
                    if (clipCenter_CM)
                        tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                    else
                        tempSpriteCopy.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

                    //save our data
                    edges_1.Add(tempSpriteCopy, outlineDirection);

                    //update position that is affected by... size, scale par x, scale par y
                    UpdateEdgePosition(tempSpriteCopy, outlineDirection);

                    //set active state
                    tempSpriteCopy.SetActive(Active_O);

                    return true;
                }
                else
                    return false;
            }
            else //if we have a REGULAR outline then we cant change the list of edges directly
                return false;
        }

        public bool removeOutline(GameObject edgeGO)
        {
            return removeOutline(edgeGO, false);
        }

        bool removeOutline(GameObject edgeGO, bool sudo)
        {
            if (PushType_Regular_or_Custom_OP == false || sudo == true)
            {
                if (edges_1.ContainsKey(edgeGO))
                {
                    edges_1.Remove(edgeGO);
                    DestroyImmediate(edgeGO);
                    return true;
                }
                else
                    return false;
            }
            else //if we have a regular outline then we cant change the list of edges directly
                return false;
        }

        public bool editOutline(GameObject edgeGO, Vector2 newDirection)
        {
            return editOutline(edgeGO, newDirection, false);
        }

        bool editOutline(GameObject edgeGO, Vector2 newDirection, bool sudo)
        {
            if (PushType_Regular_or_Custom_OP == false || sudo == true)
            {
                if (edges_1.ContainsKey(edgeGO))
                {
                    edges_1[edgeGO] = newDirection;

                    //update position that is affected by... size, scale par x, scale par y
                    UpdateEdgePosition(edgeGO, newDirection);

                    return true;
                }
                else
                    return false;
            }
            else //if we have a regular outline then we cant change the list of edges directly
                return false;
        }

        //--- helper functions

        //NOTE: you only need to copy over a variable if its not its default value (for later optimization)
        void copySpriteRendererData(SpriteRenderer from, SpriteRenderer to)
        {
            to.adaptiveModeThreshold = from.adaptiveModeThreshold;
            //COLOR -> set elsewhere
            to.drawMode = from.drawMode;
            to.flipX = from.flipX;
            to.flipY = from.flipY;
            //MASK INTERACTION -> set elsewhere
            to.size = from.size;
            to.sprite = from.sprite;
            to.tileMode = from.tileMode;

            //NOTE: Inherited Members -> Properties... not currently being copied over
        }

        void copyOriginalGO_Transforms(GameObject copierGO)
        {
            //place ourselves in the same place as our parent (for transform copying purposes...)
            if (this.transform.parent != null)
                copierGO.transform.parent = this.transform.parent.gameObject.transform;
            //ELSE... our parent is in the root and currently so are we...

            //copy over all transforms
            copierGO.transform.localScale = this.transform.localScale;
            copierGO.transform.localPosition = this.transform.localPosition;
            copierGO.transform.localRotation = this.transform.localRotation;
        }

        void destroyEdgesThatCreateOutline()
        {
            if (edges_1 != null)
            {
                foreach (KeyValuePair<GameObject, Vector2> entry in edges_1)
                    DestroyImmediate(entry.Key);
                edges_1.Clear();
            }
        }

        void makeSureWeHaveCorrectNumberOfEdges()
        {
            if (PushType_Regular_or_Custom_OP)
            {
                if (edges_1.Count != ObjsMakingOutline_OPR)
                {
                    int iterations = Mathf.Abs(ObjsMakingOutline_OPR - edges_1.Count);

                    if (edges_1.Count < ObjsMakingOutline_OPR)
                    {
                        for (int i = 0; i < iterations; i++)
                            addOutline(Vector2.up, true); //NOTE: the lines will update with a simply Vector3.Up... BUT after this we update our directions and then update all of the lines directions... then all the positions...
                    }
                    else
                    {
                        List<GameObject> keyList = new List<GameObject>(edges_1.Keys);
                        for (int i = 0; i < iterations; i++)
                            removeOutline(keyList[i], true);
                    }
                }
                else
                    ; //we are good to go
            }
            else
                ; //follow custom outline rules
        }

        void updateNormalEdgeVectors()
        {
            if (PushType_Regular_or_Custom_OP)
            {
                //NOTE: only required if regular Outline = True
                float rotation = StartAngle_OPR;
                float angleBetweenAllEdges = (ObjsMakingOutline_OPR == 0) ? 360 : 360 / ObjsMakingOutline_OPR;

                List<GameObject> edgesKeys = new List<GameObject>(edges_1.Keys);
                foreach (var aKey in edgesKeys)
                {
                    float oldMagnitude = edges_1[aKey].magnitude;
                    //NOTE: your direction is calculated from a compass (your obj rotation is not taken into consideration till later)
                    Vector3 newDirection = Quaternion.AngleAxis(rotation, Vector3.forward) * Vector3.up;

                    edges_1[aKey] = newDirection.normalized * oldMagnitude;

                    rotation += angleBetweenAllEdges;
                }

                UpdatepositionsOfEdges(); //we have new directions so we must recalculate our positions because they are based on our directions
            }
            else
                ; //follow custom outline rules
        }
    }
}