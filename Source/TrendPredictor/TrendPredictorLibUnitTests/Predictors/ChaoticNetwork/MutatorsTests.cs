using System;using Microsoft.VisualStudio.TestTools.UnitTesting;using TrendPredictorLib;using System.Collections.Generic;namespace TrendPredictorLibUnitTests{    [TestClass]    public class MutatorsTests    {        //its not good idea - but its needed in addRemoveMutators //BAD DESIGN!!!        private Network dummyNetwork;        [TestInitialize()]        public void Initialize()        {            NodeFactory nodeFactory = new NodeFactory(                new NodeFuncGenerator(),                new Dictionary<NodeType, double>()                {                    {NodeType.add, 0.5d},                     {NodeType.compare, 0.5d}                }            );            DataPoint dp1 = new DataPoint();            dp1.input.Add(1.0d);            dp1.input.Add(2.0d);            DataPoint dp2 = new DataPoint();            dp2.input.Add(2.0d);            dp2.input.Add(3.0d);            dummyNetwork = new Network(                nodeFactory,                 new List<DataPoint>(){dp1, dp2},                1000            );         }        class DummyMutator : NetworkMutator        {            public int appliesCounter = 0;            public int revertsCounter = 0;            protected override void ApplyAction()            {                appliesCounter++;            }            protected override void RevertAction()            {                revertsCounter++;            }        }        [TestMethod]        public void NetworkMutatorOkCaseTest()        {            DummyMutator mutator = new DummyMutator();            Assert.AreEqual(0, mutator.appliesCounter);            Assert.AreEqual(0, mutator.revertsCounter);            mutator.Apply();            Assert.AreEqual(1, mutator.appliesCounter);            Assert.AreEqual(0, mutator.revertsCounter);            mutator.Revert();            Assert.AreEqual(1, mutator.appliesCounter);            Assert.AreEqual(1, mutator.revertsCounter);        }        [TestMethod]        [ExpectedException(typeof(ApplicationException))]        public void NetworkMutatorShouldThrowOnDoubleApply()        {            DummyMutator mutator = new DummyMutator();            mutator.Apply();            mutator.Apply();        }        [TestMethod]        [ExpectedException(typeof(ApplicationException))]        public void NetworkMutatorShouldThrowOnDoubleRevert()        {            DummyMutator mutator = new DummyMutator();            mutator.Apply();            mutator.Revert();            mutator.Revert();        }        [TestMethod]        [ExpectedException(typeof(ApplicationException))]        public void NetworkMutatorShouldThrowOnRevertWOApply()        {            DummyMutator mutator = new DummyMutator();            mutator.Revert();        }        [TestMethod]        public void NodeTypeChangeApplyTest()        {            Node node = new Node(NodeType.compare, 0);            NodeTypeChange mutator = new NodeTypeChange(node, NodeType.compare, NodeType.add);            mutator.Apply();            Assert.AreEqual(NodeType.add, node.NodeType);        }        [TestMethod]        public void NodeTypeChangeRevertTest()        {            Node node = new Node(NodeType.compare, 0);            NodeTypeChange mutator = new NodeTypeChange(node, NodeType.compare, NodeType.add);            mutator.Apply();            mutator.Revert();                        Assert.AreEqual(NodeType.compare, node.NodeType);        }        /// <summary>        /// its should be change         /// from:        ///         ///     parent1 ---> output        ///         ///     parent2 (not connected)        ///             /// to:        ///     parent1 ---> _nodeToBeAdded -> output        ///                  /|        ///                 /        ///         parent2/        /// </summary>        [TestMethod]        public void AddRemoveNodeApplyTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.add, 0);            Node output = new Node(NodeType.add, 0);            parent1.ConnectWithOutput(output);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output);            Node nodeToBeAdded = new Node(NodeType.add, 0);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Add,                 node: nodeToBeAdded,                 parent1: parent1,                 parent2: parent2,                 outputs: new List<Node>(){output},                network: dummyNetwork);            mutator.Apply();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(1, parent1.Outputs.Count);            Assert.IsNotNull(parent1.Outputs.Find(nodeToBeAdded));            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(1, parent2.Outputs.Count);            Assert.IsNotNull(parent2.Outputs.Find(nodeToBeAdded));            Assert.AreEqual(2, nodeToBeAdded.Inputs.Count);            Assert.IsNotNull(nodeToBeAdded.Inputs.Find(parent1));            Assert.IsNotNull(nodeToBeAdded.Inputs.Find(parent2));            Assert.AreEqual(1, nodeToBeAdded.Outputs.Count);            Assert.IsNotNull(nodeToBeAdded.Outputs.Find(output));            Assert.AreEqual(1, output.Inputs.Count);            Assert.IsNotNull(output.Inputs.Find(nodeToBeAdded));            Assert.AreEqual(0, output.Outputs.Count);        }        /// <summary>        /// its should be change from:        /// from:        ///     parent1 ---> _nodeToBeRemoved -> output        ///                  /|        ///                 /        ///         parent2/        ///                 /// to:        ///     parent1 ---> output        ///         ///     parent2 (not connected)        /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output);            dummyNetwork.Operations.Add(nodeToBeRemoved);            Node nodeToBeAdded = new Node(NodeType.add, 0);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>(){output},                network: dummyNetwork);            mutator.Apply();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(1, parent1.Outputs.Count);            Assert.IsNotNull(parent1.Outputs.Find(output));            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(0, parent2.Outputs.Count);            Assert.AreEqual(1, output.Inputs.Count);            Assert.IsNotNull(output.Inputs.Find(parent1));            Assert.AreEqual(0, output.Outputs.Count);        }        /// <summary>        /// on graph like:        ///     parent1 ---> output        ///         ///     parent2 (not connected)        ///             /// there should be no changes after applying add and revering it        /// </summary>        [TestMethod]        public void AddRemoveNodeAddApplyAndRevertTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.add, 0);            Node output = new Node(NodeType.add, 0);            parent1.ConnectWithOutput(output);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output);            Node nodeToBeAdded = new Node(NodeType.add, 0);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Add,                node: nodeToBeAdded,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>(){output},                network: dummyNetwork);            mutator.Apply();            mutator.Revert();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(1, parent1.Outputs.Count);            Assert.IsNotNull(parent1.Outputs.Find(output));            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(0, parent2.Outputs.Count);            Assert.AreEqual(1, output.Inputs.Count);            Assert.IsNotNull(output.Inputs.Find(parent1));            Assert.AreEqual(0, output.Outputs.Count);        }        /// <summary>        /// on graph like:        /// parent1 ---> _nodeToBeRemoved -> output        ///              /|        ///             /        ///    parent2 /        ///             /// there should be no changes after applying remove and revering it               /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveApplyAndRevertTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output);            dummyNetwork.Operations.Add(nodeToBeRemoved);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>(){output},                network: dummyNetwork);            mutator.Apply();            mutator.Revert();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(1, parent1.Outputs.Count);            Assert.IsNotNull(parent1.Outputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(1, parent2.Outputs.Count);            Assert.IsNotNull(parent2.Outputs.Find(nodeToBeRemoved));            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent1));            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent2));            Assert.AreEqual(1, nodeToBeRemoved.Outputs.Count);            Assert.IsNotNull(nodeToBeRemoved.Outputs.Find(output));            Assert.AreEqual(1, output.Inputs.Count);            Assert.IsNotNull(output.Inputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, output.Outputs.Count);        }        /// <summary>        /// on graph like:        /// parent1 ---> _nodeToBeRemoved -> output1        ///              /|              \        ///             /                _\|        ///     parent2/                  output2        ///             /// there should be no changes after applying remove and revering it               /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveApplyAndRevertWhenNodeHas2OutputsApplyAndRevertTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output1 = new Node(NodeType.log, 0);            Node output2 = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output1);            nodeToBeRemoved.ConnectWithOutput(output2);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output1);            dummyNetwork.Operations.Add(output2);            dummyNetwork.Operations.Add(nodeToBeRemoved);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>(nodeToBeRemoved.Outputs),                network: dummyNetwork);            mutator.Apply();            mutator.Revert();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(1, parent1.Outputs.Count);            Assert.IsNotNull(parent1.Outputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(1, parent2.Outputs.Count);            Assert.IsNotNull(parent2.Outputs.Find(nodeToBeRemoved));            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent1));            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent2));            Assert.AreEqual(2, nodeToBeRemoved.Outputs.Count);            Assert.AreEqual(output1, nodeToBeRemoved.Outputs.First.Value);            Assert.AreEqual(output2, nodeToBeRemoved.Outputs.Last.Value);            Assert.AreEqual(1, output1.Inputs.Count);            Assert.IsNotNull(output1.Inputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, output1.Outputs.Count);            Assert.AreEqual(1, output2.Inputs.Count);            Assert.IsNotNull(output2.Inputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, output2.Outputs.Count);        }        /// <summary>        /// on graph like:        /// parent1 ---> _nodeToBeRemoved -> output1        ///              /|              \        ///             /                _\|        ///     parent2/                  output2        ///             ///         /// it should looke like this after applying patch:               ///         ///         /// parent1 ---->output1        ///        \        ///        _\|        ///         output2        ///         /// parent2        ///             /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveApplyAndRevertWhenNodeHas2OutputsTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output1 = new Node(NodeType.log, 0);            Node output2 = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output1);            nodeToBeRemoved.ConnectWithOutput(output2);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output1);            dummyNetwork.Operations.Add(output2);            dummyNetwork.Operations.Add(nodeToBeRemoved);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>(nodeToBeRemoved.Outputs),                network: dummyNetwork);            mutator.Apply();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(2, parent1.Outputs.Count);            Assert.AreEqual(output1, parent1.Outputs.First.Value);            Assert.AreEqual(output2, parent1.Outputs.Last.Value);            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(0, parent2.Outputs.Count);            Assert.AreEqual(0, nodeToBeRemoved.Inputs.Count);            Assert.AreEqual(0, nodeToBeRemoved.Outputs.Count);            Assert.AreEqual(1, output1.Inputs.Count);            Assert.IsNotNull(output1.Inputs.Find(parent1));            Assert.AreEqual(0, output1.Outputs.Count);            Assert.AreEqual(1, output2.Inputs.Count);            Assert.IsNotNull(output2.Inputs.Find(parent1));            Assert.AreEqual(0, output2.Outputs.Count);        }        /// <summary>        /// on graph like:        ///         ///   C1 (1st child of parent1 a.k.a child1        ///   /\        ///   ||        ///   ||        /// parent1 ---> _nodeToBeRemoved -> output        ///   ||         /|        ///   \/        /        ///   C3       /        ///           /        ///  parent2 /        ///             /// there should be no changes after applying remove and revering it               /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveApplyAndRevertWhenNodeToRemoveParent1Has3OutputsApplyAndRevertTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node child1 = new Node(NodeType.copy, 0);            Node child3 = new Node(NodeType.copy, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(child1);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent1.ConnectWithOutput(child3);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output);            dummyNetwork.Operations.Add(nodeToBeRemoved);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>() { output },                network: dummyNetwork);            mutator.Apply();            mutator.Revert();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(3, parent1.Outputs.Count);            Assert.AreEqual(child1, parent1.Outputs.First.Value);            Assert.IsNotNull(parent1.Outputs.Find(nodeToBeRemoved));            Assert.AreEqual(child3, parent1.Outputs.Last.Value);            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(1, parent2.Outputs.Count);            Assert.IsNotNull(parent2.Outputs.Find(nodeToBeRemoved));            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent1));            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent2));            Assert.AreEqual(1, nodeToBeRemoved.Outputs.Count);            Assert.IsNotNull(nodeToBeRemoved.Outputs.Find(output));            Assert.AreEqual(1, output.Inputs.Count);            Assert.IsNotNull(output.Inputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, output.Outputs.Count);            Assert.AreEqual(1, child1.Inputs.Count);            Assert.AreEqual(parent1, child1.Inputs.First.Value);            Assert.AreEqual(0, child1.Outputs.Count);            Assert.AreEqual(1, child3.Inputs.Count);            Assert.AreEqual(parent1, child3.Inputs.First.Value);            Assert.AreEqual(0, child3.Outputs.Count);        }        /// <summary>        /// on graph like:        ///         ///   C1 (1st child of parent1 a.k.a child1        ///   /\        ///   ||        ///   ||        /// parent1 ---> _nodeToBeRemoved -> output        ///   ||         /|        ///   \/        /        ///   C3       /        ///           /        ///  parent2 /        ///             /// it should looke like this after applying patch:        ///         ///  C1 (1st child of parent1 a.k.a child1        ///   /\        ///   ||        ///   ||        /// parent1 ---> output        ///   ||                 ///   \/               ///   C3               ///                   ///  parent2                ///          /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveApplyAndRevertWhenNodeToRemoveParent1Has3OutputsTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node child1 = new Node(NodeType.copy, 0);            Node child3 = new Node(NodeType.copy, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(child1);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent1.ConnectWithOutput(child3);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output);            dummyNetwork.Operations.Add(nodeToBeRemoved);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>() { output },                network: dummyNetwork);            mutator.Apply();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(3, parent1.Outputs.Count);            Assert.AreEqual(child1, parent1.Outputs.First.Value);            Assert.IsNotNull(parent1.Outputs.Find(output));            Assert.AreEqual(child3, parent1.Outputs.Last.Value);            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(0, parent2.Outputs.Count);            Assert.AreEqual(0, nodeToBeRemoved.Inputs.Count);            Assert.AreEqual(0, nodeToBeRemoved.Outputs.Count);            Assert.AreEqual(1, output.Inputs.Count);            Assert.IsNotNull(output.Inputs.Find(parent1));            Assert.AreEqual(0, output.Outputs.Count);            Assert.AreEqual(1, child1.Inputs.Count);            Assert.AreEqual(parent1, child1.Inputs.First.Value);            Assert.AreEqual(0, child1.Outputs.Count);            Assert.AreEqual(1, child3.Inputs.Count);            Assert.AreEqual(parent1, child3.Inputs.First.Value);            Assert.AreEqual(0, child3.Outputs.Count);        }        /// <summary>        /// on graph like:        ///         ///   C1 (1st child of parent1 a.k.a child1        ///   /\        ///   ||        ///   ||        /// parent1 ---> _nodeToBeRemoved -> output1        ///   ||         /|             \        ///   \/        /               _\|        ///   C3       /                  output2        ///           /        ///  parent2 /        ///             /// it should looke like this after applying patch:        ///         ///  C1 (1st child of parent1 a.k.a child1        ///   /\        ///   ||        ///   ||        /// parent1 ---> output1        ///   ||   \              ///   \/   _\|           ///   C3     output2          ///                   ///  parent2                ///          /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveApplyAndRevertWhenNodeToRemoveParent1Has3OutputsAndNodeHas2OutputsTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node child1 = new Node(NodeType.copy, 0);            Node child3 = new Node(NodeType.copy, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output1 = new Node(NodeType.log, 0);            Node output2 = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(child1);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent1.ConnectWithOutput(child3);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output1);            nodeToBeRemoved.ConnectWithOutput(output2);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output1);
            dummyNetwork.Operations.Add(child1);
            dummyNetwork.Operations.Add(child3);            dummyNetwork.Operations.Add(nodeToBeRemoved);            dummyNetwork.Operations.Add(output2);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>(nodeToBeRemoved.Outputs),                network: dummyNetwork);            mutator.Apply();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(4, parent1.Outputs.Count);            Assert.AreEqual(child1, parent1.Outputs.First.Value);            Assert.AreEqual(output1, parent1.Outputs.First.Next.Value);            Assert.AreEqual(output2, parent1.Outputs.First.Next.Next.Value);            Assert.AreEqual(child3, parent1.Outputs.Last.Value);            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(0, parent2.Outputs.Count);            Assert.AreEqual(0, nodeToBeRemoved.Inputs.Count);            Assert.AreEqual(0, nodeToBeRemoved.Outputs.Count);            Assert.AreEqual(1, output1.Inputs.Count);            Assert.IsNotNull(output1.Inputs.Find(parent1));            Assert.AreEqual(0, output1.Outputs.Count);            Assert.AreEqual(1, output2.Inputs.Count);            Assert.IsNotNull(output2.Inputs.Find(parent1));            Assert.AreEqual(0, output2.Outputs.Count);            Assert.AreEqual(1, child1.Inputs.Count);            Assert.AreEqual(parent1, child1.Inputs.First.Value);            Assert.AreEqual(0, child1.Outputs.Count);            Assert.AreEqual(1, child3.Inputs.Count);            Assert.AreEqual(parent1, child3.Inputs.First.Value);            Assert.AreEqual(0, child3.Outputs.Count);        }        /// <summary>        /// on graph like:        ///         ///   C1 (1st child of parent1 a.k.a child1        ///   /\        ///   ||        ///   ||        /// parent1 ---> _nodeToBeRemoved -> output1        ///   ||         /|             \        ///   \/        /               _\|        ///   C3       /                  output2        ///           /        ///  parent2 /        ///             /// there should be no changes after applying remove and revering it               /// </summary>        [TestMethod]        public void AddRemoveNodeRemoveApplyAndRevertWhenNodeToRemoveParent1Has3OutputsAndNodeToRemoveHas2OutputsApplyAndRevertTest()        {            Node parent1 = new Node(NodeType.add, 0);            Node parent2 = new Node(NodeType.compare, 0);            Node child1 = new Node(NodeType.copy, 0);            Node child3 = new Node(NodeType.copy, 0);            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);            Node output1 = new Node(NodeType.log, 0);            Node output2 = new Node(NodeType.log, 0);            parent1.ConnectWithOutput(child1);            parent1.ConnectWithOutput(nodeToBeRemoved);            parent1.ConnectWithOutput(child3);            parent2.ConnectWithOutput(nodeToBeRemoved);            nodeToBeRemoved.ConnectWithOutput(output1);            nodeToBeRemoved.ConnectWithOutput(output2);            dummyNetwork.Operations.Add(parent1);            dummyNetwork.Operations.Add(parent2);            dummyNetwork.Operations.Add(output1);            dummyNetwork.Operations.Add(nodeToBeRemoved);            dummyNetwork.Operations.Add(output2);            AddRemoveNode mutator = new AddRemoveNode(                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,                node: nodeToBeRemoved,                parent1: parent1,                parent2: parent2,                outputs: new List<Node>(nodeToBeRemoved.Outputs),                network: dummyNetwork);            mutator.Apply();            mutator.Revert();            Assert.AreEqual(0, parent1.Inputs.Count);            Assert.AreEqual(3, parent1.Outputs.Count);            Assert.AreEqual(child1, parent1.Outputs.First.Value);            Assert.AreEqual(nodeToBeRemoved, parent1.Outputs.First.Next.Value);            Assert.AreEqual(child3, parent1.Outputs.Last.Value);            Assert.AreEqual(0, parent2.Inputs.Count);            Assert.AreEqual(1, parent2.Outputs.Count);            Assert.IsNotNull(parent2.Outputs.Find(nodeToBeRemoved));            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);            Assert.AreEqual(parent1, nodeToBeRemoved.Inputs.First.Value);            Assert.AreEqual(parent2, nodeToBeRemoved.Inputs.Last.Value);            Assert.AreEqual(2, nodeToBeRemoved.Outputs.Count);            Assert.AreEqual(output1, nodeToBeRemoved.Outputs.First.Value);            Assert.AreEqual(output2, nodeToBeRemoved.Outputs.Last.Value);            Assert.AreEqual(1, output1.Inputs.Count);            Assert.IsNotNull(output1.Inputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, output1.Outputs.Count);            Assert.AreEqual(1, output2.Inputs.Count);            Assert.IsNotNull(output2.Inputs.Find(nodeToBeRemoved));            Assert.AreEqual(0, output2.Outputs.Count);            Assert.AreEqual(1, child1.Inputs.Count);            Assert.AreEqual(parent1, child1.Inputs.First.Value);            Assert.AreEqual(0, child1.Outputs.Count);            Assert.AreEqual(1, child3.Inputs.Count);            Assert.AreEqual(parent1, child3.Inputs.First.Value);            Assert.AreEqual(0, child3.Outputs.Count);        }        /// <summary>        /// on graph like:        ///         /// parent1 ---> _nodeToBeRemoved -> output        ///              /|         ///             /         ///   C1       /        ///   /\      |        ///   ||      |        ///   ||      /        ///  parent2 /        ///   ||        ///   ||        ///   \/        ///   C3        ///             /// it should looke like this after applying patch:        ///         /// parent1 ---> output1
        /// 
        /// 
        ///   C1       
        ///   /\      
        ///   ||     
        ///   ||     
        ///  parent2
        ///   ||
        ///   ||
        ///   \/
        ///   C3        ///           /// </summary>
        [TestMethod]
        public void AddRemoveNodeRemoveApplyWhenSecondParentHas3ChildrenTest()
        {
            Node parent1 = new Node(NodeType.add, 0);
            Node parent2 = new Node(NodeType.compare, 0);
            Node child1 = new Node(NodeType.copy, 0);
            Node child3 = new Node(NodeType.copy, 0);
            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);
            Node output = new Node(NodeType.log, 0);

            parent1.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(child1);
            parent2.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(child3);
            nodeToBeRemoved.ConnectWithOutput(output);

            dummyNetwork.Operations.Add(parent1);
            dummyNetwork.Operations.Add(parent2);
            dummyNetwork.Operations.Add(output);
            dummyNetwork.Operations.Add(child1);
            dummyNetwork.Operations.Add(child3);
            dummyNetwork.Operations.Add(nodeToBeRemoved);

            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,
                node: nodeToBeRemoved,
                parent1: parent1,
                parent2: parent2,
                outputs: new List<Node>(nodeToBeRemoved.Outputs),
                network: dummyNetwork);

            mutator.Apply();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsTrue(parent1.Outputs.Contains(output));

            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(2, parent2.Outputs.Count);
            Assert.AreEqual(child1, parent2.Outputs.First.Value);
            Assert.AreEqual(child3, parent2.Outputs.Last.Value);

            Assert.AreEqual(0, nodeToBeRemoved.Inputs.Count);
            Assert.AreEqual(0, nodeToBeRemoved.Outputs.Count);

            Assert.AreEqual(1, output.Inputs.Count);
            Assert.AreEqual(parent1, output.Inputs.First.Value);
            Assert.AreEqual(0, output.Outputs.Count);

            Assert.AreEqual(1, child1.Inputs.Count);
            Assert.AreEqual(parent2, child1.Inputs.First.Value);
            Assert.AreEqual(0, child1.Outputs.Count);

            Assert.AreEqual(1, child3.Inputs.Count);
            Assert.AreEqual(parent2, child3.Inputs.First.Value);
            Assert.AreEqual(0, child3.Outputs.Count);
        }


        /// <summary>
        /// on graph like:
        /// 
        /// parent1 ---> _nodeToBeRemoved -> output
        ///              /| 
        ///             / 
        ///   C1       /
        ///   /\      |
        ///   ||      |
        ///   ||      /
        ///  parent2 /
        ///   ||
        ///   ||
        ///   \/
        ///   C3
        ///     
        /// there should be no changes after applying remove and revering it       
        /// </summary>
        [TestMethod]
        public void AddRemoveNodeRemoveApplyAndRevertWhenSecondParentHas3ChildrenTest()
        {
            Node parent1 = new Node(NodeType.add, 0);
            Node parent2 = new Node(NodeType.compare, 0);
            Node child1 = new Node(NodeType.copy, 0);
            Node child3 = new Node(NodeType.copy, 0);
            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);
            Node output = new Node(NodeType.log, 0);

            parent1.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(child1);
            parent2.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(child3);
            nodeToBeRemoved.ConnectWithOutput(output);

            dummyNetwork.Operations.Add(parent1);
            dummyNetwork.Operations.Add(parent2);
            dummyNetwork.Operations.Add(output);
            dummyNetwork.Operations.Add(child1);
            dummyNetwork.Operations.Add(child3);
            dummyNetwork.Operations.Add(nodeToBeRemoved);

            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,
                node: nodeToBeRemoved,
                parent1: parent1,
                parent2: parent2,
                outputs: new List<Node>(nodeToBeRemoved.Outputs),
                network: dummyNetwork);

            mutator.Apply();
            mutator.Revert();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsTrue(parent1.Outputs.Contains(nodeToBeRemoved));

            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);
            Assert.AreEqual(parent1, nodeToBeRemoved.Inputs.First.Value);
            Assert.AreEqual(parent2, nodeToBeRemoved.Inputs.Last.Value);
            Assert.AreEqual(1, nodeToBeRemoved.Outputs.Count);
            Assert.AreEqual(output, nodeToBeRemoved.Outputs.First.Value);
            
            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(3, parent2.Outputs.Count);
            Assert.AreEqual(child1, parent2.Outputs.First.Value);
            Assert.AreEqual(nodeToBeRemoved, parent2.Outputs.First.Next.Value);
            Assert.AreEqual(child3, parent2.Outputs.Last.Value);
            
            Assert.AreEqual(1, output.Inputs.Count);
            Assert.AreEqual(nodeToBeRemoved, output.Inputs.First.Value);
            Assert.AreEqual(0, output.Outputs.Count);

            Assert.AreEqual(1, child1.Inputs.Count);
            Assert.AreEqual(parent2, child1.Inputs.First.Value);
            Assert.AreEqual(0, child1.Outputs.Count);

            Assert.AreEqual(1, child3.Inputs.Count);
            Assert.AreEqual(parent2, child3.Inputs.First.Value);
            Assert.AreEqual(0, child3.Outputs.Count);
        }

        /// <summary>
        /// on graph like:
        /// 
        /// parent1 ---> _nodeToBeRemoved -> output
        ///              /| 
        ///             / 
        ///            /
        ///           |
        ///           |
        ///           /
        ///  parent2 /
        ///   ||
        ///   ||
        ///   \/
        ///   C2
        ///     
        /// there should be no changes after applying remove and revering it       
        /// </summary>
        [TestMethod]
        public void AddRemoveNodeRemoveApplyAndRevertWhenSecondParentHas2AndNodeToRemoveIsFistOneChildrenTest()
        {
            Node parent1 = new Node(NodeType.add, 0);
            Node parent2 = new Node(NodeType.compare, 0);
            Node child2 = new Node(NodeType.copy, 0);
            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);
            Node output = new Node(NodeType.log, 0);

            parent1.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(child2);
            nodeToBeRemoved.ConnectWithOutput(output);

            dummyNetwork.Operations.Add(parent1);
            dummyNetwork.Operations.Add(parent2);
            dummyNetwork.Operations.Add(output);
            dummyNetwork.Operations.Add(child2);
            dummyNetwork.Operations.Add(nodeToBeRemoved);

            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,
                node: nodeToBeRemoved,
                parent1: parent1,
                parent2: parent2,
                outputs: new List<Node>(nodeToBeRemoved.Outputs),
                network: dummyNetwork);

            mutator.Apply();
            mutator.Revert();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsTrue(parent1.Outputs.Contains(nodeToBeRemoved));

            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);
            Assert.AreEqual(parent1, nodeToBeRemoved.Inputs.First.Value);
            Assert.AreEqual(parent2, nodeToBeRemoved.Inputs.Last.Value);
            Assert.AreEqual(1, nodeToBeRemoved.Outputs.Count);
            Assert.AreEqual(output, nodeToBeRemoved.Outputs.First.Value);

            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(2, parent2.Outputs.Count);
            Assert.AreEqual(nodeToBeRemoved, parent2.Outputs.First.Value);
            Assert.AreEqual(child2, parent2.Outputs.Last.Value);

            Assert.AreEqual(1, output.Inputs.Count);
            Assert.AreEqual(nodeToBeRemoved, output.Inputs.First.Value);
            Assert.AreEqual(0, output.Outputs.Count);

            Assert.AreEqual(1, child2.Inputs.Count);
            Assert.AreEqual(parent2, child2.Inputs.First.Value);
            Assert.AreEqual(0, child2.Outputs.Count);
        }


        /// <summary>
        /// on graph like:
        /// 
        /// parent1 ---> _nodeToBeRemoved -> output
        ///              /| 
        ///             / 
        ///   C1       /
        ///   /\      |
        ///   ||      |
        ///   ||      /
        ///  parent2 /
        ///     
        /// 
        /// there should be no changes after applying remove and revering it       
        /// </summary>
        [TestMethod]
        public void AddRemoveNodeRemoveApplyAndRevertWhenSecondParentHas2AndNodeToRemoveIsLastOneChildrenTest()
        {
            Node parent1 = new Node(NodeType.add, 0);
            Node parent2 = new Node(NodeType.compare, 0);
            Node child1 = new Node(NodeType.copy, 0);
            Node nodeToBeRemoved = new Node(NodeType.inverse, 0);
            Node output = new Node(NodeType.log, 0);

            parent1.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(child1);
            parent2.ConnectWithOutput(nodeToBeRemoved);
            nodeToBeRemoved.ConnectWithOutput(output);

            dummyNetwork.Operations.Add(parent1);
            dummyNetwork.Operations.Add(parent2);
            dummyNetwork.Operations.Add(output);
            dummyNetwork.Operations.Add(child1);
            dummyNetwork.Operations.Add(nodeToBeRemoved);

            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,
                node: nodeToBeRemoved,
                parent1: parent1,
                parent2: parent2,
                outputs: new List<Node>(nodeToBeRemoved.Outputs),
                network: dummyNetwork);

            mutator.Apply();
            mutator.Revert();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsTrue(parent1.Outputs.Contains(nodeToBeRemoved));

            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);
            Assert.AreEqual(parent1, nodeToBeRemoved.Inputs.First.Value);
            Assert.AreEqual(parent2, nodeToBeRemoved.Inputs.Last.Value);
            Assert.AreEqual(1, nodeToBeRemoved.Outputs.Count);
            Assert.AreEqual(output, nodeToBeRemoved.Outputs.First.Value);

            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(2, parent2.Outputs.Count);
            Assert.AreEqual(child1, parent2.Outputs.First.Value);
            Assert.AreEqual(nodeToBeRemoved, parent2.Outputs.Last.Value);

            Assert.AreEqual(1, output.Inputs.Count);
            Assert.AreEqual(nodeToBeRemoved, output.Inputs.First.Value);
            Assert.AreEqual(0, output.Outputs.Count);

            Assert.AreEqual(1, child1.Inputs.Count);
            Assert.AreEqual(parent2, child1.Inputs.First.Value);
            Assert.AreEqual(0, child1.Outputs.Count);
        }        [TestMethod]        public void AddRemoveNodeApplyAndRevert1000MutationsHashTest()        {            Assert.Inconclusive();            throw new NotImplementedException();        }    }}