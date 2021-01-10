using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AI
{
    internal static string GetMovement(int myId, Grid gameGrid, List<Entity> entities)
    {
        Entity me = entities.Find(e => e.IsPlayer && e.Owner == myId);

        Grid testGrid = new Grid(13, 11);
        int enemy = 0;
        foreach (var pl in entities.Where(e => e.IsPlayer && e != me))
        {
            if (pl.PlayerBombsLeft > 0)
            {
                gameGrid.FindEnemyPath(pl.Location, enemy++);
                gameGrid.CloneTo(testGrid);
                testGrid.SetBomb(pl.Location, pl.PlayerBombRange, 9, pl.Owner);
                testGrid.FindPathTo(pl.Location);
                if (testGrid.HasReachableSafePlace)
                    gameGrid.SetBomb(pl.Location, pl.PlayerBombRange, 9, pl.Owner);
            }
        }

        gameGrid.FindPathTo(me.Location);

        var myBombs = entities.Where(e => e.IsBomb && e.Owner == myId);
        int nextBombIn = myBombs.Any() ? myBombs.Min(e => e.BombTimer) : 0;
 
        string action = "MOVE ";

        Stopwatch stopwatch = Stopwatch.StartNew();
        Vector3 target3 = GetBestTarget(gameGrid, me.Location, myBombs.Count() + me.PlayerBombsLeft, me.BombRange, me.PlayerBombsLeft > 0 ? 0 : nextBombIn, myId);
        stopwatch.Stop();
        Console.Error.WriteLine("Target to go found in: " + stopwatch.ElapsedMilliseconds + "ms");

        if (me.PlayerBombsLeft > 0)  
        {
            int myTotalBombs = me.PlayerBombsLeft + entities.Count(e => e.IsBomb && e.Owner == myId);
            if (target3 != null)
            {
                bool setBomb = false;
                if (target3.Location != me.Location)
                {
                    int currScore = GetTargetScore(gameGrid, me.Location, target3, myBombs.Count() + me.PlayerBombsLeft, me.BombRange, me.PlayerBombsLeft > 0 ? 0 : nextBombIn, myId, testGrid);
                    
                    gameGrid.CloneTo(testGrid);

                    var score = GetTargetScore(gameGrid, me.Location, new Vector3(me.Location, 0), myBombs.Count() + me.PlayerBombsLeft, me.BombRange, me.PlayerBombsLeft > 0 ? 0 : nextBombIn, myId, testGrid);

                    testGrid.SetBomb(me.Location, me.PlayerBombRange, 8, myId);
                    testGrid.FindPathTo(me.Location);

                    if (testGrid.HasReachableSafePlace)
                    {
                        //testGrid.FindPathTo(me.Location);

                        int newScore = GetTargetScore(testGrid, me.Location, target3, myBombs.Count() + me.PlayerBombsLeft, me.BombRange, me.PlayerBombsLeft > 1 ? 0 : nextBombIn, myId, testGrid) + score;

                        if (newScore > currScore || newScore == currScore && me.PlayerBombsLeft > 2)
                            setBomb = true;
                    }
                }
                else setBomb = true;

                /*if (newScore > currScore)
                {
                    setBomb = true;
                int dist = testGrid[target3.Location].Distances.Min();
                testGrid.SetBomb(target3.Location, me.PlayerBombRange, 8, myId);
                testGrid.FindPathTo(target3.Location, dist);
                if ((me.PlayerBombsLeft > 1 || dist > 5 || dist == 0) && testGrid.HasReachableSafePlace)
                {*/
                if (setBomb)
                {
                    gameGrid.SetBomb(me.Location, me.PlayerBombRange, 8, myId);
                    gameGrid.FindPathTo(me.Location);
                    target3 = GetBestTarget(gameGrid, me.Location, myBombs.Count() + me.PlayerBombsLeft, me.BombRange, me.PlayerBombsLeft > 1 ? 0 : nextBombIn, myId);
                    action = "BOMB ";
                    //}
                }

            }

            if (action == "MOVE ")
            {
                gameGrid.CloneTo(testGrid);
                testGrid.SetBomb(me.Location, me.PlayerBombRange, 9, myId);
                testGrid.FindPathTo(me.Location);
                if (testGrid.HasReachableSafePlace)
                    foreach (var pl in entities.Where(e => e.IsPlayer && e != me))
                    {
                        gameGrid.CloneTo(testGrid);
                        testGrid.FindPathTo(pl.Location);
                        if (testGrid.HasReachableSafePlace)
                        {
                            testGrid.SetBomb(me.Location, me.PlayerBombRange, 9, myId);
                            testGrid.FindPathTo(pl.Location);
                            if (!testGrid.HasReachableSafePlace)
                            {
                                action = "BOMB ";
                                break;
                            }
                        }
                    }
            }
        }
        //else nextBombIn = myBombs.Min(e => e.BombTimer);
        
        Vector target = target3.Location;

        for (int Z = target3.Z; Z > 1; Z--)
            target = gameGrid.FollowPathFrom(target, Z);

        action += target;

        return action;
    }
    
    static int getBombItemScore(int myBombs)
    {
        return (myBombs < 3 ? 2 : (myBombs == 1 ? 3 : 1));
    }

    static int getRangeItemScore(int eRange)
    {
        return (eRange < 6 ? 2 : 1);
    }

    static int getItemScoreIn(Cell cell, int myBombs, int eRange, int turns)
    {
        return (cell.IsBombItemIn(turns) ? getBombItemScore(myBombs) : 0) + (cell.IsRangeItemIn(turns) ? getRangeItemScore(eRange) : 0);
    }

    static int GetTargetScore(Grid gameGrid, Vector pos, Vector3 pt, int myBombs, int eRange, int nextBombIn, int myId, Grid testGrid = null)
    {
        if (pt.Z > 7 || nextBombIn > pt.Z && gameGrid[pt.Location].Explosions.Any(e => e.timer <= nextBombIn + 1 && e.timer > pt.Z))
            return -100;

        if (testGrid == null)
            testGrid = new Grid(13, 11);

        gameGrid.CloneTo(testGrid);

        var explosionResult = testGrid.SetBomb(pt.Location, eRange, pt.Z + 9, myId);

        int score = explosionResult.boxesDestroyed + getBombItemScore(myBombs) * explosionResult.bombBoxes + getRangeItemScore(eRange) * explosionResult.rangeBoxes;

        testGrid.FindPathTo(pt.Location, Math.Max(pt.Z, nextBombIn));
        if (!testGrid.HasReachableSafePlace)
            return -100;

        if (gameGrid.BoxesLeft == 0)
            return 24 - Math.Abs(6 - pt.Location.X) - Math.Abs(5 - pt.Location.Y) + Math.Min(gameGrid[pt.Location].NearestEnemy * 2, 5);

        if (testGrid[pt.Location].NearestEnemy - 7 < pt.Z)
        {
            int c = 0;
            for (int i = -4; i <= 4; i++)
                for (int j = -4; j <= 4; j++)
                {
                    Vector v = pt.Location + new Vector(i, j);
                    if (!testGrid.OutOfBounds(v) && (testGrid[v].IsBomb || testGrid[v].Distances.Min() > 7))
                        c++;
                }
            if (c > 8)
                return -100;
        }

        Vector target = pt.Location;

        for (int zz = pt.Z; zz > 0; zz--)
        {
            score += getItemScoreIn(gameGrid[target], myBombs, eRange, zz);
            testGrid[target].RemoveItem();
            target = gameGrid.FollowPathFrom(target, zz);
        }

        if (score == 0)
            return -100;

        score *= 2;
        score += 9 - testGrid[pt.Location].Explosions.Last().timer;


        return score;
    }

    static Vector3 GetBestTarget(Grid gameGrid, Vector pos, int myBombs, int eRange, int nextBombIn, int myId)
    {
        Vector3 bestTarget = null;
        int bestDist = 0;
        int bestScore = -100;

        Grid testGrid = new Grid(13, 11);

        for (int j = 0; j < 11; j++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 13; i++)
            {
                Vector pt = new Vector(i, j);

                for (int z = Math.Max(gameGrid[pt].Distances.FindLastIndex(d => d <= nextBombIn), 0); z < gameGrid[pt].Distances.Count && z != -1; z++)
                {
                    int Z = gameGrid[pt].Distances[z];
                    if (!gameGrid[pt].IsPassableIn(Z) || gameGrid[pt].Explosions.Any(e => e.timer > Z && e.timer <= nextBombIn))
                        continue;

                    //if (gameGrid[pt].Explosions.Any(e => e <= nextBombIn + 1))
                    //    continue;

                    int boxesDestroyed = GetTargetScore(gameGrid, pos, new Vector3(pt, Z), myBombs, eRange, nextBombIn, myId, testGrid);

                    if (boxesDestroyed > bestScore || boxesDestroyed == bestScore && bestDist > Z)
                    {
                        bestTarget = new Vector3(pt, Z);
                        bestDist = Z;
                        bestScore = boxesDestroyed;
                    }

                    break;
                }
            }
            stopwatch.Stop();
            //Console.Error.WriteLine("Row " + j + " investigated in: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        if (bestTarget != null)
            return bestTarget;

        /*for (int j = 0; j < 11; j++)
        {
            for (int i = 0; i < 13; i++)
            {
                Vector pt = new Vector(i, j);
                if (!gameGrid[pt].IsReachable)
                    continue;

                gameGrid.CloneTo(testGrid);

                int boxesDestroyed = testGrid.SetBomb(new Vector(i, j), eRange) + (eRange < 9 && gameGrid[new Vector(i, j)].IsRangeItem || myBombs < 4 && gameGrid[new Vector(i, j)].IsBombItem ? 1 : 0);
                if (boxesDestroyed == 0)
                    continue;

                if (boxesDestroyed > bestScore || boxesDestroyed == bestScore && gameGrid[bestTarget.Value].Distance > gameGrid[new Vector(i, j)].Distance)
                {
                    testGrid.FindPathTo(new Vector(i, j), Math.Max(gameGrid[new Vector(i, j)].Distance, nextBombIn));
                    if (!testGrid.HasReachableSafePlace)
                        continue;

                    bestTarget = new Vector(i, j);
                    bestDist = gameGrid[bestTarget.Value].Distance;
                    bestScore = boxesDestroyed;
                }
            }
        }

        if (bestTarget.HasValue)
            return bestTarget;//*/

        for (int j = 0; j < 11; j++)
        {
            for (int i = 0; i < 13; i++)
            {
                Vector pt = new Vector(i, j);
                if (!gameGrid[pt].IsReachable || gameGrid[pt].IsBlowing)
                    continue;

                foreach (var Z in gameGrid[pt].Distances)
                    if (bestTarget == null || bestDist > Z)
                    {
                        bestTarget = new Vector3(pt, Z);
                        bestDist = Z;
                    }
            }
        }

        if (bestTarget != null)
            return bestTarget;

        return new Vector3(pos, 1);
    }
}
class Cell
{
    char ch;
    int itemTakenIn;

