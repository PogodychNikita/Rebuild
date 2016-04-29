using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Game : MonoBehaviour 
{
    public List<List<Location>> Map;
    public List<Location> RecLoc;
    public List<Survivor> Survivors;
    public List<Survivor> SelectedSurv;
    public List<Survivor> DefenseSurv;
    public List<Survivor> TempDefenseSurv;
    public List<Mission> ActiveMissions;
    
    public List<Transform> LocPrefs;
    public List<MissionButton> MissionButtons;
    public LocationCanvas LocationCanv;
    public MissionCanvas MissionCanv;
    public GuardingCanvas GuardCanv;
    public StatusBar StatusBar;
    public Transform SurvButtonsCont;
    public Transform SurvivorList;

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

    public int[,] IndexMap = new int[10, 10];

    public int SaveSlot;
    public SaveLoadSystem _saveLoadSysem;

	void Start () 
    {
        Map = new List<List<Location>>();
        RecLoc = new List<Location>();
        Survivors = new List<Survivor>();
        SelectedSurv = new List<Survivor>();
        ActiveMissions = new List<Mission>();
        DefenseSurv = new List<Survivor>();
        TempDefenseSurv = new List<Survivor>();

        CurLocId = new Index(0, 0);
        CurMissionId = 0;
        MapSize = 10;
        Days = 0;
        IdleSurv = 0;
        LivingSpace = 0;
        Food = 0;
        Happiness = 0;
        Defense = 0;
        DefenseFromBuildings = 0;
        FoodExtraction = 0;
        FoodIntake = 0;

        for (int i = 0; i < MapSize; i++)
        {
            Map.Add(new List<Location>());

            for (int j = 0; j < MapSize; j++)
            {
                Map[i].Add(null);
            }
        }

        _saveLoadSysem = GetComponent<SaveLoadSystem>();
	}

    public void EnterSelectedButton(int num)
    {
        if (Survivors[num].Status == "Idle")
            Survivors[num].Hint.gameObject.SetActive(true);
        else
            Survivors[num].SurvStatus.gameObject.SetActive(true);
    }

    public void ExitSelectedButton(int num)
    {
        Survivors[num].Hint.gameObject.SetActive(false);
        Survivors[num].SurvStatus.gameObject.SetActive(false);
    }

    public void EnterMissinButton(int num)
    {
        MissionButtons[num].MissionStatus.gameObject.SetActive(true);
    }

    public void ExitMissinButton(int num)
    {
        MissionButtons[num].MissionStatus.gameObject.SetActive(false);
    }

    public void CancelSelectMission()
    {
        LocationCanv.Deactivate();
        MissionCanv.Deactivate();
        GuardCanv.Deactivate();

        foreach (Survivor surv in SelectedSurv)
        {
            surv.Status = "Idle";
            surv.Unlock();
        }

        foreach (Survivor surv in DefenseSurv)
        {
            surv.Status = "Guarding the fort";
            surv.Lock();
        }

        SelectedSurv.Clear();

        CalculateDefense(DefenseSurv);
        if (Map[CurLocId.x][CurLocId.y].MissionList.Count != 0)
            Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].Calculate(SelectedSurv);
    }

    public void StartMission()
    {
        LocationCanv.Deactivate();
        MissionCanv.Deactivate();
        GuardCanv.Deactivate();

        ActiveMissions.Add(Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId]);
        Map[CurLocId.x][CurLocId.y].Clock.transform.GetChild(0).GetComponent<Text>().text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].RequareTime.ToString();
        Map[CurLocId.x][CurLocId.y].Clock.SetActive(true);

        for (int i = 0; i < SelectedSurv.Count; i++)
            SelectedSurv[i].Lock();

        SelectedSurv.Clear();
        CountIdleSurv();
    }

    public void AssignDefenders()
    {
        LocationCanv.Deactivate();
        MissionCanv.Deactivate();
        GuardCanv.Deactivate();

        foreach (Survivor surv in DefenseSurv)
            surv.Status = "Idle";

        DefenseSurv.Clear();
        for (int i = 0; i < TempDefenseSurv.Count; i++)
        {
            DefenseSurv.Add(TempDefenseSurv[i]);
            DefenseSurv[i].Status = "Guarding the fort";
            DefenseSurv[i].Lock();
        }

        TempDefenseSurv.Clear();
        CalculateDefense(DefenseSurv);

        StatusBar.DefenseNum.text = Defense.ToString();
    }
    
    public void SelectSurvivor(int num)
    {
        if (Survivors[num].Selected)
        {
            Survivors[num].Deselect();
            SelectedSurv.Remove(Survivors[num]);
        }
        else
        {
            Survivors[num].Select();
            SelectedSurv.Add(Survivors[num]);
        }

        Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].Calculate(SelectedSurv);

        MissionCanv.MissionName.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].ToString();
        MissionCanv.ReqSurvNum.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].RequareSurv.ToString();
        MissionCanv.ReqTimeNum.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].RequareTime.ToString();
        MissionCanv.DangerNum.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].Danger.ToString();

        if (SelectedSurv.Count >= Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].RequareSurv)
            MissionCanv.StartButton.gameObject.SetActive(true);
        else
            MissionCanv.StartButton.gameObject.SetActive(false);
    }

    public void SelectDefender(int num)
    {
        if (Survivors[num].Selected)
        {
            Survivors[num].Deselect();
            TempDefenseSurv.Remove(Survivors[num]);
        }
        else
        {
            Survivors[num].Select();
            TempDefenseSurv.Add(Survivors[num]);
        }

        CalculateDefense(TempDefenseSurv);
        GuardCanv.DefenseNum.text = Defense.ToString();
    }
    
    public void SelectMission(int num)
    {
        CurMissionId = num;

        LocationCanv.Deactivate();
        MissionCanv.Activate();
        GuardCanv.Deactivate();
        SurvivorList.SetParent(MissionCanv.transform, true);
        SurvivorList.localScale = Vector3.one;

        SelectedSurv.Clear();
        Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].Calculate(SelectedSurv);

        MissionCanv.MissionName.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].ToString();
        MissionCanv.ReqSurvNum.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].RequareSurv.ToString();
        MissionCanv.ReqTimeNum.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].RequareTime.ToString();
        if (Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].MaxDanger == 0)
            MissionCanv.DangerNum.gameObject.SetActive(false);
        else
        {
            MissionCanv.DangerNum.gameObject.SetActive(true);
            MissionCanv.DangerText.gameObject.SetActive(false);
            MissionCanv.DangerNum.text = Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].Danger.ToString();
        }

        MissionCanv.StartButton.gameObject.SetActive(false);

        for (int i = 0; i < Survivors.Count; i++)
        {
            Survivors[i].SurvStatus.gameObject.SetActive(false);
            Survivors[i].AddMissionListener();
        }

        Map[CurLocId.x][CurLocId.y].MissionList[CurMissionId].SortSurvButtons(Survivors);
    }

    public void SelectGuarding()
    {
        LocationCanv.Deactivate();
        MissionCanv.Deactivate();
        GuardCanv.Activate();
        SurvivorList.SetParent(GuardCanv.transform);
        SurvivorList.localScale = Vector3.one;

        for (int i = 0; i < Survivors.Count; i++)
        {
            Survivors[i].SurvStatus.gameObject.SetActive(false);
            Survivors[i].AddDefendListener();

            if (Survivors[i].Status == "Guarding the fort")
            {
                Survivors[i].Unlock();
                Survivors[i].Select();
                Survivors[i].Status = "Idle";
                TempDefenseSurv.Add(Survivors[i]);
            }
        }

        CalculateDefense(TempDefenseSurv);
        GuardCanv.DefenseNum.text = Defense.ToString();
    }

    public void SelectLocation(Index id)
    {
        CurLocId = id;

        LocationCanv.Activate();
        MissionCanv.Deactivate();
        GuardCanv.Deactivate();

        SelectedSurv.Clear();

        foreach (MissionButton missionButton in MissionButtons)
        {
            missionButton.gameObject.SetActive(false);
            missionButton.MissionStatus.gameObject.SetActive(false);
        }

        if (Map[CurLocId.x][CurLocId.y].DefensePost && Map[CurLocId.x][CurLocId.y].Reclaimed)
        {
            LocationCanv.HideAll();
            LocationCanv.AssignDefButton.SetActive(true);
        }
        else 
        {
            if (Map[CurLocId.x][CurLocId.y].Scouted)
            {
                LocationCanv.ShowStatus();
                LocationCanv.LocationName.text = Map[CurLocId.x][CurLocId.y].Name;
                LocationCanv.SurvText.text = Map[CurLocId.x][CurLocId.y].SurvPresStatus;
                LocationCanv.FoodText.text = Map[CurLocId.x][CurLocId.y].FoodPresStatus;
                LocationCanv.SafeText.text = Map[CurLocId.x][CurLocId.y].InfestedStatus;
            }
            else LocationCanv.HideStatus();

            LocationCanv.AssignDefButton.SetActive(false);
            foreach (MissionButton missionButton in MissionButtons)
                missionButton.gameObject.SetActive(false);

            for (int i = 0; i < Map[CurLocId.x][CurLocId.y].MissionList.Count; i++)
            {
                MissionButtons[i].MissionName.text = Map[CurLocId.x][CurLocId.y].MissionList[i].ToString();
                MissionButtons[i].gameObject.SetActive(true);

                if (Map[CurLocId.x][CurLocId.y].MissionList[i].RequareSurv > IdleSurv)
                {
                    MissionButtons[i].ShowStatus("We need at least" + Map[CurLocId.x][CurLocId.y].MissionList[i].RequareSurv.ToString() + " idle people to start the mission");
                }
                else if (!Map[CurLocId.x][CurLocId.y].Reclaimed)
                {
                    if (!Map[CurLocId.x][CurLocId.y].MissionList[i].CheckConditions())
                    {
                        MissionButtons[i].ShowStatus(Map[CurLocId.x][CurLocId.y].MissionList[i].Status);
                    }
                    else
                    {
                        MissionButtons[i].ClearButtonEvent();
                        MissionButtons[i].GetComponent<Image>().color = new Color(1, 1, 1);
                        MissionButtons[i].GetComponent<Button>().enabled = true;
                        MissionButtons[i].MissionStatus.gameObject.SetActive(false);
                    }
                }
                else
                {
                    LocationCanv.ShowRecStatus();
                    LocationCanv.LocationDescriptionText.text = Map[CurLocId.x][CurLocId.y].Description;
                    MissionButtons[i].ClearButtonEvent();
                    MissionButtons[i].GetComponent<Image>().color = new Color(1, 1, 1);
                    MissionButtons[i].GetComponent<Button>().enabled = true;
                    MissionButtons[i].MissionStatus.gameObject.SetActive(false);
                }
            }
        }
    }

    public void EndDay()
    {
        LocationCanv.Deactivate();
        MissionCanv.Deactivate();
        GuardCanv.Deactivate();

        for (int i = ActiveMissions.Count - 1; i >= 0; i--)
        {
            ActiveMissions[i].Execute();
        }

        //RandomSurvivorDeath();
        //RandomAddSurvivor();
        //RandomZombieAttack();
        //RandomZombieKill();
        //AddZombiesToLocations();
        //FoodConsumption();
        CalculateDefense(DefenseSurv);
        CalculateLivingSpace();
        CountIdleSurv();
        Days++;
        StatusBar.UpdateStats(Days, Defense, DefenseFromBuildings, LivingSpace, Happiness);

        _saveLoadSysem.Save();
    }

    public void AddSurvivor(Survivor surv)
    {
        surv.gameObject.SetActive(true);
        surv.transform.SetParent(SurvButtonsCont, false);
        surv.AncoredPos = new Vector3(-100, (-(SurvButtonsCont.childCount-1) * 40) - 10, 0);
        surv.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        surv.Id = SurvButtonsCont.childCount - 1;
        Survivors.Add(surv);
        SurvButtonsCont.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
        SurvButtonsCont.GetComponent<RectTransform>().sizeDelta = new Vector2(0, SurvButtonsCont.GetComponent<RectTransform>().sizeDelta.y + 40);
    }

    public void CountIdleSurv()
    {
        IdleSurv = 0;
        for (int i = 0; i < Survivors.Count; i++)
        {
            if (Survivors[i].Status == "Idle")
                IdleSurv++;
        }
    }

    public void CalculateDefense(List<Survivor> defenseSurv)
    {
        Defense = 0;

        foreach (Location loc in RecLoc)
            Defense += loc.DefenseBonus;

        DefenseFromBuildings = Defense;

        for (int i = 0; i < defenseSurv.Count; i++)
            Defense += (int)defenseSurv[i].SpecBonus.SoldierBonus;

        CountIdleSurv();
    }

    public void CalculateLivingSpace()
    {
        LivingSpace = 0;
        foreach (Location loc in RecLoc)
            LivingSpace += loc.LivingSpaceBounus;
    }

    public void GenerateMap()
    {
        GenerateIndexMap();
        SpawnLocation(0, 0, 0, 2304);

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (!Map[i][j].NeighboursAttached)
                    Map[i][j].UpdateNeighbours();
            }
        }

        Map[4][4].Reclaim();
        Map[4][5].Reclaim();
        Map[5][4].Reclaim();
        Map[5][5].Reclaim();

        List<string> StartSurvSpec = new List<string>();
        StartSurvSpec.Add("Leader");
        StartSurvSpec.Add("Scavanger");
        StartSurvSpec.Add("Scavanger");
        StartSurvSpec.Add("Builder");
        StartSurvSpec.Add("Soldier");
        StartSurvSpec.Add("Soldier");
        StartSurvSpec.Add("Soldier");
        StartSurvSpec.Add("Soldier");
        StartSurvSpec.Add("Scientist");
        StartSurvSpec.Add("Untrained");
        StartSurvSpec.Add("Untrained");
        StartSurvSpec.Add("Untrained");

        for (int i = 0; i < 12; i++)
        {
            GameObject newGameObj = GameObject.Instantiate(Resources.Load("Prefabs/Survivors/" + StartSurvSpec[i])) as GameObject;
            newGameObj.GetComponent<Survivor>().Initialize(i, this);
            newGameObj.SetActive(false);

            AddSurvivor(newGameObj.GetComponent<Survivor>());
        }

        CalculateDefense(DefenseSurv);
        CalculateLivingSpace();
        CountIdleSurv();
        StatusBar.UpdateStats(Days, Defense, DefenseFromBuildings, LivingSpace, Happiness);
    }

    public void SpawnLocation(int i, int j, int xLocPos, int yLocPos)
    {
        if (i < 10)
        {
            if (IndexMap[i, j] != -1 && IndexMap[i, j] != -2)
            {
                if (CheckType(IndexMap[i, j]) == 0)
                {
                    GameObject newLoc = Instantiate(LocPrefs[IndexMap[i, j]].gameObject) as GameObject;
                    newLoc.transform.SetParent(transform);
                    newLoc.GetComponent<Location>().RectTransf = newLoc.GetComponent<RectTransform>();
                    newLoc.GetComponent<Location>().AncoredPos = new Vector3(xLocPos + j * 256, yLocPos - j * 128, 0);
                    IndexMap[i, j] = -1;

                    Map[i][j] = newLoc.GetComponent<Location>();
                    Map[i][j].Initialize(new Index(i, j));
                    Map[i][j].Lose();
                }
                else if (CheckType(IndexMap[i, j]) == 1)
                {
                    GameObject newLoc = Instantiate(LocPrefs[IndexMap[i, j]].gameObject) as GameObject;
                    newLoc.transform.SetParent(transform);
                    newLoc.GetComponent<Location>().RectTransf = newLoc.GetComponent<RectTransform>();
                    newLoc.GetComponent<Location>().AncoredPos = new Vector3((xLocPos + j * 256), (yLocPos - j * 128) - 128, 0);
                    IndexMap[i, j] = -1;
                    IndexMap[i, j + 1] = -1;

                    Map[i][j] = newLoc.GetComponent<Location>();
                    Map[i][j + 1] = newLoc.GetComponent<Location>();
                    Map[i][j].Initialize(new Index(i, j));
                    Map[i][j].Lose();
                }
                else if (CheckType(IndexMap[i, j]) == 2 && IndexMap[i + 1, j] == -1)
                {
                    GameObject newLoc = Instantiate(LocPrefs[IndexMap[i, j]].gameObject) as GameObject;
                    newLoc.transform.SetParent(transform);
                    newLoc.GetComponent<Location>().RectTransf = newLoc.GetComponent<RectTransform>();
                    newLoc.GetComponent<Location>().AncoredPos = new Vector3((xLocPos + j * 256) - 256, (yLocPos - j * 128) - 128, 0);
                    IndexMap[i, j] = -1;
                    IndexMap[i + 1, j] = -1;

                    Map[i][j] = newLoc.GetComponent<Location>();
                    Map[i + 1][j] = newLoc.GetComponent<Location>();
                    Map[i][j].Initialize(new Index(i, j));
                    Map[i][j].Lose();
                }
                else if (CheckType(IndexMap[i, j]) == 2 && IndexMap[i + 1, j] != -2)
                {
                    IndexMap[i + 1, j] = -2;
                    SpawnLocation(i + 1, 0, xLocPos - 256, yLocPos - 128);
                }

                if (j < 9)
                    SpawnLocation(i, j + 1, xLocPos, yLocPos);
                else if (j >= 9 && i < 10)
                    SpawnLocation(i + 1, 0, xLocPos - 256, yLocPos - 128);
            }
            else if (IndexMap[i, j] == -1)
            {
                if (j < 9)
                    SpawnLocation(i, j + 1, xLocPos, yLocPos);
                else if (j >= 9 && i < 10)
                    SpawnLocation(i + 1, 0, xLocPos - 256, yLocPos - 128);
            }
            else if (IndexMap[i, j] == -2)
            {
                IndexMap[i, j] = -1;
                SpawnLocation(i - 1, 0, xLocPos + 256, yLocPos + 128);
            }
        }
    }

    public void GenerateIndexMap()
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                IndexMap[i, j] = -1;

        int[] FreeLoc = new int[21];
        FreeLoc[0] = 1; //Cityhall
        FreeLoc[1] = 1; //Evilgraveyard
        FreeLoc[2] = 2; //AllMart
        FreeLoc[3] = 2; //Mall
        FreeLoc[4] = 2; //BigFarm
        FreeLoc[5] = 5; //PoliceStation
        FreeLoc[6] = 5; //Laboratory
        FreeLoc[7] = 5; //School
        FreeLoc[8] = 5; //Hospital
        FreeLoc[9] = 5; //Church
        FreeLoc[10] = 5; //Bar
        FreeLoc[11] = 5; //Apartament
        FreeLoc[12] = 10; //Farm
        FreeLoc[13] = 10; //Park
        FreeLoc[14] = 10; //Suburb
        FreeLoc[15] = 10; //Parking
        FreeLoc[16] = 10; //Warehouse
        FreeLoc[17] = 10; //Office
        FreeLoc[18] = 10; //Mcnoodles
        FreeLoc[19] = 10; //XXOR
        FreeLoc[20] = 10; //Graveyard

        int Row = 0;
        int Col = 0;
        System.Random Rand = new System.Random();
        int randCol = 0;

        IndexMap[4, 4] = 12;
        IndexMap[4, 5] = 14;
        IndexMap[5, 4] = 5;
        IndexMap[5, 5] = 14;

        randCol = Rand.Next(10);
        IndexMap[9, randCol] = 0;
        IndexMap[8, randCol] = 0;
        FreeLoc[0]--;
        randCol = Rand.Next(10);
        IndexMap[1, randCol] = 1;
        IndexMap[0, randCol] = 1;
        FreeLoc[1]--;

        for (int n = 0; n < 100; n++)
        {
            int newLoc = GetRandLoc(FreeLoc, Rand);

            if (CheckType(newLoc) == 0)
            {
                FindPlaceForSmall(Row, Col, newLoc, IndexMap);
            }
            else if (CheckType(newLoc) == 1)
            {
                FindPlaceForBigRight(Row, Col, newLoc, IndexMap);
            }
            else if (CheckType(newLoc) == 2)
            {
                FindPlaceForBigDown(Row, Col, newLoc, IndexMap);
            }

            if (Col < 9) Col++; else if (Row < 9) { Col = 0; Row++; }
        }
    }

    public void FindPlaceForSmall(int _row, int _col, int rand, int[,] map)
    {
        int row = _row;
        int col = _col;

        if (map[row, col] == -1)
        {
            map[row, col] = rand;
        }
        else if (col < 9)
        {
            col++;
            FindPlaceForSmall(row, col, rand, map);
        }
        else if (col >= 9 && row < 9)
        {
            row++;
            col = 0;
            FindPlaceForSmall(row, col, rand, map);
        }
    }

    static void FindPlaceForBigRight(int _row, int _col, int rand, int[,] map)
    {
        int row = _row;
        int col = _col;

        if (map[row, col] == -1)
        {
            if (col < 9)
            {
                if (map[row, col + 1] == -1)
                {
                    map[row, col] = rand;
                    map[row, col + 1] = rand;
                }
                else if (map[row, col + 1] != -1)
                {
                    map[row, col] = 13;
                    col++;
                    FindPlaceForBigRight(row, col, rand, map);
                }
            }
            else if (col >= 9 && row < 9)
            {
                row++;
                col = 0;
                FindPlaceForBigRight(row, col, rand, map);
            }
        }
        else if (map[row, col] != -1)
        {
            if (col < 9)
            {
                col++;
                FindPlaceForBigRight(row, col, rand, map);
            }
            else if (col >= 9 && row < 9)
            {
                row++;
                col = 0;
                FindPlaceForBigRight(row, col, rand, map);
            }
        }
    }

    static void FindPlaceForBigDown(int _row, int _col, int rand, int[,] map)
    {
        int row = _row;
        int col = _col;

        if (row >= 8)
            return;

        if (map[row, col] == -1)
        {
            if (map[row + 1, col] == -1)
            {
                map[row, col] = rand;
                map[row + 1, col] = rand;
            }
            else if (map[row + 1, col] != -1)
            {
                if (col < 9)
                {
                    col++;
                    FindPlaceForBigDown(row, col, rand, map);
                }
                else if (col >= 9)
                {
                    row++;
                    col = 0;
                    FindPlaceForBigDown(row, col, rand, map);
                }
            }
        }
        else if (map[row, col] != -1)
        {
            if (col < 9)
            {
                col++;
                FindPlaceForBigDown(row, col, rand, map);
            }
            else if (col >= 9)
            {
                row++;
                col = 0;
                FindPlaceForBigDown(row, col, rand, map);
            }
        }
    }

    public int GetRandLoc(int[] freeLoc, System.Random rand)
    {
        int freeLocLeft = 0;

        foreach (int fL in freeLoc)
            freeLocLeft += fL;

        if (freeLocLeft != 0)
        {
            int randLoc = 0;

            for (; ; )
            {
                randLoc = rand.Next(21);

                if (freeLoc[randLoc] != 0)
                {
                    freeLoc[randLoc]--;
                    return randLoc;
                }
            }
        }
        else return -1;
    }

    public int CheckType(int loc)
    {
        if (loc == 0) return 2;
        else if (loc == 1) return 2;
        else if (loc == 2) return 2;
        else if (loc == 3) return 1;
        else if (loc == 4) return 1;
        else return 0;
    }
}
