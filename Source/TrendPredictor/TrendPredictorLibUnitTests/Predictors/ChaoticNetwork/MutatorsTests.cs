using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrendPredictorLib;

namespace TrendPredictorLibUnitTests
{
    [TestClass]
    public class MutatorsTests
    {
        class DummyMutator : NetworkMutator
        {
            public int appliesCounter = 0;
            public int revertsCounter = 0;

            protected override void ApplyAction()
            {
                appliesCounter++;
            }

            protected override void RevertAction()
            {
                revertsCounter++;
            }
        }

        [TestMethod]
        public void NetworkMutatorOkCaseTest()
        {
            DummyMutator mutator = new DummyMutator();
            Assert.AreEqual(0, mutator.appliesCounter);
            Assert.AreEqual(0, mutator.revertsCounter);

            mutator.Apply();
            Assert.AreEqual(1, mutator.appliesCounter);
            Assert.AreEqual(0, mutator.revertsCounter);

            mutator.Revert();
            Assert.AreEqual(1, mutator.appliesCounter);
            Assert.AreEqual(1, mutator.revertsCounter);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void NetworkMutatorShouldThrowOnDoubleApply()
        {
            DummyMutator mutator = new DummyMutator();

            mutator.Apply();
            mutator.Apply();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void NetworkMutatorShouldThrowOnDoubleRevert()
        {
            DummyMutator mutator = new DummyMutator();

            mutator.Apply();

            mutator.Revert();
            mutator.Revert();
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void NetworkMutatorShouldThrowOnRevertWOApply()
        {
            DummyMutator mutator = new DummyMutator();

            mutator.Revert();
        }

        [TestMethod]
        public void NodeTypeChangeApplyTest()
        {
            Node node = new Node(NodeType.compare);

            NodeTypeChange mutator = new NodeTypeChange(node, NodeType.compare, NodeType.add);
            mutator.Apply();

            Assert.AreEqual(NodeType.add, node.NodeType);
        }

        [TestMethod]
        public void NodeTypeChangeRevertTest()
        {
            Node node = new Node(NodeType.compare);

            NodeTypeChange mutator = new NodeTypeChange(node, NodeType.compare, NodeType.add);
            mutator.Apply();
            mutator.Revert();
            
            Assert.AreEqual(NodeType.compare, node.NodeType);
        }

        /// <summary>
        /// its should be change 
        /// from:
        /// 
        ///     parent1 ---> output
        /// 
        ///     parent2 (not connected)
        ///     
        /// to:
        ///     parent1 ---> _nodeToBeAdded -> output
        ///                  /|
        ///                 /
        ///         parent2/
        /// </summary>
        [TestMethod]
        public void AddRemoveNodeApplyTest()
        {
            Node parent1 = new Node(NodeType.add);
            Node parent2 = new Node(NodeType.add);
            Node output = new Node(NodeType.add);
            parent1.ConnectWithOutput(output);

            Node nodeToBeAdded = new Node(NodeType.add);
            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Add, 
                node: nodeToBeAdded, 
                parent1: parent1, 
                parent2: parent2, 
                output: output);

            mutator.Apply();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsNotNull(parent1.Outputs.Find(nodeToBeAdded));

            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(1, parent2.Outputs.Count);
            Assert.IsNotNull(parent2.Outputs.Find(nodeToBeAdded));

            Assert.AreEqual(2, nodeToBeAdded.Inputs.Count);
            Assert.IsNotNull(nodeToBeAdded.Inputs.Find(parent1));
            Assert.IsNotNull(nodeToBeAdded.Inputs.Find(parent2));
            Assert.AreEqual(1, nodeToBeAdded.Outputs.Count);
            Assert.IsNotNull(nodeToBeAdded.Outputs.Find(output));

            Assert.AreEqual(1, output.Inputs.Count);
            Assert.IsNotNull(output.Inputs.Find(nodeToBeAdded));
            Assert.AreEqual(0, output.Outputs.Count);

        }

        /// <summary>
        /// its should be change from:
        /// from:
        ///     parent1 ---> _nodeToBeRemoved -> output
        ///                  /|
        ///                 /
        ///         parent2/
        ///         
        /// to:
        ///     parent1 ---> output
        /// 
        ///     parent2 (not connected)
        /// </summary>
        [TestMethod]
        public void AddRemoveNodeRemoveTest()
        {
            Node parent1 = new Node(NodeType.add);
            Node parent2 = new Node(NodeType.compare);
            Node nodeToBeRemoved = new Node(NodeType.inverse);
            Node output = new Node(NodeType.log);

            parent1.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(nodeToBeRemoved);
            nodeToBeRemoved.ConnectWithOutput(output);

            Node nodeToBeAdded = new Node(NodeType.add);
            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,
                node: nodeToBeRemoved,
                parent1: parent1,
                parent2: parent2,
                output: output);
            mutator.Apply();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsNotNull(parent1.Outputs.Find(output));

            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(0, parent2.Outputs.Count);

            Assert.AreEqual(1, output.Inputs.Count);
            Assert.IsNotNull(output.Inputs.Find(parent1));
            Assert.AreEqual(0, output.Outputs.Count);
        }

        /// <summary>
        /// on graph like:
        ///     parent1 ---> output
        /// 
        ///     parent2 (not connected)
        ///     
        /// there should be no changes after applying add and revering it
        /// </summary>
        [TestMethod]
        public void AddRemoveNodeAddApplyAndRevertTest()
        {
            Node parent1 = new Node(NodeType.add);
            Node parent2 = new Node(NodeType.add);
            Node output = new Node(NodeType.add);
            parent1.ConnectWithOutput(output);

            Node nodeToBeAdded = new Node(NodeType.add);
            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Add,
                node: nodeToBeAdded,
                parent1: parent1,
                parent2: parent2,
                output: output);

            mutator.Apply();
            mutator.Revert();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsNotNull(parent1.Outputs.Find(output));

            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(0, parent2.Outputs.Count);

            Assert.AreEqual(1, output.Inputs.Count);
            Assert.IsNotNull(output.Inputs.Find(parent1));
            Assert.AreEqual(0, output.Outputs.Count);
        }