    public List<int> Distances = new List<int>();
    public int[] enemyDistances = new int[] { 255, 255, 255 };
    public List<Explosion> Explosions = new List<Explosion>();

    public bool IsEmpty { get { return ch == '.'; } }
    public bool IsBox { get { return ch >= '0' && ch <= '2'; } }
    public bool IsBomb { get { return Explosions.Any(e => e.IsBomb && e.timer < 9); } }
    public bool IsWall { get { return ch == 'X'; } }
    public bool IsBombItem { get { return ch == 'J'; } }
    public bool IsRangeItem { get { return ch == 'I'; } }
    public bool IsRangeItemBox { get { return ch == '1'; } }
    public bool IsBombItemBox { get { return ch == '2'; } }
    public bool IsItem { get { return IsRangeItem || IsBombItem; } }
    public bool IsPassable { get { return (IsEmpty || IsItem) || (IsBox) && IsBlowing || IsBomb; } }
    public bool IsReachable { get { return Distances.Any(); } }
    public bool IsBlowing { get { return Explosions.Count > 0; } }
    public bool IsEmptyBox { get { return ch == '0'; } }
    public bool IsSafe { get { return !Explosions.Any() || Distances.Max() > Explosions.Max(e => e.timer); } }

    public int NearestEnemy { get { return enemyDistances.Min(); } }

