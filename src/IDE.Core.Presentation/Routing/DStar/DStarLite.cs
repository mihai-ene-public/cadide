using System;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.Routing.DStar
{
    //code taken from here (I don't think it is working, coding not to great): https://github.com/SorcerersApprentice/DStarLite

    //a c++ implemetation; needs porting:https://github.com/ArekSredzki/dstar-lite

    public class DStarLite
    {
        private readonly List<State> _u = new List<State>();
        private readonly Dictionary<State, StateInfo> _s = new Dictionary<State, StateInfo>();
        private State _start;
        private State _goal;
        private double _kM;
        private const int Maxsteps = 8000;
        private int _steps;
        private bool _change;
        private readonly List<State> _changes = new List<State>();
        private readonly double _mSqrt2 = Math.Sqrt(2.0);
        private readonly int[,] _directions = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 } };

        public void SetGrid(int x, int y)
        {
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    _s.Add(new State(i, j), new StateInfo());
                }
            }
        }

        public void SetStart(int x, int y)
        {
            var temp = new State(x, y);
            if (_s.ContainsKey(temp))
            {
                _start = temp;
            }
        }

        public void SetGoal(int x, int y)
        {
            var temp = new State(x, y);
            if (_s.ContainsKey(temp))
            {
                _goal = temp;
            }
        }

        public void UpdateCost(int x, int y, double cost)
        {
            var temp = new State(x, y);

            if (!_s.ContainsKey(temp)) return;
            var cInfo = _s[temp];
            if (_steps != 0)
            {
                cInfo.CostNew = cost;
                _change = true;
                _changes.Add(temp);
            }
            else
            {
                cInfo.Cost = cost;
            }
        }
        /// <summary>
        /// As per [S. Koenig, 2002]
        /// </summary>
        public double[] CalcKeys(State s)
        {
            var temp = new double[2];
            var sInfo = _s[s];
            temp[0] = Math.Min(sInfo.G, sInfo.Rhs) + Heuristics(_start, s) + _kM;
            temp[1] = Math.Min(sInfo.G, sInfo.Rhs);
            return temp;
        }
        /// <summary>
        /// As per [S. Koenig, 2002]
        /// </summary>
        public void Initialize()
        {
            _u.Clear();
            _kM = 0;
            foreach (var s in _s)
            {
                s.Value.Rhs = s.Value.G = double.PositiveInfinity;
                //s.Value.Keys = new[] { double.PositiveInfinity, double.PositiveInfinity }; // Hmm
            }
            var gInfo = _s[_goal];
            gInfo.Rhs = 0;
            gInfo.Keys[0] = Heuristics(_start, _goal);
            gInfo.Keys[1] = 0;
            _u.Add(_goal);
            _steps = 0;

        }
        /// <summary>
        /// As per [S. Koenig, 2002]
        /// </summary>
        public void UpdateVertex(State u)
        {
            var uInfo = _s[u];
            if (!uInfo.G.Equals(uInfo.Rhs) && _u.Contains(u))
            {
                uInfo.Keys = CalcKeys(u);
                _u.Remove(u);
                Add(u);
            }
            else if (!uInfo.G.Equals(uInfo.Rhs) && !_u.Contains(u))
            {
                uInfo.Keys = CalcKeys(u);
                Add(u);
            }
            else if (uInfo.G.Equals(uInfo.Rhs) && _u.Contains(u))
            {
                _u.Remove(u);

            }
        }
        /// <summary>
        /// As per [S. Koenig, 2002], with one modification, maxsteps, because the code can loop forever otherwise.
        /// </summary>
        public void ComputerShotestPath()
        {
            var startInfo = _s[_start];
            while ((_u.Any() && KeyLessThan(_u.First(), _start) || startInfo.Rhs > startInfo.G) && _steps < Maxsteps)
            {
                _steps++;
                var u = _u.First();
                var uInfo = _s[u];
                var kOld = uInfo.Keys;
                var kNew = CalcKeys(u);
                if (KeyLessThan(kOld, kNew))
                {
                    uInfo.Keys = kNew;
                    _u.Remove(u);
                    Add(u);
                }
                else if (uInfo.G > uInfo.Rhs)
                {
                    uInfo.G = uInfo.Rhs;
                    _u.Remove(u);
                    foreach (var s in Pred(u))
                    {
                        var sInfo = _s[s];
                        sInfo.Rhs = Math.Min(sInfo.Rhs, Cost(s, u) + uInfo.G);
                        UpdateVertex(s);
                    }
                }
                else
                {
                    var gOld = uInfo.G;
                    uInfo.G = double.PositiveInfinity;
                    var tempList = Pred(u);
                    tempList.Add(u);
                    foreach (var s in tempList)
                    {
                        var sInfo = _s[s];
                        if (sInfo.Rhs.Equals(Cost(s, u) + gOld))
                        {
                            if (s.X != _goal.X && s.Y != _goal.Y)
                            {
                                var list = Succ(s);
                                if (list.Any())
                                {
                                    var smallest = list[0];
                                    var smallInfo = _s[smallest];
                                    foreach (var sPrime in list)
                                    {
                                        var sPrimeInfo = _s[sPrime];
                                        if (!(Cost(s, sPrime) + sPrimeInfo.G < Cost(s, smallest) + smallInfo.G)) continue;
                                        smallest = sPrime;
                                        smallInfo = sPrimeInfo;
                                    }
                                    sInfo.Rhs = smallInfo.G + Cost(s, smallest);
                                }
                            }
                        }
                        UpdateVertex(s);
                    }
                }
            }
        }
        /// <summary>
        /// As per [S. Koenig, 2002]
        /// </summary>
        public void Main()
        {
            var last = _start;
            Initialize();
            ComputerShotestPath();
            var h = 0;
            Console.WriteLine(_start.X + " " + _start.Y);
            while (_start.X != _goal.X || _start.Y != _goal.Y)
            {
                var startInfo = _s[_start];
                if (double.IsPositiveInfinity(startInfo.Rhs))
                {
                    Console.WriteLine("No path found.");
                    return;
                }
                var tempList = Succ(_start);
                if (tempList.Any())
                {
                    var smallest = tempList[0];
                    var smallInfo = _s[smallest];
                    foreach (var s in tempList)
                    {
                        var sInfo = _s[s];
                        if (!(Cost(_start, s) + sInfo.G < Cost(_start, smallest) + smallInfo.G)) continue;
                        smallest = s;
                        smallInfo = sInfo;
                    }
                    _start = smallest;
                }
                Console.WriteLine("Iterration: {0}", h);
                Console.WriteLine(_start.X + " " + _start.Y);
                //We scan for changes HERE.
                if (h == 0)
                {
                    UpdateCost(1, 1, 1000);
                }
                h++;
                if (!_change) continue;
                {
                    _kM = _kM + Heuristics(last, _start);
                    last = _start;
                    foreach (var v in _changes)
                    {
                        var changedInfo = _s[v];
                        var cOld = changedInfo.Cost;
                        changedInfo.Cost = changedInfo.CostNew;
                        if (cOld > changedInfo.Cost)
                        {
                            changedInfo.Rhs = Math.Min(changedInfo.Rhs, changedInfo.Cost + changedInfo.G);
                        }
                        else if (changedInfo.Rhs.Equals(cOld + changedInfo.G))
                        {
                            if (v.X != _goal.X && v.Y != _goal.Y)
                            {
                                var list2 = Succ(v);
                                if (list2.Any())
                                {
                                    var smallest = list2[0];
                                    var smallInfo = _s[smallest];
                                    foreach (var ss in list2)
                                    {
                                        var ssInfo = _s[ss];
                                        if (!(Cost(v, ss) + ssInfo.G < Cost(v, smallest) + smallInfo.G)) continue;
                                        smallest = ss;
                                        smallInfo = ssInfo;
                                    }
                                    changedInfo.Rhs = smallInfo.G + Cost(v, smallest);
                                }
                            }
                        }
                        UpdateVertex(v);
                    }
                    _changes.Clear();
                    _change = false;
                    ComputerShotestPath();
                }
            }

        }
        /// <summary>
        /// If you know A*, it shouldn't be a very big leap to understand what's going on here.
        /// </summary>
        /// <param name="a"> The first state.</param>
        /// <param name="b"> The second state.</param>
        /// <returns></returns>
        private bool KeyLessThan(State a, State b)
        {
            var aInfo = _s[a];
            var bInfo = _s[b];

            if (aInfo.Keys[0] < bInfo.Keys[0])
            {
                return true;
            }
            if (!aInfo.Keys[0].Equals(bInfo.Keys[0])) return false;
            return aInfo.Keys[1] < bInfo.Keys[1];
        }
        /// <summary>
        /// Same as the above, but with keys instead of states.
        /// </summary>
        /// <param name="keyA">First pair.</param>
        /// <param name="keyB">Second pair.</param>
        /// <returns></returns>
        private static bool KeyLessThan(IReadOnlyList<double> keyA, IReadOnlyList<double> keyB)
        {
            if (keyA[0] < keyB[0])
            {
                return true;
            }
            if (!keyA[0].Equals(keyB[0])) return false;
            return keyA[1] < keyB[1];
        }

        /// <summary>
        /// The method used to estimate the distance from one state to another.
        /// </summary>
        /// <param name="a">The first state.</param>
        /// <param name="b">The second state.</param>
        /// <returns>Returns the heuristics value.</returns>
        private double Heuristics(State a, State b)
        {
            double min = Math.Abs(a.X - b.X);
            double max = Math.Abs(a.Y - b.Y);
            if (!(min > max)) return ((_mSqrt2 - 1.0) * min + max);
            var temp = min;
            min = max;
            max = temp;
            return ((_mSqrt2 - 1.0) * min + max);
        }
        /// <summary>
        /// Gets the predecessors of a state s.
        /// </summary>
        /// <param name="s">The state.</param>
        /// <returns>A list of predecessors, may be empty.</returns>
        private List<State> Pred(State s)
        {
            var tempList = new List<State>();
            var sInfo = _s[s];
            for (var i = 0; i < 8; i++)
            {
                var temp = new State(s.X + _directions[i, 0], s.Y + _directions[i, 1]);

                if (!_s.ContainsKey(temp)) continue;
                var tInfo = _s[temp];
                if (tInfo.Rhs > sInfo.Rhs)
                {
                    tempList.Add(temp);
                }
            }
            return tempList;
        }
        /// <summary>
        /// Gets the successors of a state s.
        /// </summary>
        /// <param name="s">The state.</param>
        /// <returns>A list of successors, may be empty.</returns>
        private List<State> Succ(State s)
        {
            var tempList = new List<State>();
            var sInfo = _s[s];
            for (var i = 0; i < 8; i++)
            {
                var temp = new State(s.X + _directions[i, 0], s.Y + _directions[i, 1]);

                if (!_s.ContainsKey(temp)) continue;
                var tInfo = _s[temp];
                if (tInfo.G < sInfo.G)
                {
                    tempList.Add(temp);
                }
            }
            return tempList;
        }
        /// <summary>
        /// Gets the cost of traversal from one state to another adjacent state.
        /// </summary>
        /// <param name="a">First state.</param>
        /// <param name="b">Second state.</param>
        /// <returns>The cost, double.</returns>
        private double Cost(State a, State b)
        {

            //var aInfo = _s[a];
            var bInfo = _s[b];
            //return Math.Max(aInfo.Cost, bInfo.Cost);
            return bInfo.Cost;

        }

        public void Add(State state)
        {
            if (_u.Any())
            {
                for (var i = 0; i < _u.Count; i++)
                {
                    var ss = _u[i];

                    if (!KeyLessThan(state, ss)) continue;
                    _u.Insert(i, state);
                    return;
                }
                _u.Insert(_u.Count, state);
            }
            else
            {
                _u.Add(state);
            }
        }
    }

    public class State
    {
        /// <summary>
        /// The state is a combination of this class and StateInfo.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public State(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            var item = obj as State;
            if (item == null)
            {
                return false;
            }
            return (X == item.X && Y == item.Y);
        }

        public override int GetHashCode()
        {
            var hash = 3;
            hash = 79 * hash + X;
            hash = 79 * hash + Y;
            return hash;
        }
    }

    public class StateInfo
    {
        public double[] Keys { get; set; } = { double.PositiveInfinity, double.PositiveInfinity };

        public double G { get; set; } = double.PositiveInfinity;

        public double Rhs { get; set; } = double.PositiveInfinity;

        public double Cost { get; set; } = 1;

        public double CostNew { get; set; }
    }
}
