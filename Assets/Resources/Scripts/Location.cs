using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Location : MonoBehaviour
{
    public string Name;
    public string Type;
    [TextArea(3, 10)]
    public string Description;
    public bool DefensePost;
    public int DefenseBonus;
    public int FoodExtBonus;
    public int HappinesBonus;
    public int LivingSpaceBounus;
    public List<GameObject> Walls;
    public List<string> ReclaimedMissions;
    public Transform ZombieCont;
    public GameObject Clock;
    public Image SqrImage;
    public Image LocImage;
    public Game MainScript;
    public RectTransform RectTransf;
    public List<Location> Neighbours;
    public List<GameObject> Zombies;
    public Vector3 AncoredPos
    {
        get { return RectTransf.anchoredPosition3D; }
        set { RectTransf.anchoredPosition3D = value; }
    }

    public bool Reclaimed;
    public bool Scouted;
    public bool NeighboursAttached;
    public int FoodAmount;
    public int ZombiesAmount;
    public string SurvPresStatus;
    public string FoodPresStatus;
    public string InfestedStatus;
    public Index Id;
    public List<Survivor> SurvPres;
    public List<Survivor> SpotSurv;
    public List<Mission> MissionList;

    public void Initialize(Index id)
    {
        Reclaimed = false;
        Scouted = false;
        FoodAmount = 0;
        ZombiesAmount = 0;
        Id = id;
        MissionList = new List<Mission>();
        Neighbours = new List<Location>();
        SurvPres = new List<Survivor>();
        SpotSurv = new List<Survivor>();
        MainScript = transform.parent.GetComponent<Game>();
        RectTransf = GetComponent<RectTransform>();

        this.gameObject.name = this.gameObject.name.Replace("(Clone)", "");
        
        for (int i = 0; i < ZombieCont.childCount; i++)
            Zombies.Add(ZombieCont.GetChild(i).gameObject);

        SqrImage.GetComponent<Button>().onClick.AddListener(() => MainScript.SelectLocation(Id));
        LocImage.GetComponent<Button>().onClick.AddListener(() => MainScript.SelectLocation(Id));
    }

    public void Lose()
    {
        Reclaimed = false;
        Scouted = false;
        FoodAmount = UnityEngine.Random.Range(10, 50);
        ZombiesAmount = UnityEngine.Random.Range(3, 12);

        SqrImage.color = new Color(0.5f, 0.5f, 0.5f);
        LocImage.color = new Color(0.5f, 0.5f, 0.5f);

        MissionList.Clear();
        MissionList.Add(new ScoutTheArea());
        MissionList.Add(new RecruitSurvivors());
        MissionList.Add(new ScavengeForFood());
        MissionList.Add(new KillZombies());
        MissionList.Add(new ReclaimArea());

        MissionList[0].Initialize(this);
        MissionList[1].Initialize(this);
        MissionList[2].Initialize(this);
        MissionList[3].Initialize(this);
        MissionList[4].Initialize(this);

        ChangeInfestedStatus();
        AddRandSurv();

        if (FoodAmount > 0 && FoodAmount <= 20)
            FoodPresStatus = "Some food";
        else if (FoodAmount > 20)
            FoodPresStatus = "Lots of food";

        if (SurvPres.Count == 0)
            SurvPresStatus = "Nobody";
        else
            SurvPresStatus = "Survivor";

        MainScript.RecLoc.Remove(this);

        UpdateWalls();
    }

    public void Reclaim()
    {
        Reclaimed = true;
        SqrImage.color = new Color(1, 1, 1);
        LocImage.color = new Color(1, 1, 1);

        MissionList.Clear();
        for (int i = 0; i < ReclaimedMissions.Count; i++)
        {
            var missionType = GetMissionType(ReclaimedMissions[i]);
            if (missionType != null)
            {
                Mission newMission = (Mission)System.Activator.CreateInstance(missionType);
                newMission.Initialize(this);
                MissionList.Add(newMission);
            }
            else Debug.Log("Mission at " + gameObject.name + " not found");
        }

        foreach (GameObject zombie in Zombies)
            zombie.SetActive(false);

        MainScript.RecLoc.Add(this);

        UpdateWalls();
    }

    public void ChangeInfestedStatus()
    {
        int n = 0;

        if (ZombiesAmount > 0 && ZombiesAmount <= 3)
        {
            n = 3;
            InfestedStatus = "Safe-ish";
        }
        else if (ZombiesAmount > 3 && ZombiesAmount <= 6)
        {
            n = 4;
            InfestedStatus = "Troubling";
        }
        if (ZombiesAmount > 6 && ZombiesAmount <= 12)
        {
            n = 8;
            InfestedStatus = "Hazardous";
        }
        else if (ZombiesAmount > 12 && ZombiesAmount <= 24)
        {
            n = 12;
            InfestedStatus = "Infested";
        }
        else if (ZombiesAmount > 24)
        {
            n = 16;
            InfestedStatus = "Overrun";
        }

        if (n > 0 && n < 16)
        {
            foreach (GameObject zombie in Zombies)
                zombie.SetActive(false);

            for (int i = 0; i < n; i++)
            {
                int rand = UnityEngine.Random.Range(3, Zombies.Count);
                if (!Zombies[rand].activeSelf)
                    Zombies[rand].SetActive(true);
                else n++;
            }
        }
        else if (n >= 16)
        {
            foreach (GameObject zombie in Zombies)
                zombie.SetActive(true);
        }
        else if (n == 0)
        {
            foreach (GameObject zombie in Zombies)
                zombie.SetActive(false);
            InfestedStatus = "Safe-ish";
        }
    }

    public void AddRandSurv()
    {
        int survAmount = UnityEngine.Random.Range(0, 3);

        for (int i = 0; i < survAmount; i++)
        {
            string newSpec = "";
            int randSpec = UnityEngine.Random.Range(0, 5);

            switch (randSpec)
            {
                case 0: newSpec = "Builder"; break;
                case 1: newSpec = "Leader"; break;
                case 2: newSpec = "Scavanger"; break;
                case 3: newSpec = "Scientist"; break;
                case 4: newSpec = "Soldier"; break;
                case 5: newSpec = "Untrained"; break;
            }

            int survId = int.Parse((Id.x.ToString() + Id.y.ToString()));

            GameObject newGameObj = GameObject.Instantiate(Resources.Load("Prefabs/Survivors/" + newSpec)) as GameObject;
            newGameObj.transform.SetParent(transform);
            newGameObj.GetComponent<Survivor>().Initialize(survId, MainScript);
            newGameObj.GetComponent<Survivor>().AncoredPos = new Vector3(0, 0, 0);
            newGameObj.SetActive(false);
            SurvPres.Add(newGameObj.GetComponent<Survivor>());
        }
    }

    public void UpdateNeighbours()
    {
        if (Type == "Small")
        {
            if (Id.y > 0) Neighbours.Add(MainScript.Map[Id.x][Id.y - 1]);
            else Neighbours.Add(null);

            if (Id.x > 0) Neighbours.Add(MainScript.Map[Id.x - 1][Id.y]);
            else Neighbours.Add(null);

            if (Id.x < 9) Neighbours.Add(MainScript.Map[Id.x + 1][Id.y]);
            else Neighbours.Add(null);

            if (Id.y < 9) Neighbours.Add(MainScript.Map[Id.x][Id.y + 1]);
            else Neighbours.Add(null);

            NeighboursAttached = true;
        }
        else if (Type == "Horizontal")
        {
            if (Id.y > 0) Neighbours.Add(MainScript.Map[Id.x][Id.y - 1]);
            else Neighbours.Add(null);

            if (Id.x > 0) Neighbours.Add(MainScript.Map[Id.x - 1][Id.y]);
            else Neighbours.Add(null);

            if (Id.x > 0 && Id.y < 9) Neighbours.Add(MainScript.Map[Id.x - 1][Id.y + 1]);
            else Neighbours.Add(null);

            if (Id.y < 8) Neighbours.Add(MainScript.Map[Id.x][Id.y + 2]);
            else Neighbours.Add(null);

            if (Id.x < 9) Neighbours.Add(MainScript.Map[Id.x + 1][Id.y]);
            else Neighbours.Add(null);

            if (Id.x < 9 && Id.y < 9) Neighbours.Add(MainScript.Map[Id.x + 1][Id.y + 1]);
            else Neighbours.Add(null);

            NeighboursAttached = true;
        }
        else if (Type == "Vertical")
        {
            if (Id.x > 0) Neighbours.Add(MainScript.Map[Id.x - 1][Id.y]);
            else Neighbours.Add(null);

            if (Id.y > 0) Neighbours.Add(MainScript.Map[Id.x][Id.y - 1]);
            else Neighbours.Add(null);

            if (Id.x < 9 && Id.y > 0) Neighbours.Add(MainScript.Map[Id.x + 1][Id.y - 1]);
            else Neighbours.Add(null);

            if (Id.x < 8) Neighbours.Add(MainScript.Map[Id.x + 2][Id.y]);
            else Neighbours.Add(null);

            if (Id.y < 9) Neighbours.Add(MainScript.Map[Id.x][Id.y + 1]);
            else Neighbours.Add(null);

            if (Id.x < 9 && Id.y < 9) Neighbours.Add(MainScript.Map[Id.x + 1][Id.y + 1]);
            else Neighbours.Add(null);

            NeighboursAttached = true;
        }
    }

    public bool CheckNeighbours()
    {
        bool close = false;

        foreach (Location loc in Neighbours)
        {
            if (loc.Reclaimed)
            {
                close = true;
                break;
            }
            else close = false;
        }

        return close;
    }

    public void UpdateWalls()
    {
        for (int i = 0; i < Neighbours.Count; i++)
        {
            if (Neighbours[i] != null)
            {
                int locSize = 0;

                if (Type == "Small")
                    locSize = 2;
                if (Type == "Horizontal" || Type == "Vertical")
                    locSize = 3;

                if (!Neighbours[i].Reclaimed)
                {
                    if (i < locSize)
                        Walls[i].SetActive(true);
                    else
                    {
                        for (int j = 0; j < Neighbours[i].Neighbours.Count; j++)
                        {
                            if (Neighbours[i].Neighbours[j] != null)
                                if (Neighbours[i].Neighbours[j].Id == Id)
                                    Neighbours[i].Walls[j].SetActive(true);
                        }
                    }
                }
                else if (Neighbours[i].Reclaimed)
                {
                    if (i < locSize)
                        Walls[i].SetActive(false);
                    else
                    {
                        for (int j = 0; j < Neighbours[i].Neighbours.Count; j++)
                        {
                            if (Neighbours[i].Neighbours[j] != null)
                                if (Neighbours[i].Neighbours[j].Id == Id)
                                    Neighbours[i].Walls[j].SetActive(false);
                        }
                    }
                }
            }
        }        
    }

    public System.Type GetMissionType(string TypeName)
    {

        // Try Type.GetType() first. This will work with types defined
        // by the Mono runtime, in the same assembly as the caller, etc.
        var type = System.Type.GetType(TypeName);

        // If it worked, then we're done here
        if (type != null)
            return type;

        // If the TypeName is a full name, then we can try loading the defining assembly directly
        if (TypeName.Contains("."))
        {
            // Get the name of the assembly (Assumption is that we are using 
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = System.Reflection.Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;

        }

        // If we still haven't found the proper type, we can enumerate all of the 
        // loaded assemblies and see if any of them define the type
        var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            // Load the referenced assembly
            var assembly = System.Reflection.Assembly.Load(assemblyName);
            if (assembly != null)
            {
                // See if that assembly defines the named type
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }

        // The type just couldn't be found...
        return null;
    }
}
