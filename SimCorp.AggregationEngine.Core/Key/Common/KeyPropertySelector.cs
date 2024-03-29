﻿using SimCorp.AggregationEngine.Core.Domain;
using System.Reflection.Emit;
using System.Reflection;
using SimCorp.AggregationEngine.Core.Key.KeyAttributes;

namespace SimCorp.AggregationEngine.Core.Key.Common;

public class KeyPropertySelector : IKeyPropertySelector
{
    private readonly IDictionary<Type, object> typeSolver = new Dictionary<Type, object>();

    public KeyValuePair<string, string?>? GetPropertyWithAggregationLevel<T>(T value, AggregationLevel aggregationLevel) where T : IMetaData
    {
        var selector = getTypeSolver<T>().aggregationLevelAttributeSelector;
        if (selector.ContainsKey(aggregationLevel))
        {
            return selector[aggregationLevel](value);
        }
        return default;
    }

    public IReadOnlyDictionary<string, string?> GetPropertiesWithKeyAttribute<T>(T value) where T : IParameters
    {
        var getters = getTypeSolver<T>().keyPropertyAttributeSelector;
        return new Dictionary<string, string?>(getters.Select(fun => fun(value)));
    }

    private GenericTypeSelectorSolver<T> getTypeSolver<T>()
    {
        var type = typeof(T);
        if (!typeSolver.ContainsKey(type))
        {
            typeSolver[type] = new GenericTypeSelectorSolver<T>();
        }
        var res = typeSolver[type] as GenericTypeSelectorSolver<T>;
        if (res == null)
        {
            throw new NullReferenceException($"Could not resolve type solver {type.Name}");
        }
        return res;
    }

    private class GenericTypeSelectorSolver<T>
    {
        public delegate KeyValuePair<string, string?> GetPropertyInfo(T value);

        public IDictionary<AggregationLevel, GetPropertyInfo> aggregationLevelAttributeSelector { get; }
        public IList<GetPropertyInfo> keyPropertyAttributeSelector { get; }

        public GenericTypeSelectorSolver()
        {
            aggregationLevelAttributeSelector = new Dictionary<AggregationLevel, GetPropertyInfo>();
            initializeSelectorsForAggregationLevelAttribute();
            keyPropertyAttributeSelector = new List<GetPropertyInfo>();
            initializeSelectorsForKeyPropertyAttribute();
        }

        private void initializeSelectorsForKeyPropertyAttribute()
        {
            var interfaceTypes = typeof(T).GetInterfaces().ToList();
            if (typeof(T).IsInterface) { interfaceTypes.Add(typeof(T)); }

            var properties = interfaceTypes.SelectMany(x => x.GetProperties())
                                           .Where(x => x.GetMethod != null)
                                           .Where(x => x.GetCustomAttribute<KeyPropertyAttribute>() != null);

            foreach (PropertyInfo property in properties)
            {
                var getPropertyInfo = buildGetPropertyInfoSelector(property);
                keyPropertyAttributeSelector.Add(getPropertyInfo);
            }
        }

        private void initializeSelectorsForAggregationLevelAttribute()
        {
            var interfaceTypes = typeof(T).GetInterfaces().ToList();
            if(typeof(T).IsInterface) { interfaceTypes.Add(typeof(T));}

            var properties = interfaceTypes.SelectMany( x => x.GetProperties())
                                           .Where(x => x.GetMethod != null)
                                           .Where(x => x.GetCustomAttribute<AggregationLevelAttribute>() != null);

            foreach (PropertyInfo property in properties)
            {
                var aggregationLevelAttribute = property.GetCustomAttribute<AggregationLevelAttribute>(false);
                var getPropertyInfo = buildGetPropertyInfoSelector(property);
                aggregationLevelAttributeSelector.Add(aggregationLevelAttribute!.AggregationLevel, getPropertyInfo);
            }
        }

        private GetPropertyInfo buildGetPropertyInfoSelector(PropertyInfo property)
        {
            var propertyValueGetter = buildPropertyValueGetter(property);

            GetPropertyInfo getter = (value) =>
            {
                var result = propertyValueGetter.DynamicInvoke(value);
                return KeyValuePair.Create(property.Name, result == null ? null : result.ToString());
            };

            return getter;
        }

        private Delegate buildPropertyValueGetter(PropertyInfo property)
        {
            Type[] args = { typeof(T) };
            var methodName = $"get{property.Name}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}";
            var GetPropertyValue = new DynamicMethod(methodName, property.PropertyType, args, GetType().Module);
            var getPropertyMethod = property.GetGetMethod();
            if (getPropertyMethod == null)
            {
                throw new NullReferenceException($"Could not resolve getter method for property ${property.Name}.");
            }

            var il = GetPropertyValue.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, getPropertyMethod, null);
            il.Emit(OpCodes.Ret);

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
            return GetPropertyValue.CreateDelegate(delegateType);
        }

    }
}
