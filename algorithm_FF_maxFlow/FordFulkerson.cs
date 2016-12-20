using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphFlowExample
{
    /// <summary>
    /// 
    /// </summary>
    public class FordFulkerson
    {
        static Dictionary<int, Node> Nodes { get; set; }
        static Dictionary<string, Edge> Edges { get; set; }
        private const float MaxValue = float.MaxValue;

        public FordFulkerson()
        {
            Nodes = new Dictionary<int, Node>();
            Edges = new Dictionary<string, Edge>();
        }



        void ParseData()
        {
            Reset();

            /** 
             * Example graph reference
             * http://www.cs.princeton.edu/%7Ewayne/kleinberg-tardos/07demo-maxflow.ppt 
             * 
             * Put your graph info here
             */
            var names = new[]
            {
                "dummy", "s", //1
                "B-15", "A-16", "B-16", //2, 3, 4
                "English", "C/C++", "Java", "JS", //5, 6, 7, 8
                "Ira", "Alla", "Vika", "Yarik", "Sasha", "Andriy", "Igor", "Yura", //9, 10, 11, 12, 13, 14, 15, 16
                "MonTs-1", "MonTs-2", "MonTs-3", "MonTs-4", "MonTs-5", "MonTs-6", "MonTs-7", //17, 18, 19, 20, 21, 22, 23
                "Magenta", "Sea", "Green", "Zal-1", "Zal-2", "Zal-3", //24, 25, 26, 27, 28, 29,
                "t" //30
            };
            foreach (Node node in names.Select(name => new Node() {Name = name}))
                Nodes.Add(node.Id, node);

            var edges = new[]
            {
                "1 2 999999", "1 3 999999", "1 4 999999", // s->B-15, s->A-15, s->A-16
                "2 5 4", "2 6 6", "2 7 8", "2 8 5", /*B-15->[lessons]*/ 
                "3 5 2", "3 6 6", "3 7 4", "3 8 10", /*A-16->[lessons]*/ 
                "4 5 3", "4 6 5", "4 7 8", "4 8 8", /*B-16->[lessons]*/ 
                "5 9 4", "5 10 4", "5 11 4", /*English->[teachers]*/ 
                "6 11 7", "6 12 7", "6 13 7", /*(C/C++)->[teachers]*/ 
                "7 14 12", "7 15 12", /*Java->[teachers]*/ 
                "8 13 9", "8 15 9", "8 16 9", /*JS->[teachers]*/ 
                "9 17 1", "9 19 1", /*Ira->[TimeSlot]*/ 
                "10 17 1", "10 19 1", /*Alla->[TimeSlot]*/ 
                "11 18 1", "11 19 1", /*Vika->[TimeSlot]*/ 
                "12 18 1", "12 19 1", "12 20 1", /*Yarik->[TimeSlot]*/ 
                "13 22 1", "13 23 1", /*Sasha->[TimeSlot]*/ 
                "14 20 1", "14 21 1", /*Andriy->[TimeSlot]*/ 
                "15 21 1", "15 22 1", "15 23 1", /*Igor->[TimeSlot]*/ 
                "16 21 1", "16 22 1", "16 23 1", /*Yura->[TimeSlot]*/
                "17 24 1", "17 25 1", "17 26 1", "17 27 1", "17 28 1", "17 29 1", /*MonTs-1->room*/
                "18 24 1", "18 25 1", "18 26 1", "18 27 1", "18 28 1", "18 29 1", /*MonTs-2->room*/
                "19 24 1", "19 25 1", "19 26 1", "19 27 1", "19 28 1", "19 29 1", /*MonTs-3->room*/
                "20 24 1", "20 25 1", "20 26 1", "20 27 1", "20 28 1", "20 29 1", /*MonTs-4->room*/
                "21 24 1", "21 25 1", "21 26 1", "21 27 1", "21 28 1", "21 29 1", /*MonTs-5->room*/
                "22 24 1", "22 25 1", "22 26 1", "22 27 1", "22 28 1", "22 29 1", /*MonTs-6->room*/
                "23 24 1", "23 25 1", "23 26 1", "23 27 1", "23 28 1", "23 29 1", /*MonTs-7->room*/
                "24 30 999999", "25 30 999999", "26 30 999999", "27 30 999999", "28 30 999999", "29 30 999999" /*room->t*/

            };

            foreach (var edge in edges)
            {
                string[] s = edge.Split(' ');

                Node node1 = Nodes[int.Parse(s[0])];
                Node node2 = Nodes[int.Parse(s[1])];
                float capacity = float.Parse(s[2]);

                AddEdge(node1, node2, capacity);
                AddEdge(node2, node1, 0f); // residual, if undirected graph, set value as capacity
            }
        }

        public void Run()
        {
            ParseData();
            Algo();
        }

        void Algo()
        {
            var nodeSource = Nodes[1];
            var nodeTerminal = Nodes[Nodes.Count - 1];

            PrintNodes();

            FordFulkersonAlgo(nodeSource, nodeTerminal);
        }


        void FordFulkersonAlgo(Node nodeSource, Node nodeTerminal)
        {
            PrintLn("\n** FordFulkerson");
            var flow = 0f;

            var path = Bfs(nodeSource, nodeTerminal);

            while (path != null && path.Count > 0)
            {
                var minCapacity = MaxValue;
                foreach (var edge in path)
                {
                    if (edge.Capacity < minCapacity)
                        minCapacity = edge.Capacity; // update
                }

                if (minCapacity == MaxValue || minCapacity < 0)
                    throw new Exception("minCapacity " + minCapacity);

                AugmentPath(path, minCapacity);
                flow += minCapacity;

                path = Bfs(nodeSource, nodeTerminal);
            }

            // max flow
            PrintLn("\n** Max flow = " + flow);

            // min cut
            PrintLn("\n** Min cut");
            FindMinCut(nodeSource);
        }


        static void AugmentPath(IEnumerable<Edge> path, float minCapacity)
        {
            foreach (var edge in path)
            {
                var keyResidual = GetKey(edge.NodeTo.Id, edge.NodeFrom.Id);
                var edgeResidual = Edges[keyResidual];

                edge.Capacity -= minCapacity;
                edgeResidual.Capacity += minCapacity;
            }
        }

        // similar to bfs
        void FindMinCut(Node root)
        {
            var queue = new Queue<Node>();
            var discovered = new HashSet<Node>();
            var minCutNodes = new List<Node>();
            var minCutEdges = new List<Edge>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (discovered.Contains(current))
                    continue;

                minCutNodes.Add(current);
                discovered.Add(current);

                var edges = current.NodeEdges;
                foreach (var edge in edges)
                {
                    var next = edge.NodeTo;
                    if (edge.Capacity <= 0 || discovered.Contains(next))
                        continue;
                    queue.Enqueue(next);
                    minCutEdges.Add(edge);
                }
            }

            // bottleneck as a list of arcs
            var minCutResult = new List<Edge>();
            List<int> nodeIds = minCutNodes.Select(node => node.Id).ToList();

            var nodeKeys = new HashSet<int>();
            foreach (var node in minCutNodes)
                nodeKeys.Add(node.Id);

            var edgeKeys = new HashSet<string>();
            foreach (var edge in minCutEdges)
                edgeKeys.Add(edge.Name);


            ParseData(); // reset the graph

            // finding by comparing residual and original graph

            foreach (var id in nodeIds)
            {
                var node = Nodes[id];
                var edges = node.NodeEdges;
                foreach (var edge in edges)
                {
                    if (nodeKeys.Contains(edge.NodeTo.Id))
                        continue;

                    if (edge.Capacity > 0 && !edgeKeys.Contains(edge.Name))
                        minCutResult.Add(edge);
                }
            }

            float maxflow = 0;
            foreach (var edge in minCutResult)
            {
                maxflow += edge.Capacity;
                PrintLn(edge.Info());
            }
            PrintLn("min-cut total maxflow = " + maxflow);
        }

        /*
           Customized for network flow, capacity
          
            Wikipedia
            1. Enqueue the root node.
            2. Dequeue a node and examine it.
                * If the element sought is found in this node, quit the search and return a result.
                * Otherwise enqueue any successors (the direct child nodes) that haven't been seen.
            3. If the queue is empty, every node on the graph has been examined 
                – quit the search and return "not found".
            4. Repeat from Step 2.
         */

        List<Edge> Bfs(Node root, Node target)
        {
            root.TraverseParent = null;
            target.TraverseParent = null; //reset

            var queue = new Queue<Node>();
            var discovered = new HashSet<Node>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();
                discovered.Add(current);

                if (current.Id == target.Id)
                    return GetPath(current);

                var nodeEdges = current.NodeEdges;
                foreach (var edge in nodeEdges)
                {
                    var next = edge.NodeTo;
                    var c = GetCapacity(current, next);
                    if (c > 0 && !discovered.Contains(next))
                    {
                        next.TraverseParent = current;
                        queue.Enqueue(next);
                    }
                }
            }
            return null;
        }


        static List<Edge> GetPath(Node node)
        {
            var path = new List<Edge>();
            var current = node;
            while (current.TraverseParent != null)
            {
                var key = GetKey(current.TraverseParent.Id, current.Id);
                var edge = Edges[key];
                path.Add(edge);
                current = current.TraverseParent;
            }
            return path;
        }

        public static string GetKey(int id1, int id2)
        {
            return id1 + "|" + id2;
        }

        public float GetCapacity(Node node1, Node node2)
        {
            var edge = Edges[GetKey(node1.Id, node2.Id)];
            return edge.Capacity;
        }

        public void AddEdge(Node nodeFrom, Node nodeTo, float capacity)
        {
            var key = GetKey(nodeFrom.Id, nodeTo.Id);
            var edge = new Edge() {NodeFrom = nodeFrom, NodeTo = nodeTo, Capacity = capacity, Name = key};
            Edges.Add(key, edge);
            nodeFrom.NodeEdges.Add(edge);
        }


        static void PrintNodes()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                PrintLn(node.ToString() + " outnodes=" + node.GetInfo());
            }
        }

        static void Reset()
        {
            Nodes.Clear();
            Edges.Clear();
            Node.ResetCounter();
        }

        public class Node
        {
            private static int _counter;
            public readonly int Id;
            public string Name { get; set; }
            public List<Edge> NodeEdges { get; set; }
            public Node TraverseParent { get; set; }

            public Node()
            {
                Id = _counter++;
                NodeEdges = new List<Edge>();
            }

            public static void ResetCounter()
            {
                _counter = 0;
            }

            public string GetInfo()
            {
                var sb = new StringBuilder();
                foreach (var edge in NodeEdges)
                {
                    var node = edge.NodeTo;
                    if (edge.Capacity > 0)
                        sb.Append(node.Name + "C" + edge.Capacity + " ");
                }
                return sb.ToString();
            }

            public override string ToString()
            {
                return string.Format("Id={0}, Name={1}", Id, Name);
            }
        }

        public class Edge
        {
            public Node NodeFrom { get; set; }
            public Node NodeTo { get; set; }
            public float Capacity { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return
                    string.Format("NodeFrom={0}, NodeTo={1}, C={2}", NodeFrom.Name, NodeTo.Name, Capacity);
            }

            public string Info()
            {
                return string.Format("NodeFrom=({0}), NodeTo=({1}), C={2}", NodeFrom, NodeTo, Capacity);
            }
        }

        public static void PrintLn(object o)
        {
            Console.WriteLine(o);
        } //alias

        public static void PrintLn()
        {
            Console.WriteLine();
        } //alias

        public static void Print(object o)
        {
            Console.Write(o);
        } //alias
    }
}