    public Cell(char p)
    {
        ch = p;
        Distances = new List<int>();
        itemTakenIn = 255;
    }

    private Cell(Cell c)
    {
        ch = c.ch;
        Distances = new List<int>(c.Distances);
        itemTakenIn = c.itemTakenIn;
        Explosions = new List<Explosion>(c.Explosions);
    }

    public bool IsPassableIn(int turns)
    {
        return (IsEmpty || IsItem || IsBox && Explosions.Any(e => e.timer < turns)) && !Explosions.Any(e => e.IsBomb && e.timer - turns < 8 && e.timer - turns > 0 || e.timer == turns + 1);
        //return IsPassable && !Explosions.Any(e => e.timer == turns + 1) || IsBox && (Explosions.Any(e => e.timer <= turns) || IsBomb && Explosions.Count > 0 && Explosions.Max(e=>e.timer) >= turns + 8);
    }

    public int TurnsToPass { get { return Explosions.Max(e=>e.timer); } }

    internal void SetItem(int itemType)
    {
        ch = (char)('I' + itemType - 1);
    }

    public void SetBomb(Vector pt, int blowTime, int radius, int owner)
    {
        //ch = 'B';
        //Explosions.Add(new Explosion(radius, blowTime));
        Explosions.Add(new Explosion(pt, radius, blowTime, owner));
    }

