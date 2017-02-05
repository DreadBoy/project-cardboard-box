using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class DependencyFactory : MonoBehaviour
{
    public List<Dependency> implementations = new List<Dependency>()
    {
    };



    public class Dependency
    {
        public Type Interface { get; set; }
        public Type Implementation { get; set; }

        
    }

    public Type GetImplementation<T>() where T : IDependency
    {
        foreach (var imp in implementations)
        {
            if (typeof(T) == imp.Interface)
                return imp.Implementation;
        }

        return null;
    }

    public void GetExistingInstance<T>() where T : IDependency
    {

    }
}