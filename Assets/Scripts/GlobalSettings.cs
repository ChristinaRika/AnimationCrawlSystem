﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{
    private float blurRadius = 1.0f;
    private float hstep = 0.5f;
    private float vstep = 0.5f;
    private float grayLevel = 1.0f;
    public Slider gaussBlurSlider;
    public Slider horizontalStepSlider;
    public Slider verticalStepSlider;
    public Slider grayLevelSlider;
    private int cartoon = 0;
    public Slider redStrengthSlider;
    private float redStrength = 1.0f;
    public Slider greenStrengthSlider;
    private float greenStrength = 1.0f;
    public Slider blueStrengthSlider;
    private float blueStrength = 1.0f;
    public Slider pixelSizeSlider;
    private int pixelSize = 1;


    public Material material;

    // Update is called once per frame
    void Update()
    {
        //set properties of shader
        material.SetFloat("radius", blurRadius);
        material.SetFloat("hstep", hstep);
        material.SetFloat("vstep", hstep);
        material.SetFloat("fadeLevel", grayLevel);
        material.SetInt("cartoon", cartoon);
        material.SetFloat("redStrength",redStrength);
        material.SetFloat("blueStrength",blueStrength);
        material.SetFloat("greenStrength",greenStrength);
        material.SetInt("_PixelSize", pixelSize);
    }
    
    public void ValueChange(){
        blurRadius = gaussBlurSlider.value;
        hstep = horizontalStepSlider.value;
        vstep = verticalStepSlider.value;
        grayLevel = 1 - grayLevelSlider.value;
        redStrength = redStrengthSlider.value;
        blueStrength = blueStrengthSlider.value;
        greenStrength = greenStrengthSlider.value;
        pixelSize = (int)pixelSizeSlider.value;
    }
    public void SetCartoon(){
        cartoon = 1;
    }
    public void UnSetCartoon(){
        cartoon = 0;
    }
}