    internal void SetBombExplosion(Vector pt, int blowTime, int owner)
    {
        Explosions.Add(new Explosion(pt, 0, blowTime, owner));


        /*if (Explosions.Count == 0 || Explosions.FindIndex(e => e / 9 == blowTime / 9) == -1)
            Explosions.Add(blowTime);
        else if(IsBomb)
            Explosions[Explosions.FindIndex(e => e / 9 == blowTime / 9)] = Math.Min(Explosions.Find(e => e / 9 == blowTime / 9), blowTime);
        else Explosions.Add(blowTime);*/
    }

    internal Cell Clone()
    {
        return new Cell(this);
    }

    public override string ToString()
    {
        return ch.ToString();
    }

    internal void RemoveItem()
    {
        if (IsItem)
            ch = '.';
        else if (IsBox)
            ch = '0';
    }

    internal bool IsBombItemIn(int turns)
    {
        return (IsBombItem || IsBombItemBox && IsBlowing && Explosions.Min(e => e.timer) < turns);
    }

    internal bool IsRangeItemIn(int turns)
    {
        return (IsRangeItem || IsRangeItemBox && IsBlowing && Explosions.Min(e => e.timer) < turns);
    }
}
class Entity
{
    int type, param1, param2;

    public bool IsPlayer { get { return type == 0; } }
    public bool IsBomb { get { return type == 1; } }
    public bool IsItem { get { return type == 2; } }
    public int Owner { get; private set; }
    public Vector Location { get; private set; }

    public int PlayerBombsLeft { get { return param1; } }
    public int PlayerBombRange { get { return param2; } }

