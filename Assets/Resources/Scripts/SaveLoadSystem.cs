using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoadSystem : MonoBehaviour
{
    public Game _game;
    public static bool NewGame;
    public static bool LoadGame;
    public static int SaveSlot;
    public int Slot;
    public Button NewGameButton;
    public Button LoadButton;
    public Button SaveSlot1Button;
    public Button SaveSlot2Button;
    public Button SaveSlot3Button;
    public Button BackButton;

    public void Start()
    {
        if (NewGame)
        {
            _game.GenerateMap();
        }
        else if (LoadGame)
        {
            Slot = SaveSlot;
            Load();
        }
    }

    public void PressNewGame()
    {
        NewGame = true;
        SceneManager.LoadScene(1);
    }

    public void PressLoad()
    {
        NewGameButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(true);
        SaveSlot1Button.gameObject.SetActive(true);
        SaveSlot2Button.gameObject.SetActive(true);
        SaveSlot3Button.gameObject.SetActive(true);
    }

    public void PressSlot(int slot)
    {
        LoadGame = true;
        SaveSlot = slot;
        SceneManager.LoadScene(1);
    }

    public void PressBack()
    {
        NewGameButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(false);
        SaveSlot1Button.gameObject.SetActive(false);
        SaveSlot2Button.gameObject.SetActive(false);
        SaveSlot3Button.gameObject.SetActive(false);
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");

        GameData gameData = new GameData();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                gameData.LocData[i][j].Name = _game.Map[i][j].name;
                gameData.LocData[i][j].Reclaimed = _game.Map[i][j].Reclaimed;
                gameData.LocData[i][j].Scouted = _game.Map[i][j].Scouted;
                gameData.LocData[i][j].NeighboursAttached = _game.Map[i][j].NeighboursAttached;
                gameData.LocData[i][j].FoodAmount = _game.Map[i][j].FoodAmount;
                gameData.LocData[i][j].ZombiesAmount = _game.Map[i][j].ZombiesAmount;
                gameData.LocData[i][j].SurvPresStatus = _game.Map[i][j].SurvPresStatus;
                gameData.LocData[i][j].FoodPresStatus = _game.Map[i][j].FoodPresStatus;
                gameData.LocData[i][j].InfestedStatus = _game.Map[i][j].InfestedStatus;
                gameData.LocData[i][j].Sibling = _game.Map[i][j].transform.GetSiblingIndex();
                gameData.LocData[i][j].LocPos = new LocationPosition(_game.Map[i][j].AncoredPos.x, _game.Map[i][j].AncoredPos.y, _game.Map[i][j].AncoredPos.z);

                for (int n = 0; n < _game.Map[i][j].SurvPres.Count; n++)
                {
                    gameData.LocData[i][j].LocSurvPres.Add(new SurvivorData());
                    gameData.LocData[i][j].LocSurvPres[n].Name = _game.Map[i][j].SurvPres[n].Name;
                    gameData.LocData[i][j].LocSurvPres[n].Status = _game.Map[i][j].SurvPres[n].Status;
                    gameData.LocData[i][j].LocSurvPres[n].Selected = _game.Map[i][j].SurvPres[n].Selected;
                    gameData.LocData[i][j].LocSurvPres[n].Alive = _game.Map[i][j].SurvPres[n].Alive;
                    gameData.LocData[i][j].LocSurvPres[n].Id = _game.Map[i][j].SurvPres[n].Id;
                    gameData.LocData[i][j].LocSurvPres[n].Specialisation = _game.Map[i][j].SurvPres[n].Specialisation;
                }

                for (int n = 0; n < _game.Map[i][j].SpotSurv.Count; n++)
                {
                    gameData.LocData[i][j].LocSpotSurvPres.Add(new SurvivorData());
                    gameData.LocData[i][j].LocSpotSurvPres[n].Name = _game.Map[i][j].SurvPres[n].Name;
                    gameData.LocData[i][j].LocSpotSurvPres[n].Status = _game.Map[i][j].SurvPres[n].Status;
                    gameData.LocData[i][j].LocSpotSurvPres[n].Selected = _game.Map[i][j].SurvPres[n].Selected;
                    gameData.LocData[i][j].LocSpotSurvPres[n].Alive = _game.Map[i][j].SurvPres[n].Alive;
                    gameData.LocData[i][j].LocSpotSurvPres[n].Id = _game.Map[i][j].SurvPres[n].Id;
                    gameData.LocData[i][j].LocSpotSurvPres[n].Specialisation = _game.Map[i][j].SurvPres[n].Specialisation;
                }
            }
        }

        for (int i = 0; i < _game.Survivors.Count; i++)
        {
            gameData.SurvData.Add(new SurvivorData());
            gameData.SurvData[i].Name = _game.Survivors[i].Name;
            gameData.SurvData[i].Status = _game.Survivors[i].Status;
            gameData.SurvData[i].Selected = _game.Survivors[i].Selected;
            gameData.SurvData[i].Alive = _game.Survivors[i].Alive;
            gameData.SurvData[i].Id = _game.Survivors[i].Id;
            gameData.SurvData[i].Specialisation = _game.Survivors[i].Specialisation;
        }

        for (int i = 0; i < _game.DefenseSurv.Count; i++)
            gameData.DefenseSurvId.Add(_game.DefenseSurv[i].Id);

        for (int i = 0; i < _game.ActiveMissions.Count; i++)
        {
            gameData.ActMissionData.Add(new MissionData());
            gameData.ActMissionData[i].CurLocId = _game.ActiveMissions[i].CurLoc.Id;

            for (int n = 0; n < _game.ActiveMissions[i].SelSurv.Count; n++)
                gameData.ActMissionData[i].SelSurvId.Add(_game.ActiveMissions[i].SelSurv[n].Id);

            gameData.ActMissionData[i].RequareSurv = _game.ActiveMissions[i].RequareSurv;
            gameData.ActMissionData[i].RequareTime = _game.ActiveMissions[i].RequareTime;
            gameData.ActMissionData[i].MinRequareTime = _game.ActiveMissions[i].MinRequareTime;
            gameData.ActMissionData[i].MaxRequareTime = _game.ActiveMissions[i].MaxRequareTime;
            gameData.ActMissionData[i].Danger = _game.ActiveMissions[i].Danger;
            gameData.ActMissionData[i].MinDanger = _game.ActiveMissions[i].MinDanger;
            gameData.ActMissionData[i].MaxDanger = _game.ActiveMissions[i].MaxDanger;
            gameData.ActMissionData[i].Status = _game.ActiveMissions[i].Status;
            gameData.ActMissionData[i].Name = _game.ActiveMissions[i].GetType().Name;
        }

        gameData.CurLocId = _game.CurLocId;
        gameData.CurMissionId = _game.CurMissionId;
        gameData.MapSize = _game.MapSize;
        gameData.Days = _game.Days;
        gameData.IdleSurv = _game.IdleSurv;
        gameData.LivingSpace = _game.LivingSpace;
        gameData.Food = _game.Food;
        gameData.Happiness = _game.Happiness;
        gameData.Defense = _game.Defense;
        gameData.DefenseFromBuildings = _game.DefenseFromBuildings;
        gameData.FoodExtraction = _game.FoodExtraction;
        gameData.FoodIntake = _game.FoodIntake;

        bf.Serialize(file, gameData);
        file.Close();

        Debug.Log("Saved");
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/saveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open);

            GameData gameData = (GameData)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (_game.Map[i][j] == null)
                    {
                        int newLocId = _game.LocPrefs.FindIndex(pref => pref.name == gameData.LocData[i][j].Name);
                        GameObject newPref = _game.LocPrefs.Find(pref => pref.name == gameData.LocData[i][j].Name).gameObject;
                        GameObject newLoc = Instantiate(newPref) as GameObject;
                        newLoc.transform.SetParent(transform);
                        newLoc.transform.SetSiblingIndex(gameData.LocData[i][j].Sibling);

                        if (_game.CheckType(newLocId) == 0)
                        {
                            _game.Map[i][j] = newLoc.GetComponent<Location>();
                        }
                        else if (_game.CheckType(newLocId) == 1)
                        {
                            _game.Map[i][j] = newLoc.GetComponent<Location>();
                            _game.Map[i][j + 1] = newLoc.GetComponent<Location>();
                        }
                        else if (_game.CheckType(newLocId) == 2)
                        {
                            _game.Map[i][j] = newLoc.GetComponent<Location>();
                            _game.Map[i + 1][j] = newLoc.GetComponent<Location>();
                        }

                        _game.Map[i][j].Initialize(new Index(i, j));
                        _game.Map[i][j].AncoredPos = new Vector3(gameData.LocData[i][j].LocPos.X, gameData.LocData[i][j].LocPos.Y, gameData.LocData[i][j].LocPos.Z);
                        _game.Map[i][j].Lose();

                        //_game.Map[i][j].Reclaimed = gameData.LocData[i][j].Reclaimed;
                        _game.Map[i][j].SurvPres.Clear();
                        _game.Map[i][j].SpotSurv.Clear();

                        foreach (SurvivorData survData in gameData.LocData[i][j].LocSurvPres)
                        {
                            GameObject newGameObj = GameObject.Instantiate(Resources.Load("Prefabs/Survivors/" + survData.Specialisation)) as GameObject;
                            newGameObj.transform.SetParent(_game.Map[i][j].transform);
                            newGameObj.GetComponent<Survivor>().Initialize(survData.Id, _game);
                            newGameObj.GetComponent<Survivor>().AncoredPos = new Vector3(0, 0, 0);
                            newGameObj.SetActive(false);
                            _game.Map[i][j].SurvPres.Add(newGameObj.GetComponent<Survivor>());
                        }

                        foreach (SurvivorData survData in gameData.LocData[i][j].LocSpotSurvPres)
                        {
                            GameObject newGameObj = GameObject.Instantiate(Resources.Load("Prefabs/Survivors/" + survData.Specialisation)) as GameObject;
                            newGameObj.transform.SetParent(_game.Map[i][j].transform);
                            newGameObj.GetComponent<Survivor>().Initialize(survData.Id, _game);
                            newGameObj.GetComponent<Survivor>().AncoredPos = new Vector3(0, 0, 0);
                            newGameObj.SetActive(false);
                            _game.Map[i][j].SpotSurv.Add(newGameObj.GetComponent<Survivor>());
                        }
                    }
                }
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (!_game.Map[i][j].NeighboursAttached)
                        _game.Map[i][j].UpdateNeighbours();
                }
            }

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (gameData.LocData[i][j].Reclaimed)
                        _game.Map[i][j].Reclaim();

            for (int i = 0; i < gameData.SurvData.Count; i++)
            {
                GameObject newGameObj = GameObject.Instantiate(Resources.Load("Prefabs/Survivors/" + gameData.SurvData[i].Specialisation)) as GameObject;
                newGameObj.GetComponent<Survivor>().Initialize(gameData.SurvData[i].Id, _game);

                _game.AddSurvivor(newGameObj.GetComponent<Survivor>());
                _game.Survivors[i].Status = gameData.SurvData[i].Status;
            }

            for (int i = 0; i < gameData.DefenseSurvId.Count; i++)
                _game.DefenseSurv.Add(_game.Survivors.Find(s => s.Id == gameData.DefenseSurvId[i]));

            for (int i = 0; i < gameData.ActMissionData.Count; i++)
            {
                var missionType = GetMissionType(gameData.ActMissionData[i].Name);
                if (missionType != null)
                {
                    Mission newMission = (Mission)System.Activator.CreateInstance(missionType);
                    for (int n = 0; n < 10; n++)
                    {
                        for (int m = 0; m < 10; m++)
                        {
                            if (gameData.ActMissionData[i].CurLocId == _game.Map[n][m].Id)
                            {
                                newMission.Initialize(_game.Map[n][m]);
                                n = 10;
                                break;
                            }
                        }
                    }
                    _game.ActiveMissions.Add(newMission);
                }
                else Debug.Log("Mission at " + gameObject.name + " not found");
            }

            foreach (Survivor surv in _game.Survivors)
                if (surv.Status != "Idle")
                    surv.Lock();

            _game.CurLocId = gameData.CurLocId;
            _game.CurMissionId = gameData.CurMissionId;
            _game.MapSize = gameData.MapSize;
            _game.Days = gameData.Days;
            _game.IdleSurv = gameData.IdleSurv;
            _game.LivingSpace = gameData.LivingSpace;
            _game.Food = gameData.Food;
            _game.Happiness = gameData.Happiness;
            _game.Defense = gameData.Defense;
            _game.DefenseFromBuildings = gameData.DefenseFromBuildings;
            _game.FoodExtraction = gameData.FoodExtraction;
            _game.FoodIntake = gameData.FoodIntake;

            Debug.Log("Loaded");
        }
    }

    [Serializable]
    public class GameData
    {
        public List<List<LocationData>> LocData;
        public List<SurvivorData> SurvData;
        public List<Index> RecLocIndex;
        public List<int> DefenseSurvId;
        public List<MissionData> ActMissionData;
        public Index CurLocId;
        public int CurMissionId;
        public int MapSize;
        public int Days;
        public int IdleSurv;
        public int LivingSpace;
        public int Food;
        public int Happiness;
        public int Defense;
        public int DefenseFromBuildings;
        public int FoodExtraction;
        public int FoodIntake;

        public GameData()
        {
            LocData = new List<List<LocationData>>();
            ActMissionData = new List<MissionData>();
            SurvData = new List<SurvivorData>();
            DefenseSurvId = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                LocData.Add(new List<LocationData>());

                for (int j = 0; j < 10; j++)
                {
                    LocData[i].Add(new LocationData());
                }
            }
        }
    }

    [Serializable]
    public class LocationData
    {
        public string Name;
        public bool Reclaimed;
        public bool Scouted;
        public bool NeighboursAttached;
        public int FoodAmount;
        public int ZombiesAmount;
        public string SurvPresStatus;
        public string FoodPresStatus;
        public string InfestedStatus;
        public int Sibling;
        public LocationPosition LocPos;
        public List<SurvivorData> LocSurvPres;
        public List<SurvivorData> LocSpotSurvPres;

        public LocationData()
        {
            LocSurvPres = new List<SurvivorData>();
            LocSpotSurvPres = new List<SurvivorData>();
        }
    }

    [Serializable]
    public class LocationPosition
    {
        public float X;
        public float Y;
        public float Z;

        public LocationPosition()
        {

        }

        public LocationPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    [Serializable]
    public class SurvivorData
    {
        public string Name;
        public string Status;
        public bool Selected;
        public bool Alive;
        public int Id;
        public string Specialisation;

        public SurvivorData()
        {

        }
    }

    [Serializable]
    public class MissionData
    {
        public string Name;
        public Index CurLocId;
        public List<int> SelSurvId;
        public int RequareSurv;
        public int RequareTime;
        public int MinRequareTime;
        public int MaxRequareTime;
        public int Danger;
        public int MinDanger;
        public int MaxDanger;
        public string Status;

        public MissionData()
        {

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