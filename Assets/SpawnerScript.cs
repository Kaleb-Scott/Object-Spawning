using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Net;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.UI;

public class SpawnerScript : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject bondPrefab;
    public TextMeshPro textMesh;
    public TMP_InputField inputField;



    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Space) && inputField.text.Length > 0) //The user pressed the spacebar
        {

            string str = "";

            try
            {
                //Gets data for molecule from the internet as a single string
                WebClient client = new WebClient();
                str = client.DownloadString("https://cactus.nci.nih.gov/chemical/structure/" + inputField.text + "/file?format=sdf");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            //Regular expression used for spliting the lines of the data apart
            Regex reg = new Regex(" *\n *");

            //Splits the lines of data into an array were each element is one line
            string[] strings = reg.Split(str);

            reg = new Regex(" +");

            //Gets the number of atoms contained in the molecule
            int numAtoms = int.Parse(reg.Split(strings[3])[0]);

            //Gets the number of bonds in the molecule
            int numBonds = int.Parse(reg.Split(strings[3])[1]);

            //Array to hold the position data of the atoms
            Vector3[] atomPositions = new Vector3[numAtoms];


            string[] data;
            Vector3 position;

            //creates all of the molecule's atoms
            for (int i = 4; i < 4+numAtoms; i++)
            {
                data = reg.Split(strings[i]);
                
                //textMesh.text = textMesh.text + i + string.Join(" ", data.Take(4));

                //Gets the position data for the current atom and stores it
                position = new Vector3(float.Parse(data[0]) * 5, float.Parse(data[1]) * 5, float.Parse(data[2]) * 5);
                atomPositions[i-4] = position;

                //Instantiates the current atom at the proper position
                Instantiate(spherePrefab, position, Quaternion.identity);
            }

            int firstAtomNum, secondAtomNum;
            float x, y, z;
            float xAngle, yAngle, zAngle;

            for(int i = 4+numAtoms; i < 4+numAtoms+numBonds; i++)
            {
                data = reg.Split(strings[i]);

                firstAtomNum = int.Parse(data[0])-1;
                secondAtomNum = int.Parse(data[1])-1;

                x = (atomPositions[firstAtomNum].x + atomPositions[secondAtomNum].x) / 2;
                y = (atomPositions[firstAtomNum].y + atomPositions[secondAtomNum].y) / 2;
                z = (atomPositions[firstAtomNum].z + atomPositions[secondAtomNum].z) / 2;

                position = new Vector3(x, y, z);

                GameObject tmp = Instantiate(bondPrefab, position, Quaternion.identity);

                Vector3 direction = new Vector3(atomPositions[secondAtomNum].x - atomPositions[firstAtomNum].x, atomPositions[secondAtomNum].y - atomPositions[firstAtomNum].y, atomPositions[secondAtomNum].z - atomPositions[firstAtomNum].z);
                
                tmp.transform.up = direction;

            }
        }
    }
}