    public int BombTimer { get { return param1; } set { param1 = value; } }
    public int BombRange { get { return param2; } } 

    public int ItemType { get { return param1; } }

    public Entity()
    {
        var line = Console.ReadLine();
        Console.Error.WriteLine(line);

        var inputs = line.Split(' ').Select(int.Parse).ToArray();

        type = inputs[0];
        Owner = inputs[1];
        Location = new Vector(inputs[2], inputs[3]);
        param1 = inputs[4];
        param2 = inputs[5];
    }
}
class Explosion
{
    public Vector location;
    public int bombRadius;
    public int timer;
    public int Owner;

    public bool IsBomb { get { return bombRadius > 0; } }

    public Explosion(Vector loc, int bombRadius, int timer, int owner)
    {
        location = loc;
        this.bombRadius = bombRadius;
        this.timer = timer;
        Owner = owner;
    }
}public struct ExplosionResult
{
    public int boxesDestroyed;
    public int bombBoxes, rangeBoxes;
}
class Grid
{
    int width;
    int height;

    Cell[][] grid;
    
    //List<Explosion> bombs = new List<Explosion>();

    public bool HasReachableSafePlace
    {
        get
        {
            for (int j = 0; j < 11; j++)
                for (int i = 0; i < 13; i++)
                    if (grid[j][i].IsReachable && grid[j][i].IsSafe)
                        return true;
            return false;
        }
    }

    public int BoxesLeft
    {
        get
        {
            int boxes = 0;
            for (int j = 0; j < 11; j++)
                for (int i = 0; i < 13; i++)
                    if (grid[j][i].IsBox)
                        boxes++;
            return boxes;
        }
    }

    public Cell this[Vector pt] { get { return grid[pt.Y][pt.X]; } }

    public Grid(int width, int height)
    {
        grid = new Cell[height][];

        this.width = width;
        this.height = height;
    }

    internal void Update()
    {
        for (int i = 0; i < height; i++)
        {
            string line = Console.ReadLine();
            Console.Error.WriteLine(line);
            grid[i] = line.ToCharArray().Select(c => new Cell(c)).ToArray();
        }
    }

    internal bool ExplosionIntersects(List<Entity> entities, Vector explosion, int bombRange, Vector target)
    {
        Vector dir = new Vector(Math.Sign(target.X - explosion.X), Math.Sign(target.Y - explosion.Y));
        if (dir.X != 0 && dir.Y != 0 || dir.X + dir.Y == 0)
            return false;

        for (int i = 1; i < bombRange; i++)
        {
            explosion += dir;
            if (explosion == target)
                return true;

            if (OutOfBounds(explosion) || this[explosion].IsWall || this[explosion].IsBox || entities.Any(e => !e.IsPlayer && e.Location == explosion))
                return false;
        }
        return false;
    }

    public bool OutOfBounds(Vector pt)
    {
        return pt.X < 0 || pt.Y < 0 || pt.X >= width || pt.Y >= height;
    }

