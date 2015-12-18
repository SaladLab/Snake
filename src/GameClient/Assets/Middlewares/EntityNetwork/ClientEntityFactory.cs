using System;
using System.Collections.Concurrent;
using EntityNetwork;
using EntityNetwork.Unity3D;
using UnityEngine;

public class ClientEntityFactory : IClientEntityFactory
{
    private static ClientEntityFactory _default;

    public static ClientEntityFactory Default
    {
        get { return _default ?? (_default = new ClientEntityFactory()); }
    }

    public Transform RootTransform { get; set; }

    private readonly ConcurrentDictionary<Type, Type> _clientEntityToProtoTypeMap =
        new ConcurrentDictionary<Type, Type>();

    Type IClientEntityFactory.GetProtoType(Type entityType)
    {
        return _clientEntityToProtoTypeMap.GetOrAdd(entityType, t =>
        {
            var type = entityType;
            while (type != null && type != typeof(object))
            {
                if (type.Name.EndsWith("ClientBase"))
                {
                    var typePrefix = type.Namespace.Length > 0 ? type.Namespace + "." : "";
                    var protoType = type.Assembly.GetType(typePrefix + "I" +
                                                          type.Name.Substring(0, type.Name.Length - 10));
                    if (protoType != null && typeof(IEntityPrototype).IsAssignableFrom(protoType))
                    {
                        return protoType;
                    }
                }
                type = type.BaseType;
            }
            return null;
        });
    }

    IClientEntity IClientEntityFactory.Create(Type protoType)
    {
        var resourceName = "Client" + protoType.Name.Substring(1);
        var resource = Resources.Load(resourceName);
        if (resource == null)
            throw new InvalidOperationException("Failed to load resource(" + resourceName + ")");

        var go = (GameObject)GameObject.Instantiate(resource);
        if (go == null)
            throw new InvalidOperationException("Failed to instantiate resource(" + resourceName + ")");

        if (RootTransform != null)
            go.transform.SetParent(RootTransform, false);

        return go.GetComponent<IClientEntity>();
    }

    void IClientEntityFactory.Delete(IClientEntity entity)
    {
        var enb = ((EntityNetworkBehaviour)entity);
        GameObject.Destroy(enb.gameObject);
    }
}