        /// <summary>
        /// on graph like:
        /// parent1 ---> _nodeToBeRemoved -> output
        ///                  /|
        ///                 /
        ///         parent2/
        ///     
        /// there should be no changes after applying remove and revering it       
        /// </summary>
        [TestMethod]
        public void AddRemoveNodeRemoveApplyAndRevertTest()
        {
            Node parent1 = new Node(NodeType.add);
            Node parent2 = new Node(NodeType.compare);
            Node nodeToBeRemoved = new Node(NodeType.inverse);
            Node output = new Node(NodeType.log);

            parent1.ConnectWithOutput(nodeToBeRemoved);
            parent2.ConnectWithOutput(nodeToBeRemoved);
            nodeToBeRemoved.ConnectWithOutput(output);

            AddRemoveNode mutator = new AddRemoveNode(
                nodeChangeType: AddRemoveNode.NodeChangeType.Remove,
                node: nodeToBeRemoved,
                parent1: parent1,
                parent2: parent2,
                output: output);

            mutator.Apply();
            mutator.Revert();

            Assert.AreEqual(0, parent1.Inputs.Count);
            Assert.AreEqual(1, parent1.Outputs.Count);
            Assert.IsNotNull(parent1.Outputs.Find(nodeToBeRemoved));

            Assert.AreEqual(0, parent2.Inputs.Count);
            Assert.AreEqual(1, parent2.Outputs.Count);
            Assert.IsNotNull(parent2.Outputs.Find(nodeToBeRemoved));

            Assert.AreEqual(2, nodeToBeRemoved.Inputs.Count);
            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent1));
            Assert.IsNotNull(nodeToBeRemoved.Inputs.Find(parent2));
            Assert.AreEqual(1, nodeToBeRemoved.Outputs.Count);
            Assert.IsNotNull(nodeToBeRemoved.Outputs.Find(output));

            Assert.AreEqual(1, output.Inputs.Count);
            Assert.IsNotNull(output.Inputs.Find(nodeToBeRemoved));
            Assert.AreEqual(0, output.Outputs.Count);
        }
    }
}