    public void FindEnemyPath(Vector enemy, int id)
    {
        for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
                grid[j][i].enemyDistances[id] = 255;

        var q = new Queue<Vector>(256);
        this[enemy].enemyDistances[id] = 0;
        q.Enqueue(enemy);

        while (q.Count != 0)
        {
            var pt = q.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                var p = pt + Vector.Directions[i];
                if (OutOfBounds(p) || !this[p].IsPassable || this[p].enemyDistances[id] != 255)
                    continue;

                this[p].enemyDistances[id] = this[pt].enemyDistances[id] + 1;
                q.Enqueue(p);
            }
        }
    }

    public void FindPathTo(Vector target, int startDist = 0)
    {
        for (var j = 0; j < height; j++)
            for (var i = 0; i < width; i++)
                grid[j][i].Distances.Clear();

        var q = new Queue<Vector>(256);
        this[target].Distances.Add(startDist);
        q.Enqueue(target);
        var v = new List<int>();
        int iter = 0;
        while (q.Count != 0)
        {
            iter++;
            var pt = q.Dequeue();
            v.Clear();
            foreach (var Z in this[pt].Distances)
                for (int i = 0; i < 4; i++)
                {
                    var p = pt + Vector.Directions[i];
                    if (OutOfBounds(p) || !this[p].IsPassable)
                        continue;

                    bool enqueue = false;

                    //!this[p].Distances.Any() && 

                    if ((!this[p].Distances.Any() || this[p].Explosions.Any() && this[p].Explosions.Max(e => e.timer) > this[p].Distances.Max() && Z + 1 >= this[p].Explosions.Max(e => e.timer)) && this[p].IsPassableIn(Z + 1))
                    {
                        this[p].Distances.Add(Z + 1);
                        enqueue = true;
                    }
                    if (this[p].IsBlowing && (!this[p].Distances.Any() || this[p].Distances.Max() <= this[p].Explosions.Max(e => e.timer)))
                    {
                        var latestExplosion = this[p].Explosions.Where(e => e.timer > Z && !this[p].Explosions.Any(ee => ee.timer - 1 == e.timer));
                        if (latestExplosion.Any())
                        {
                            int t = latestExplosion.Min(e => e.timer) - 1;
                            if (this[p].IsBox)
                                t++;
                            if (!this[pt].Explosions.Any(e => e.timer > Z && e.timer <= t + 1) && this[p].IsPassableIn(t + 1))
                            {
                                if (!this[p].Distances.Contains(t + 1))
                                {
                                    this[p].Distances.Add(t + 1);
                                    enqueue = true;
                                }
                                if (Z != t)
                                    v.Add(t);
                            }
                        }
                    }

                    if (enqueue)
                        q.Enqueue(p);
                }
            this[pt].Distances.AddRange(v);
        }
        //Console.Error.WriteLine(iter);
    }
