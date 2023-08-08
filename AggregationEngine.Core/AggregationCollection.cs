using AggregationEngine.Core.Domain;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace AggregationEngine.Core;

public class AggregationCollection<TValue> : IAggregationCollection<IKey, TValue> 
{
    private INode<TValue> root { get; }
    private IDictionary<IKey,INode<TValue>> leaves { get; }
    private IList<IDictionary<IKey, INode<TValue>>> nodes;
    private IAggregationStructure aggregationStructure;

    private readonly IAggregationCollectionFactory<IKey, TValue> aggregationCollectionFactory;

    public AggregationCollection(IAggregationCollectionFactory<IKey, TValue> aggregationCollectionFactory, IAggregationStructure aggregationStructure)
    {
        this.aggregationCollectionFactory = aggregationCollectionFactory ?? throw new ArgumentNullException(nameof(aggregationCollectionFactory));
        this.aggregationStructure = aggregationStructure ?? throw new ArgumentNullException(nameof(aggregationStructure));
        root = aggregationCollectionFactory.CreateNode(AggregationLevel.Root);
        leaves = new Dictionary<IKey,INode<TValue>>();  
        nodes = new List<IDictionary<IKey, INode<TValue>>>();
        foreach(var _ in aggregationStructure)
        {
            nodes.Add(new Dictionary<IKey, INode<TValue>> { });
        }
        nodes[0][root.Key] = root;
    }


    public TValue this[IKey key] => throw new NotImplementedException();

    public IEnumerable<IKey> Keys => throw new NotImplementedException();

    public IEnumerable<TValue> Values => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public void AddLeaf(IAggregationPosition position, Action<TValue, IAggregationPosition> updateValue)
    {
        if (containPosition(position))
        {
            throw new Exception("Aggregation position is already excist!");
        }
        var node = root;
        var keyBuilder = aggregationCollectionFactory.CreateKeyBuilder(position);
        keyBuilder.Add(node.AggregationLevel);
        for(var nextLevel = aggregationStructure.Next(node.AggregationLevel); nextLevel != AggregationLevel.None; nextLevel = aggregationStructure.Next(node.AggregationLevel)) {
            updateValue(node.Value, position);
            node = moveToNextNode(node, nextLevel,keyBuilder);
        }
        node.Position = position;
    }

    private INode<TValue> moveToNextNode(INode<TValue> parent, AggregationLevel childLevel, IKeyBuilder keyBuilder)
    {
        keyBuilder.Add(childLevel);
        var childKey = keyBuilder.Build();
        var levelNodes = nodes[aggregationStructure.IndexOf(childLevel)];
        INode<TValue>? childNode;
        if (!levelNodes.TryGetValue(childKey, out childNode))
        {
            childNode.Parent = parent.Key;
            childNode.Key = childKey;
            levelNodes.Add(childKey, childNode);
            parent.Children.Add(childNode.Key);
        }
        return childNode;
    }

    private bool containPosition(IAggregationPosition position)
    {
        var keyBuilder = aggregationCollectionFactory.CreateKeyBuilder(position);
        keyBuilder.Add(aggregationStructure);
        var key = keyBuilder.Build();
        return ContainsKey(key);
    }

    public void AddLeaves(IEnumerable<IAggregationPosition> positions, Action<TValue, IAggregationPosition> updateValue)
    {
        if(positions.All(x => containPosition(x)))
        {
            throw new Exception("Violent uniq aggrigation position constrains");
        }
        foreach(var position in positions)
        {
            AddLeaf(position, updateValue);
        }
    }

    public void ApplyOperation(Action<TValue, IParameter, IAggregationPosition> updateValue)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(IKey key)
    {
        return leaves.ContainsKey(key);
    }

    public IEnumerable<IAggregationCollection<IKey, TValue>> Decompose(AggregationLevel aggregationLevel)
    {
        var res = new List<IAggregationCollection<IKey, TValue>>();
        var levelNodes = nodes[aggregationStructure.IndexOf(aggregationLevel)];
        

    }

    public IEnumerable<IAggregationCollection<IKey, TValue>> DecomposeAndPopulate(AggregationLevel aggregationLevel)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IAggregationPosition> GetAggregationPositions(AggregationLevel aggregationLevel)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<IKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void ReconstructStructure(IAggregationStructure newAggregationStructure)
    {
        throw new NotImplementedException();
    }

    public void RemoveLeaf(IKey key)
    {
        for(var node = leaves[key]; node.AggregationLevel != AggregationLevel.Root;)
        {
            var previousLevel = aggregationStructure.Previous(node.AggregationLevel);
            var parent = nodes[aggregationStructure.IndexOf(previousLevel)][node.Parent];
            parent.Children.Remove(node.Key);
            nodes[aggregationStructure.IndexOf(node.AggregationLevel)].Remove(node.Key);
            node = parent;
        }
    }

    public void RemoveLeaves(IEnumerable<IKey> keys)
    {
        foreach(var key in keys)
        {
            RemoveLeaf(key);
        }
    }

    public bool TryGetValue(IKey key, [MaybeNullWhen(false)] out TValue value)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
