using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cube : MonoBehaviour
{
    public enum CubeType { TYPE_NORMAL, TYPE_NEXT, TYPE_UNAVAILABLE, TYPE_START, TYPE_FINISH, TYPE_NEW_PIECE, TYPE_BLOCK }


    public Point point;
    public bool enabledStatus;
    public bool needEnabler;
    public bool isBreaked;

    private bool lastEnabledStatus;
    private GameManager gameManager;
    private DevelopmentManager devManager;

    private Renderer cubeRenderer;
    private Renderer pillarRenderer;
    private GameObject particlesParent;
    private ParticleSystem[] particles;

    private Color mainColor;
    private CubeType previousType;


    // Start is called before the first frame update
    void Awake()
    {
        if (gameManager == null)
        {
            GameObject obj = GameObject.Find("GameManager");
            if (obj != null)
            {
                gameManager = obj.GetComponent<GameManager>();
            }
        }
        if (devManager == null)
        {
            GameObject obj = GameObject.Find("DevelopmentManager");
            if (obj != null)
            {
                devManager = obj.GetComponent<DevelopmentManager>();
            }
        }

        cubeRenderer = this.GetComponent<Renderer>();
        pillarRenderer = this.gameObject.transform.GetChild(0).GetComponent<Renderer>();
        particlesParent = this.gameObject.transform.GetChild(1).gameObject;

        particles = particlesParent.transform.GetComponentsInChildren<ParticleSystem>();

        this.enabledStatus = lastEnabledStatus = false;
        this.cubeRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastEnabledStatus != enabledStatus)
        {
            //Debug.Log("Cube Update " + (mainColor!=null?"null":mainColor.ToString()));
            lastEnabledStatus = enabledStatus;
            cubeRenderer.enabled = enabledStatus;
            if (enabledStatus)
            {
                if (mainColor == null)
                {
                    SetCubeType(CubeType.TYPE_NORMAL);
                }
                else
                {
                    ChangeCubeStatus(enabledStatus, mainColor);
                }
              
            }
            else
            {
                SetCubeType(CubeType.TYPE_UNAVAILABLE);
            }
        }

    }

    public void InitializeCube(Point point)
    {
        this.gameObject.name = "Cube_" + point.i + "_" + point.j;
        this.point = point;
        this.enabledStatus = lastEnabledStatus = false;
        this.cubeRenderer.enabled = false;
        SetCubeType(CubeType.TYPE_UNAVAILABLE);
        this.pillarRenderer.material.color = Constants.MATRIX_PILLAR_COLOR_01;
        StandardShaderUtils.ChangeRenderMode(this.pillarRenderer.material, GlobalSingleton.GetInstance().GetMainCubesBlendMode()); 
        this.particlesParent.SetActive(false);
    }

    public void RestoreEvenColorCubes()
    {
        lastEnabledStatus = enabledStatus = true;
        cubeRenderer.enabled = true;
        Color cubeColor = ((point.i + point.j) % 2 == 0 ? Constants.MATRIX_BOX_BLACK_COLOR : Constants.MATRIX_BOX_WHITE_COLOR);
        ChangeCubeType(cubeColor, Constants.MATRIX_PILLAR_COLOR_01, cubeColor, false);
        previousType = CubeType.TYPE_NORMAL;
        needEnabler = false;
    }

    public void ChangeCubeType(Color cubeColor, Color pillarColor, Color particleColor, bool particleVisibility)
    {
        if (cubeRenderer == null)
        {
            return;
        }
        //cube color
        cubeRenderer.material.color = cubeColor;
        pillarRenderer.material.color = pillarColor;

        //particle
        if (SystemInfo.systemMemorySize > Constants.RAM_HIGH && particleVisibility)
        {
            ChangeParticlesColor(particleColor);
            particlesParent.SetActive(true);
        }
        else {
            particlesParent.SetActive(false);
        }
    }

    private void ChangeParticlesColor(Color particleColor) {
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
            var main = particle.main;
            main.startColor = particleColor;
            particle.Play();
        }
    }

    public void RestoreCube()
    {
        RestoreEvenColorCubes();
        pillarRenderer.material.color =Constants.MATRIX_PILLAR_COLOR_01;
        particlesParent.SetActive(false);
    }

    public void ChangeCubeStatus(bool status, Color color) {
        //gameObject.SetActive(true);
        enabledStatus = status;
        cubeRenderer.material.color = mainColor = color;
        pillarRenderer.material.color = (status ? Constants.MATRIX_PILLAR_COLOR_01 : Constants.MATRIX_PILLAR_COLOR_02);
        needEnabler = !status;
    }

    public void SetCubeType(CubeType type)
    {
        bool particleVisibility = false;
        Color particleColor = new Color();
        Color cubeColor = new Color();
        Color pillarColor = Constants.MATRIX_PILLAR_COLOR_01;

        switch (type)
        {
            case CubeType.TYPE_NORMAL:
                {
                    previousType = type;
                    particleVisibility = false;
                    int even = ((point.i + point.j) % 2 == 0 ? 0 : 1);
                    cubeColor = new Color(even, even, even, 1f);
                    break;
                }
            case CubeType.TYPE_NEXT:
                {
                    cubeColor = Constants.MATRIX_BOX_NEXT_COLOR;

                    if (previousType == CubeType.TYPE_FINISH || previousType == CubeType.TYPE_START)
                    {
                        particleVisibility = true;
                        if (previousType == CubeType.TYPE_FINISH) {
                            cubeColor = Constants.MATRIX_BOX_FINISH_NEXT_COLOR;
                            particleColor = Constants.MATRIX_BOX_FINISH_COLOR;
                        }
                        else {
                            cubeColor = Constants.MATRIX_BOX_START_NEXT_COLOR;
                            particleColor = Constants.MATRIX_BOX_START_COLOR;
                        }
                    }
                  
                    break;
                }
            case CubeType.TYPE_UNAVAILABLE:
                {
                    previousType = type;
                    particleVisibility = false;
                    int even = ((point.i + point.j) % 2 == 0 ? 0 : 1);
                    cubeColor = new Color(even, even, even, 0f);
                    pillarColor = Constants.MATRIX_PILLAR_COLOR_02;
                    break;
                }
            case CubeType.TYPE_START:
                {
                    previousType = type;
                    particleVisibility = true;
                    particleColor = (Constants.MATRIX_BOX_START_COLOR + new Color(0.1f, 0.1f, 0.1f));
                    cubeColor = Constants.MATRIX_BOX_START_COLOR;
                    break;
                }
            case CubeType.TYPE_FINISH:
                {
                    previousType = type;
                    particleVisibility = true;
                    particleColor = (Constants.MATRIX_BOX_FINISH_COLOR + new Color(0.1f, 0.1f, 0.1f));
                    cubeColor = Constants.MATRIX_BOX_FINISH_COLOR;
                    break;
                }
            case CubeType.TYPE_NEW_PIECE:
                {
                    previousType = type;
                    particleVisibility = false;
                    cubeColor = Constants.MATRIX_BOX_NEW_PIECE_COLOR;
                    break;
                }
            case CubeType.TYPE_BLOCK:
                {
                    previousType = type;
                    particleVisibility = false;
                    cubeColor = Constants.MATRIX_BLOCK_COLOR_01;
                    break;
                }
        }
        ChangeCubeType(cubeColor, pillarColor, particleColor, particleVisibility);
    }

    public CubeType GetPreviousType() {
        return previousType;
    }


}