/*if (this[p].IsPassableIn( + x))
{
    this[p].Distance = this[pt].Distance + x;
    q.Enqueue(p);
}
else if ((this[p].IsPassable || this[p].IsBomb || this[p].IsBox) && this[p].Explosions.Any() && !this[pt].Explosions.Any())
{
    this[p].Distance = this[p].TurnsToPass;
    q.Enqueue(p);
}*/
    internal void CloneTo(Grid testGrid)
    {
        for (int j = 0; j < 11; j++)
        {
            if (testGrid.grid[j] == null)
                testGrid.grid[j] = new Cell[13];
            for (int i = 0; i < 13; i++)
                testGrid.grid[j][i] = grid[j][i].Clone();
        }

        //testGrid.bombs.Clear();
        //foreach (var bomb in bombs)
        //    testGrid.bombs.Add(bomb);
    }


    public Vector FollowPathFrom(Vector pt, int Z)
    {
        if (this[pt].Distances.Any(d => d < Z))
        {
            int minDist = this[pt].Distances.FindLast(d => d < Z);
            int maxDist = this[pt].Distances.Find(d => d >= Z);
            if (!this[pt].Explosions.Any(e => e.timer > minDist && e.timer <= maxDist))
                return pt;
        }

        /*if (this[pt].Distance == 255)
        {
            Vector nearest = pt;
            for (int i = 0; i < 4; i++)
            {
                Vector ppt = pt + Vector.Directions[i];
                if (!OutOfBounds(ppt) && this[ppt].Distance < this[nearest].Distance)
                    nearest = ppt;
            }
            return nearest;
        }*/

        for (int i = 0; i < 4; i++)
        {
            Vector ppt = pt + Vector.Directions[i];
            if (!OutOfBounds(ppt) && this[ppt].Distances.Contains(Z - 1))//this[ppt].IsPassableIn(Z))
                return ppt;
            //if (!OutOfBounds(ppt) && this[pt].MaxDist > this[ppt].Distance && this[ppt].MaxDist + 1 >= this[pt].Distance)
            //    return ppt;
        }

        /*for (int i = 0; i < 4; i++)
        {
            Vector ppt = pt + Vector.Directions[i];
            if (!OutOfBounds(ppt) && this[ppt].Distance < this[pt].Distance)
            {
                if (this[ppt].Explosions.Any(e => e.timer >= this[ppt].Distance && e.timer <= this[pt].Distance))
                    this[ppt].Distance = this[ppt].Explosions.Max(e => e.timer) - 1;
                return ppt;
            }
        }*/
        
        return pt;
    }

    /*public void ResetAllBombs()
    {
        bombs.Sort((a, b) => a.timer - b.timer);
        foreach(var bomb in bombs)
            this[bomb.location].SetBomb(bomb.location, bomb.bombRadius, bomb.timer, );
    }*/

    public ExplosionResult SetBomb(Vector pt, int radius, int timer, int owner)
    {
        ExplosionResult score = new ExplosionResult();

        this[pt].Explosions.Sort((a, b) => a.timer - b.timer);
        int t = this[pt].Explosions.FindIndex(e => Math.Abs(e.timer - timer) < 9);
        if (t != -1)
        {
            var e = this[pt].Explosions[t];
            if (timer > e.timer)
                timer = e.timer;
            if (this[pt].IsBomb && timer == e.timer && radius == e.bombRadius && owner == e.Owner)
                return score;

            //bombs.Add(new Explosion(pt, radius, timer, owner));

            //if (this[pt].IsBomb && timer < this[pt].Explosions[t].timer)
            //    SetBomb(pt, e.bombRadius, timer, e.Owner);
        }
        //else bombs.Add(new Explosion(pt, radius, timer, owner));

        /*if (bombs.ContainsKey(pt))
            bombs[pt] = Math.Max(bombs[pt], radius);
        else bombs.Add(pt, radius);
        this[pt].SetBomb(timer, radius);*/

        this[pt].SetBomb(pt, timer, radius, owner);

        for (int i = 0; i < 4; i++)
        {
            bool itemPassed = false;
            for (int l = 1; l < radius; l++)
            {
                var p = pt + Vector.Directions[i] * l;

                if (OutOfBounds(p) || this[p].IsWall)
                    break;

                if (!itemPassed && this[p].IsBox && (!this[p].IsBlowing || !this[p].Explosions.Any(e => e.Owner == owner)))
                {
                    if (this[p].IsBombItemBox)
                        score.bombBoxes++;
                    else if (this[p].IsRangeItemBox)
                        score.rangeBoxes++;
                    score.boxesDestroyed++;
                }

                if (this[p].IsItem)
                    itemPassed = true;

                bool blowing = this[p].Explosions.Any(e => e.timer < timer);
                if (this[p].IsBomb && this[p].Explosions.Any(e => e.IsBomb && Math.Abs(e.timer - timer) < 9))
                    //{
                    //var bomb = bombs.Find(e => e.location == p);
                    //bombs.Remove(bomb);
                    SetBomb(p, this[p].Explosions[0].bombRadius, timer, this[p].Explosions[0].Owner);
                //}
                this[p].SetBombExplosion(pt, timer, owner);

                if ((!this[p].IsEmpty && !((this[p].IsBox || this[p].IsItem) && blowing) || this[p].IsBomb) && !this[p].IsItem)
                    break;
            }
        }

        return score;
    }
}
class Program
{
    static void Main(string[] args)
    {
        var inputs = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
        int width = inputs[0];
        int height = inputs[1];
        int myId = inputs[2];

        Grid gameGrid = new Grid(width, height);

        while (true)
        {
            gameGrid.Update();

            var entities = new List<Entity>(int.Parse(Console.ReadLine()));
            Console.Error.WriteLine(entities.Capacity);
            for (int i = 0; i < entities.Capacity; i++)
                entities.Add(new Entity());

            foreach (var entity in entities.Where(e => e.IsItem))
                gameGrid[entity.Location].SetItem(entity.ItemType);

            var bombs = entities.Where(e => e.IsBomb).ToArray();
            int bombsCount = bombs.Count();
            for (int i = 0; i < bombsCount - 1; i++)
                for (int j = i + 1; j < bombsCount; j++)
                    if (bombs[i].BombTimer != bombs[j].BombTimer && gameGrid.ExplosionIntersects(entities, bombs[i].Location, bombs[i].BombRange, bombs[j].Location))
                        bombs[j].BombTimer = bombs[i].BombTimer;
            
            foreach (var entity in bombs)
                gameGrid.SetBomb(entity.Location, entity.BombRange, entity.BombTimer, entity.Owner);

            Stopwatch stopwatch = Stopwatch.StartNew();
            string movement = AI.GetMovement(myId, gameGrid, entities);
            stopwatch.Stop();
            Console.WriteLine(movement + " " + stopwatch.ElapsedMilliseconds);
        }
    }
}
struct Vector
{
    private static string intToString(int d)
    {
        return d.ToString();
    }

