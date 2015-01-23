using System;
using UnityEngine;

namespace MonoBehavioural
{

    public static class StateExtensions
    {
        public static void ChangeState<T>(this MonoBehaviour obj)
        {
            StateExtensions.ChangeState(obj, typeof(T));
        }

        public static void ChangeState(this MonoBehaviour obj, System.Type newState)
        {
            Debug.LogError("No ChangeState method defined for " + obj.GetType());
        }

        public static void PushState<T>(this MonoBehaviour obj)
        {
            StateExtensions.PushState(obj, typeof(T));
        }

        public static void PushState(this MonoBehaviour obj, System.Type newState)
        {
            Debug.LogError("No PushState method defined for " + obj.GetType());
        }

        public static void PopState(this MonoBehaviour obj)
        {
            Debug.LogError("No PopState method defined for " + obj.GetType());
        }

        public static string GetActiveState(this MonoBehaviour obj)
        {
            Debug.LogError("No GetState method defined for " + obj.GetType());
            return null;
        }
    }

    // Indicates that this nested type is to be used as a state
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class StateAttribute : System.Attribute
    {
        public bool ReplaceBase { get; set; }
        // TODO: implement ReplaceState
        //public System.Type ReplaceState { get; set; }
        public StateAttribute()
        {
        }
    }

    // Indicates the initial state that this class should use
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class InitialStateAttribute : System.Attribute
    {
        public System.Type Identifier { get; private set; }
        public InitialStateAttribute(System.Type stateType)
        {
            this.Identifier = stateType;
        }
    }

    // Indicates that a method shouldn't be used as a state override
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreAttribute : System.Attribute
    {
        public IgnoreAttribute()
        {
        }
    }

    // Indicates a method in a state machine behaviour that should act as the callback for when 
    //  the current state has no handler, or there is no current state
    [AttributeUsage(AttributeTargets.Method)]
    public class DefaultHandlerAttribute : System.Attribute
    {
        public string EventName { get; set; }
        public DefaultHandlerAttribute(string name)
        {
            this.EventName = name;
        }
    }

}