using AggregationEngine.Core.Domain;
using System.Collections;

namespace AggregationEngine.Core;

public class DefaultAggregationTree<TKey, TValue, TLevelKey> where TKey : IKey
{
    private readonly IAggregationTreeFactory<TKey, TValue> aggregationTreeFactory;
    private readonly IAggregationStructure aggregationStructure;
    private readonly Func<AggregationLevel, IAggregationPosition, object> mapToLevelKey;

    private readonly IDictionary<AggregationLevel,IDictionary<TKey, INode<TKey,TValue>>> nodes;

    public DefaultAggregationTree(  IAggregationTreeFactory<TKey, TValue> aggregationTreeFactory, 
                                    IAggregationStructure aggregationStructure,
                                    Func<AggregationLevel, IAggregationPosition, object> mapToLevelKey)
    {
        this.aggregationTreeFactory = aggregationTreeFactory ?? throw new ArgumentNullException(nameof(aggregationTreeFactory));
        this.aggregationStructure = aggregationStructure ?? throw new ArgumentNullException(nameof(aggregationStructure));
        this.mapToLevelKey = mapToLevelKey ?? throw new ArgumentNullException( nameof(mapToLevelKey));
    }

    public void AddNode(IAggregationPosition position, Action<TValue, IAggregationPosition> evaluator)
    {
        var keyBuilder = aggregationTreeFactory.CreateKeyBuilder(position);
        var parentKey = aggregationTreeFactory.CreateEmptyKey();
        var parentLevel = AggregationLevel.None;
        for (var level = aggregationStructure.Next(parentLevel); level != AggregationLevel.None;)
        {
            keyBuilder.Add(level);
            var key = keyBuilder.Build().First();
            INode<TKey, TValue>? node = null;
            if (!nodes[level].TryGetValue(key, out node))
            {
                node = aggregationTreeFactory.CreateNode();
                nodes[level].Add(key, node);
                node.ParentKey = parentKey;
            }
            evaluator(node.Value, position);

            if (parentLevel != AggregationLevel.None)
            {
                var parentNode = nodes[parentLevel][parentKey];
                if(!parentNode.ChildrenKeys.Contains(key))
                {
                    parentNode.ChildrenKeys.Add(key);
                }
            }
            parentLevel = level;
            parentKey = key; 
        }
    }

    public void AddRange(IEnumerable<IAggregationPosition> positions, Action<TValue, IAggregationPosition> evaluator, Func<IEnumerable<IAggregationPosition>, IAggregationPosition> accumulator)
    {
        var keyBuilder = aggregationTreeFactory.CreateKeyBuilder(positions);
        foreach (var level in aggregationStructure)
        {
            foreach(var key in keyBuilder.Add(level).Build().Except(nodes[level].Keys)){
                nodes[level].Add(key, aggregationTreeFactory.CreateNode());
            }
        }

        if(aggregationStructure.Count <= 1)
        {
            return;
        }

        

        for(var level = aggregationStructure.Last(); level != aggregationStructure.First(); level = aggregationStructure.Previous(level))
        {
            int index = aggregationStructure.IndexOf(level) - 1;
            foreach(var val in nodes[level])
            {
                if (val.Value.ParentKey.IsEmpty)
                {
                    //val.Value.ParentKey = val.Key.GetSubKey(index);
                }
            }

        }

        var previousLevel = aggregationStructure.Last();
        var keyValPairs = keyBuilder.Build().Zip(positions, (x, y) => new {Key = x, Value = y});

        for(int index = aggregationStructure.IndexOf(previousLevel); index >= 0; index--)
        {
            keyValPairs = keyValPairs.GroupBy(x => x.Key).Select(x => new { Key = x.Key, Value = accumulator(x.Select(y => y.Value)) });

        }
    }

    public IEnumerable<DefaultAggregationTree<TKey, TValue, TLevelKey>> DecomposeBottom(AggregationLevel level, Action<TValue, IAggregationPosition> evaluator)
    {
        var setOfTrees = new List<IDictionary<AggregationLevel, IDictionary<object, INode<TValue>>>>();
        var subStructure = aggregationStructure.GetSubStructure(level, AggregationLevel.None);
        foreach(var pair in nodes[level])
        {
            setOfTrees.Add(new Dictionary<AggregationLevel, IDictionary<object, INode<TValue>>>());
            foreach (var subLevel in subStructure)
            {
                setOfTrees.Last().Add(subLevel, new Dictionary<object, INode<TValue>>());
            }
            setOfTrees.Last()[level].Add(pair.Key, pair.Value);
        }

        foreach (var subTree in setOfTrees)
        {
            foreach (var subLevel in subStructure)
            {
                var childrenNodeKeys = subTree[subLevel].Values.SelectMany(x => x.ChildrenKeys).ToHashSet();
                var nextSubLevel = subStructure.Next(subLevel);
                if(nextSubLevel != AggregationLevel.None)
                {
                    subTree[nextSubLevel] = nodes[nextSubLevel].Where(x => childrenNodeKeys.Contains(x.Key)).ToDictionary(x=>x.Key, x=>x.Value);
                }
            }
        }

        return setOfTrees.Select(x => new DefaultAggregationTree<TKey, TValue, TLevelKey>(aggregationTreeFactory, aggregationStructure, mapToLevelKey));
    }




}