    public int X;
    public int Y;

    public static readonly Vector[] Directions = { 
        new Vector(-1, 0), 
        new Vector(0, -1), 
        new Vector(1, 0),
        new Vector(0, 1) 
    };

    private static float COMPARISON_TOLERANCE = 0.0001f;

    public Vector(int x, int y)
    {
        X = x;
        Y = y;
    }


    static public Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.X + b.X, a.Y + b.Y);
    }

    static public Vector operator *(Vector a, Vector b)
    {
        return new Vector(a.X * b.X, a.Y * b.Y);
    }

    static public Vector operator -(Vector a)
    {
        return new Vector(-a.X, -a.Y);
    }
    static public Vector operator -(Vector a, Vector b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y);
    }

    static public Vector operator *(Vector a, int b)
    {
        return new Vector(a.X * b, a.Y * b);
    }

    public Vector Rotate(int radians)
    {
        int s = (int)Math.Sin(radians),
            c = (int)Math.Cos(radians);
        return new Vector(X * c + Y * s, Y * c - X * s);
    }

    public int Dot(Vector other)
    {
        return X * other.X + Y * other.Y;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Vector))
            return false;
        Vector a = this;
        Vector b = (Vector)obj;
        if (Math.Abs(a.X - b.X) > COMPARISON_TOLERANCE)
            return false;
        if (Math.Abs(a.Y - b.Y) > COMPARISON_TOLERANCE)
            return false;
        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    static public bool operator ==(Vector a, Vector b)
    {
        if (Math.Abs(a.X - b.X) > COMPARISON_TOLERANCE)
            return false;
        if (Math.Abs(a.Y - b.Y) > COMPARISON_TOLERANCE)
            return false;
        return true;
    }

    static public bool operator !=(Vector a, Vector b)
    {
        if (Math.Abs(a.X - b.X) > COMPARISON_TOLERANCE)
            return true;
        if (Math.Abs(a.Y - b.Y) > COMPARISON_TOLERANCE)
            return true;
        return false;
    }

    public int Length { get { return (int)Math.Sqrt(LengthSq); } }
    public int LengthSq { get { return this.Dot(this); } }
    private int Angle { get { return (int)Math.Atan2(Y, X); } }

    public Vector Normalized()
    {
        int length = LengthSq;
        if (length > 0)
            return this * (1 / (int)Math.Sqrt(length));
        return new Vector(0, 0);
    }

    public Vector Ortho()
    {
        return new Vector(-Y, X);
    }

    public override string ToString()
    {
        return intToString(X) + " " + intToString(Y);
    }
}internal class Vector3
{
    public Vector Location;
    public int Z;

    public Vector3(Vector pos, int z)
    {
        Location = pos;
        Z = z;
    }
}